using BracketScript;
using System.IO;
using System;
using System.Collections.Generic;

namespace BracketScript {
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
            currentScope = new Scope("global"); // initialize global scope

            // define built-in class types
            Class b = new Class("byte", Scope.internal_scopes["global"]) // initialize byte class
            {
                size = 1,
            }; 
            Function b_new = new Function("new", b.classScope);
            for(int i = 0; i < processed.Count; i++) {
                ret.AddRange(Token.GetTokens(processed[i])); // add token to token collection
            }
            return ret;
        }
        
    }
    public class Token {
        public static int CurrentLine;
        public string _refid; 
        public string data;
        int Line; // the line that this token takes place
        int indent; // the indentation present on this token
        public enum TokenType {
            function_dec, var_dec,
            var_assign, keyword,
            class_dec, empty_line
        } TokenType t_type;
        
        public void ThrowHere(Exception e) {
            Debug.Error($"At line: {Line} \n>> {e.Message}");
            Environment.Exit(0);
        }
        public static Token[] GetTokens(string line) {
            List<Token> ret = new List<Token>();

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