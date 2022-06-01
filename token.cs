using bs;
using System.IO;
using System.Collections.Generic;

namespace bs {
	struct token_t {
		// possible types of
		// tokens.
		public enum token_type {
			IDENTIFIER, OPERATOR,
			LITERAL, KEYWORD,
		} token_type _t; // the type of this token
		
		// the current line number that
		// the program is executing
		public static int Line { get { return _line; } private set { _line = value; } }
		private static int _line;

		public int line; // line this token is on

		// plaintext matches for certain token types,
		// allows the token class to parse tokens based on
		// these string[]'s.
		static Dictionary<token_type,string[]> match = 
			new Dictionary<token_type,string[]>() {
				{ 
					token_type.OPERATOR, 
					new string[] { 
						"=","+","-","*","/",
						"^","&",">","<","!",
						"%","(",")",":",";",
						"and", "or", "not",
					}
				},
				{
					token_type.KEYWORD,
					new string[] {
						"export", "virtual", 
						"static", "if", "while",
						"for", "do", "import",
						"switch", "case", 
					}
				},
			};

		public string txt; // plaintext value of this token 

		public static token_t[] GetTokens(string ln) {
			Line++;
			if(string.IsNullOrWhiteSpace(ln)) return null; 

			// preprocess line:
			string line = ln;

			// get all plaintext tokens
			foreach(var t in match[token_type.OPERATOR]) {
				line = line.Replace(t, $" {t} "); // 
			}
			line = line.Replace("  ", " "); // remove double spacing

			string[] spl = line.Split(' '); // split string
			
			// get match profiles
			List<string> opm = new List<string>();
			opm.AddRange(match[token_type.OPERATOR]);

			List<string> kym = new List<string>();
			kym.AddRange(match[token_type.KEYWORD]);

			List<token_t> ret = new List<token_t>(); // the list of tokens to return

			// now iterate through each word to generate a token_t,
			// return the output of each word as a list to end function.
			for(int i = 0; i < spl.Length; i++) {
				int _ln = token_t.Line;
				token_type ty = token_type.IDENTIFIER; // default to identifier

				// check for token type:
				if(opm.Contains(spl[i])) {
					ty = token_type.OPERATOR; // is an operator
				} else if(kym.Contains(spl[i])) {
					ty = token_type.KEYWORD;
				}

				// add this token
				ret.Add(new token_t() {
					line = _ln,
					_t = ty,
					txt = spl[i],
				});
			}
			Console.WriteLine($"line {Line} parsed: {ret.Count} tokens");	
			return ret.ToArray();
		}
	};
}
