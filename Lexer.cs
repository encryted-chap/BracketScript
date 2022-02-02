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
        public static List<Token> Lexify (string input) {
            // get processor output
            List<string> processed = PreProcessor.Process(
                File.ReadAllLines(input)
            );
            List<Token> ret = new List<Token>();

            for(int i = 0; i < processed.Count; i++) {

            }
            return new List<Token>();
        }
        
    }
    public class Token {
        public string _refid; 
        public string data;
        int Line; // the line that this token takes place
        int indent; // the indentation present on this token
        public enum TokenType {
            function_dec, var_dec,
            var_assign, keyword,
        } TokenType t_type;
        
        public void ThrowHere(Exception e) {
            Debug.Error($"At line: {Line}; \n{e.Message}");
            Environment.Exit(0);
        }
        public static Token[] GetTokens(string line) {
            List<Token> ret = new List<Token>();

            return ret.ToArray();
        }
        public static void AddASM(Token T) {
            string[] dat = T.data.Split(','); // split at commas
            switch(T.t_type) {
                case TokenType.var_dec: {
                    // data="varname,vartype
                    Class varclass = Class.classes[dat[1]]; // get class type
                    int stackindex = memory_manager.Alloc(varclass.size); // ensure to allocate space for variable
                } break;
            }
        }
    }
}