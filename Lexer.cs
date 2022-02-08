using System.IO;
using System;
using System.Collections.Generic;
using MoonSharp;
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
            return new List<string>();
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
            // get processor output
            List<string> processed = PreProcessor.Process(
                File.ReadAllLines(input)
            );
            List<Token> ret = new List<Token>(); // token collection
            for(int i = 0; i < processed.Count; i++) {
                // create new unmanaged token collection:
                TokenCollection tc = new TokenCollection();
                DynValue lua_output = Script.RunFile("dummy_file"); // run lua file and grab output
                string[] toks = lua_output.CastToString().Split("]["); // get each token
                foreach(var v in toks) {
                    // create and register new unmanaged token
                    tc.Add( 
                        new unmanaged_token(v, i+1) // grab lua return value
                    );
                }
                ret.AddRange(tc.End()); // parse unmanaged_tokens to Tokens
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
            type = data = string.Empty; // begone, null refs
            type = t.Remove(t.IndexOf(':')); // get anything from before :
            data = t.Substring(t.IndexOf(':')+1); // get anything after the t
            this.line = line; 
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
                Add(tok);
        }
        // returns the tokens created from this line
        public Token[] End() {
            List<Token> ret = new List<Token>(); // the list of tokens to return
            int current = 0; // the current token being developed
            for(int i = 0; i < tokens.Count; i++) {

                if(object.Equals(null, ret[current])) {
                    ret.Add(new Token()); // begone nullrefs
                }
            }
            return ret.ToArray(); // dummy 
        }
    }
    public class Token {
        public static int CurrentLine; // the line this token takes place on
        public string _refid; 
        public string data; // raw text data that makes up this token (not code, config data)
        int Line; // the line that this token takes place
        int indent; // the indentation present on this token
        public enum TokenType {
            function_dec, var_dec,
            eq_operator, keyword,
            class_dec, empty_line, 
            str, integer, floating_int, // constants
            unknown_symbol, class_type, 
            variable_name, function_name
        } TokenType t_type;
        
        // puts types together in order to make an executeable token
        public static Token Concatenate(Token[] tokens) {
            Token ret = new Token(); // the new token to return
            // concatenate using type order
            for(int i = 0; i < tokens.Length; i++) {
                switch(tokens[i].t_type) {
                    case TokenType.unknown_symbol: {
                        // attempt to find declaration of current symbol
                        if(Lexer.currentScope.contained_c.ContainsKey(tokens[i].data)) {
                            // if current scope contains class, its a class
                            tokens[i].t_type = TokenType.class_type;
                        } else if(Lexer.currentScope.contained_v.ContainsKey(tokens[i].data)) {
                            // same thing but its a variable
                            tokens[i].t_type = TokenType.variable_name;
                        } else if(Lexer.currentScope.contained_f.ContainsKey(tokens[i].data)) {
                            // same thing but its a function
                            tokens[i].t_type = TokenType.function_name;
                        } 
                    } continue; // since this has been unresolved, continue

                }
            }
            return new Token();
        }
        public void ThrowHere(Exception e) {
            Debug.Error($"At line {Line}: {e.Message}");
            Environment.Exit(0);
        }
        // converts a line of code into a Token (or Tokens)
        public static Token[] GetTokens(string line) {
            List<Token> ret = new List<Token>();
            int line_indent=0;
            for(int i = 0; i < line.Length; i++) {
                if(line[i] == '\t') line_indent++; // get trailing tabs
                else break;
            }
            // single tokens
            switch(line) {
                case "pass":
                    ret.Add(new Token() {
                        t_type = TokenType.keyword, // pass is a keyword
                        data = "pass", // make sure Execute() knows which token
                        Line = Token.CurrentLine, // make sure to log line this traces back to
                        indent = line_indent
                    });
                    return ret.ToArray();
                case "":
                    ret.Add(new Token() {
                        t_type = TokenType.empty_line, // basic empty line
                        Line = Token.CurrentLine, // log current line
                        indent = line_indent
                    });
                    return ret.ToArray();
                
            }
            string[] data = line.Split(' '); // get each word
            
            for(int i = 0; i < data.Length; i++) {
                
            }
            for(int i = 0; i < ret.Count; i++)
                ret[i].Line = CurrentLine; // make sure to set the line
            
            CurrentLine++; // increase the current line for accuracy
            return ret.ToArray();
        }
        // takes a single token and executes it
        public static void Execute(Token T) {
            string[] dat = T.data.Split(','); // split at commas
            switch(T.t_type) {
                case TokenType.empty_line: break; // this only exists so we have an accurate line count
                case TokenType.var_dec: {
                    // data="varname,vartype"
                    Class vclass = Lexer.currentScope.contained_c[dat[1]]; // get class by id
                    
                    // check for previous definitions of variable and commit error if it does
                    if(Lexer.currentScope.contained_v.ContainsKey(dat[0]))
                        T.ThrowHere(new Exception("Scope already contains a definition for variable " + dat[0])); // error
                    int sindex = memory_manager.Alloc(vclass.size); // allocate variable
                    // use data to declare new variable
                    Variable v = new Variable() {
                        name = dat[0],
                        retType = vclass,
                        stack_index = sindex
                    };
                    Lexer.currentScope.contained_v.Add(v.name, v); // add to this Scope
                    // now variable should be initialized
                } break; 
            }
        }
    }
}