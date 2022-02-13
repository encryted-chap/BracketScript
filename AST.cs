using System.Collections.Generic;

namespace BracketScript {
    public class AST_Parser {
        static List<string> token_abstract; // list of tokens in AST format
        static List<Token> Tokens = new List<Token>();
        static bool add;
        // AST format of tokens:
        static Dictionary<Token.TokenType, string> ast_format = new Dictionary<Token.TokenType, string>() {
            {Token.TokenType.str, "str"}, {Token.TokenType.unknown_symbol, "us"},
            {Token.TokenType.eq_operator, "op"}, {Token.TokenType.num, "us"}, 
            {Token.TokenType.keyword, "k"}
        };
        // add a token to this AST parser
        public static void GetAST(List<Token> t) {
            token_abstract = new List<string>();

            // parse each token for AST
            foreach(var token in t) 
                token_abstract.Add(Parse(token));
        }
        // parse a single string to AST format
        public static string Parse(Token t) {
            string ret = ast_format[t.t_type]; // get AST type
            Tokens.Add(t); // register this token

            ret += $":{Tokens.Count-1}"; // type:index_in_Tokens
            return ret;
        }
    }
}