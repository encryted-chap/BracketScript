using System.IO;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace BracketScript
{
    using static global;
    public class PreProcessor {
        static List<string> code;
        public static List<string> Process(string[] raw_code) {
            code = new List<string>();
            code.AddRange(raw_code);
            if(code.Count != 0) {
                Stage1(); // basic preprocessing
                Stage2(); // clean up
            } else {
                Debug.Error("Empty file fed to preprocessor! Exiting...");
                Environment.Exit(0);
            }
            return PreProcessor.code;
        }
        public static void Stage1() {
            for(int i = 0; i < code.Count; i++) {
                // replace 4 spaces with \t
                int trailers = code[i].Length - code[i].Trim().Length; // len - len without whitepace
                code[i] = code[i].Trim();
                // now replace :)
                for(; trailers > 0; trailers--) {
                    code[i] = $"\t{code[i]}";
                }
            }
        }
        public static void Stage2() {
        }
    }
    public class Lexer {
        public static Scope currentScope;
        public static List<Token> Lexify (string input) {
            Debug.Message("Lexer invoked");
            // get processor output
            List<string> processed = PreProcessor.Process (
                File.ReadAllLines(input)
            );
            Debug.Success($"Preprocessing complete: {processed.Count}");
            List<Token> ret = new List<Token>(); // token collection
            int cline=0;
            // create new unmanaged token collection:
            TokenCollection tc = new TokenCollection();

            for(int i = 0; i < processed.Count; i++) {
                Script lexerlua = new Script(); // generate new lua script
                lexerlua.Globals["inputline"] = processed[i]; // feed input to lua
                DynValue val = lexerlua.DoFile("lua/lexer.lua"); // load and execute file
                string lines_tokens = val.CastToString(); // get return value
                Debug.Message("token:");
                Debug.Message(lines_tokens);
                foreach(var t in lines_tokens.Split("]["))
                    tc.Add(new unmanaged_token(t, cline)); // register new token collection
                cline++;
                
                
            }

            Console.Clear();
            ret.AddRange(tc.End()); // parse unmanaged_tokens to Tokens
            // prepare execution environment
            Lexer.currentScope = new Scope("global"); // initialize global scope
            // now define base classes
            Class Byte = new Class("byte", currentScope.CreateUnder()) {
                size = 1,
            }; // todo: write function definitions for Byte

            Class integer = new Class("int", currentScope.CreateUnder()) {
                size = 4, // 32 bit
            };
            Function byte_new_int = new Function("new", Byte.classScope, new Variable[] {new Variable() {name="n", retType=integer}}); // todo: int variable type
            byte_new_int.instructions = new List<string>() {
                
            };
            Byte.classScope.contained_f.Add(byte_new_int.name, byte_new_int);
            currentScope.contained_c.Add("byte", Byte);
            currentScope.contained_c.Add("int", integer);

            foreach(var v in Byte.classScope.contained_f) {
                v.Value.DefineASM();
            }
            // now to do a second pass on the tokens
            ret = Parse(currentScope, ret);
            
            return ret;
        }
        public static List<Token> Parse(Scope currentScope, List<Token> toParse) {
            Debug.Message("Continue_Parse");
            List<Token> ret = toParse; 
            for(int i = 0; i < ret.Count; i++) {
                switch(ret[i].t_type) {
                    case Token.TokenType.eq_operator:
                        if(ret[i].data == "=") {
                            //this means we are trying to assign something 
                            //(a lot of this is rough rn, not dynamically decided or even really writing anything )
                            Variable got;
                            Variable toAssign;
                            if(!currentScope.contained_v.TryGetValue(ret[i-1].data, out got)){
                                ret[i--].ThrowHere(new Exception($"There was no variable {ret[i-1].data}"));
                            }
                            if(!currentScope.contained_v.TryGetValue(ret[i+1].data, out toAssign)) {
                                ret[i--].ThrowHere(new Exception($"There was no variable {ret[i+1].data}"));
                            }
                            got.Assign(toAssign);
                            i++;
                            break;
                        } 
                        
                        break;
                    case Token.TokenType.keyword:
                        if(ret[i].data == "pass")
                            asm("\tnop"); // this means do nothing
                        break;
                    case Token.TokenType.unknown_symbol:
                        // first we need to check if its a definition:
                        if(i+1 < ret.Count && ret[i+1].t_type == Token.TokenType.unknown_symbol) {
                            // this means it is a definition
                            string classname = ret[i++].data; // get classname from token
                            string dataname = ret[i].data;
                            // resolve the class
                            if(!currentScope.contained_c.ContainsKey(classname)) // if the class name doesn't exist, throw exception at token
                                ret[i-1].ThrowHere(new Exception($"There was no class {classname} defined in scope {currentScope.refid}"));
                            Class defclass = currentScope.contained_c[classname]; // get class
                            if(i+1 < ret.Count && ret[i+1].t_type == Token.TokenType.eq_operator && ret[i+1].data == "(") {
                                // this means that this token has been resolved as a function definition
                                // cool, now let's try to resolve the arguments and create a new function
                                Function f = new Function(ret[i].data, currentScope, null); // initialize
                                i+=1; // increment to argument list

                                // resolve args
                                List<Variable> template_args = new List<Variable>();

                                while(ret[i].data != ")") {
                                    if(ret[i].data == "," || ret[i].data == "(") {
                                        i++;
                                        continue;
                                    } 
                                    else {
                                        Variable arg = new Variable(); // define dummy variable

                                        // if class wasn't resolved, throw
                                        if(!currentScope.contained_c.TryGetValue(ret[i].data, out arg.retType))
                                            ret[i].ThrowHere(new Exception($"There was no class {ret[i].data} defined in scope {currentScope.refid}"));
                                        arg.name = ret[++i].data;

                                        template_args.Add(arg);
                                        Debug.Success($"Added argument {arg.retType.id}:{arg.name} to {f.name}");
                                    }
                                    i++;
                                }
                                i++;
                                f.arg_template = template_args.ToArray();
                                if(ret[i].data != ":") 
                                    ret[i-1].ThrowHere(new Exception("Illegal function declaration ( maybe missing a : )")); // if it doesnt end with ':', it's not a legal function
                                int function_indent = ret[i+1].indent;
                                // add instructions to function
                                for(; i < ret.Count && (ret[i].t_type == Token.TokenType.empty_line || (i != ret.Count && ret[i].indent >= function_indent)); i++) {
                                    f.toInstructions.Add(ret[i]);
                                } 
                                currentScope.contained_f.Add(f.name, f); // register function
                                Debug.Success("Added " + f.name);
                                f.DefineASM();
                                i--;
                                continue;
                            } else {
                                // this means that it's a Variable declaration, let's make a new variable
                                Variable v = new Variable() {
                                    // fill in the data
                                    retType = defclass,
                                    name = dataname,
                                };
                                if(!currentScope.contained_v.TryAdd(v.name, v))
                                    ret[i-1].ThrowHere(new Exception($"There was already a variable {v.name} defined in scope {Lexer.currentScope.refid}"));
                                asm($"\n; allocate ({defclass.id}){currentScope.refid}::{v.name}");
                                v.Alloc(); // actually allocate variable
                            }
                        } else if(i+1 < ret.Count && ret[i+1].t_type == Token.TokenType.eq_operator && ret[i+1].data == "(") {
                            // function call
                            Debug.Success($"Calling {ret[i].data}");
                            Function toCall = currentScope.contained_f[ret[i].data]; // fetch the function
                            Debug.Success($"Fetched function {toCall.fullname}");
                            
                            int throwindex = i++; // for use later

                            // check arguments
                            if(ret[i].t_type == Token.TokenType.eq_operator && ret[i].data == ")") {
                                // no arg call
                                if(!toCall.ArgsCheck()) { // no args
                                    ret[i-2].ThrowHere(new ArgumentException());
                                } else {
                                    Debug.Success("Arguments correct, calling...");
                                    toCall.Call(); // call function
                                }
                            } else {
                                List<Variable> args = new List<Variable>(); // argument list to pass
                                for(i++; ret[i].data != ")"; i++) {
                                    // get args
                                    args.Add(currentScope.contained_v[ret[i].data]); // add var
                                }
                                if(!toCall.ArgsCheck(args.ToArray())) {
                                    ret[throwindex].ThrowHere(new ArgumentException());
                                } else {
                                    // add arguments to function
                                    toCall.args = args.ToArray(); // assign arguments
                                    toCall.Call(); // now call function
                                }
                            }

                        }
                        break;
                    default:
                        Debug.Error(Enum.GetName<Token.TokenType>(ret[i].t_type));
                        break;
                }
            }
            return ret;
        } 
        
    }
    // token output from lua
    public class unmanaged_token {
        public string type, data; // token = token_type:data
        public int line; // the line this token takes place on
        public unmanaged_token(string t, int line) {
            t = t.TrimStart('[').TrimEnd(']'); // just trim off unnecessary seperators
            
            
            bool isdat=false;
            foreach(var c in t) {
                if(!isdat) {
                    if(c == ':') isdat=true;
                    type += c;
                } else {
                    data += c;
                }
            }
            this.line = line; 
            type = type.Trim(':');
        }
        public override string ToString() {
            return $"[{type}:{data}]";
        }
    }
    // A class for converting a collection of unmanaged tokens into a token
    public class TokenCollection {
        List<unmanaged_token> tokens;
        // registers a single token to this collection
        public void Add(unmanaged_token t) {
            if(object.Equals(null, tokens)) 
                tokens = new List<unmanaged_token>(); // null refs are stinky
            tokens.Add(t);
        }
        public void AddRange(unmanaged_token[] t) {
            // add each val
            foreach(var tok in t) 
                Add(tok); // add token
        }
        // returns the tokens created from this line
        public Token[] End() {
            if(object.Equals(tokens, null))
                tokens = new List<unmanaged_token>();
            List<Token> ret = new List<Token>(); // the list of tokens to return
            int line_indent = 0; // get indentation
            for(int i = 0; i < tokens.Count; i++) {
                if(tokens[i].type == "indent") {
                    line_indent = Convert.ToInt32(tokens[i].data);
                } else {
                    // register new token
                    ret.Add(new Token () {
                        t_type = Enum.Parse<Token.TokenType>(tokens[i].type), // get the token type
                        data = tokens[i].data, // get the raw text data of this token
                        indent = line_indent, // get the tabs
                        Line = tokens[i].line
                    });
                }
            }
            Debug.Success($"Unmanaged tokens parsed: count={ret.Count}"); // for debugging
            return Token.Concat(ret.ToArray()); // return the tokens
        }
    }
    public class Token {
        public static int CurrentLine; // the line this token takes place on
        public string _refid; 
        public string data; // raw text data that makes up this token (not code, config data)
        public int Line; // the line that this token takes place
        public int indent; // the indentation present on this token
        public enum TokenType {
            eq_operator, keyword, empty_line, 
            str, num,  // constants
            unknown_symbol
        } public TokenType t_type;
        // parse tokens into more abstract types (var_dec, )
        public static Token[] Concat(Token[] input) {
            List<Token> ret = new List<Token>();
            ret.AddRange(input); // pass input to return array
            

            return ret.ToArray();
        }
        public string GetData(int index) {
            return data.Split(',')[index];
        }
        
        public void ThrowHere(Exception e) {
            Debug.Error($"At line {Line+1}: {e.Message}");
            Environment.Exit(0);
        }
        public Token() {
            CurrentLine++; 
            global.gen_id(out _refid); // generate randomized identifier
        }
    }
}