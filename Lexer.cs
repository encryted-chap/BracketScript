using System.IO;
using System;
using System.Collections.Generic;
using MoonSharp.Interpreter;

namespace BracketScript
{
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

                
            }
            ret.AddRange(tc.End()); // parse unmanaged_tokens to Tokens
            // prepare execution environment
            Lexer.currentScope = new Scope("global"); // initialize global scope

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
                        indent = line_indent // get the tabs
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
        int Line; // the line that this token takes place
        public int indent; // the indentation present on this token
        public enum TokenType {
            function_dec, var_dec, equation, // abstract
            eq_operator, keyword,
            class_dec, empty_line, 
            str, num,  // constants
            unknown_symbol, class_type, 
            variable_name, function_name,
            var_op
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
            Debug.Error($"At line {Line}: {e.Message}");
            Environment.Exit(0);
        }
        
        // takes a single token and executes it
        public static void Execute(Token[] T) {
            for(int i = 0; i < T.Length; i++) {
                switch(T[i].t_type) {
                    case TokenType.var_dec:
                        // create new variable
                        Variable nv = new Variable() {
                            name = T[i].GetData(1), // data[1] is the name
                            retType = Lexer.currentScope.contained_c[T[i].GetData(0)], // classname
                            isNull = true, // obviously hasn't been allocated yet
                        };
                        nv.Alloc(); // allocate this variable in memory_manager
                        Lexer.currentScope.contained_v.Add(nv.name, nv); // register as var
                        break;
                }
            }
        }
        public Token() {
            CurrentLine++; 
            global.gen_id(out _refid); // generate randomized identifier
        }
    }
}