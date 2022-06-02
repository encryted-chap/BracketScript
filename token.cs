using bs;
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
		public int indent; // indentation of this token

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
						",","and","not","@", 
						"or","\"", "\'", 
					}
				},
				{
					token_type.KEYWORD,
					new string[] {
						"export", "virtual", 
						"static", "if", "while",
						"for", "do", "import",
						"switch", "case", "extern",
						"def", "return", "pass", 
						"class", "namespace",
					}
				},
			};

		public string txt; // plaintext value of this token 

		public static token_t[] GetTokens(string ln) {
			int ind = 0; // indentation for this line

			Line++;
			if(string.IsNullOrWhiteSpace(ln)) return null; 

			// preprocess line:
			string line = ln;
			line = line.Replace("    ", "\t"); // use tabs

			string tst = "\t";
			for(; line.StartsWith(tst); ++ind, tst += "\t"); // get indent using "FOR MAGIC!"
			line = line.Trim(); // trim off tabs (and trailing whitespace)

			List<string> strlit = new List<string>(); // list of string literals
			while(line.Contains('\"')) {
				// get string literals
				int begin = line.IndexOf('\"'); // get start index
				int end = line.IndexOf('\"', begin+1)+1; // get end index
				
				string lit = line.Substring(begin, end - begin); // get string
				strlit.Add(lit); // add string literal

				line = line.Remove(begin, end - begin); // remove all but one
				line = line.Insert(begin, "\n"); // replace with newline
			}

			line = line.Replace('\n','\"');

			// get all plaintext tokens
			foreach(var t in match[token_type.OPERATOR]) {
				line = line.Replace(t, $" {t} "); // space so that its counted as token
			}
			line = line.Replace("  ", " "); // remove double spacing
			line = line.Trim(); // trim once more

			string[] spl = line.Split(' '); // split string
			
			// get match profiles
			List<string> opm = new List<string>();
			opm.AddRange(match[token_type.OPERATOR]);

			List<string> kym = new List<string>();
			kym.AddRange(match[token_type.KEYWORD]);

			List<token_t> ret = new List<token_t>(); // the list of tokens to return

			Console.WriteLine($"parsing line {Line} ... ");
			Console.WriteLine($"===> indentation: {ind}");
			int str_index = 0; // index of the current literal

			// now iterate through each word to generate a token_t,
			// return the output of each word as a list to end function.
			for(int i = 0; i < spl.Length; i++) {
				if(string.IsNullOrEmpty(spl[i])) continue; // reiterate on null token
				if(spl[i] == "#") break; // end on comment

				int _ln = token_t.Line;
				token_type ty = token_type.IDENTIFIER; // default to identifier

				if(spl[i] == "\"") {
					// string literal
					spl[i] = strlit[0]; // get string literal 0
					strlit.RemoveAt(0); // pop 0 off
					ty = token_type.LITERAL; // assign literal type
				} else if(opm.Contains(spl[i])) {
					ty = token_type.OPERATOR; // is an operator
				} else if(kym.Contains(spl[i])) {
					ty = token_type.KEYWORD; // is a keyword
				} else {
					// is an identifier
					// try to check for literals
					try {
						System.Convert.ToInt32(spl[i]);
						ty = token_type.LITERAL;
					} catch { }
				}

				// add this token
				ret.Add(new token_t() {
					line = _ln,
					_t = ty,
					txt = spl[i],
					indent = ind,
				});
				Console.WriteLine($"===> token: {ret[ret.Count-1]._t},\"{ret[ret.Count-1].txt}\""); 
			}
			Console.WriteLine($"line {Line} parsed: {ret.Count} tokens\n");	

			return ret.ToArray();
		}

		// processes a file's tokens, this is used to process
		// import files, and such.
		public static scope Process(List<token_t[]> file, scope g) {
			scope.global_scope = g;

			vmem.Clear(); // clear all virtual memory for processing
			
			// iterate through every line of tokens
			for(int i = 0; i < file.Count; i++) {
				token_t[] ln = file[i]; // for readability
		
			}
			return g; // return the global scope, now updated
		}
		// matches for Test()
		private static Dictionary<char,token_type> tmatch = 
			new Dictionary<char,token_type>() {
				{ 'o', token_type.OPERATOR },
				{ 'i', token_type.IDENTIFIER },
				{ 'k', token_type.KEYWORD },
				{ 'L', token_type.LITERAL },
			};

		// * = any token type
		// ! = end search
		// o = operator
		// i = identifier
		// k = keyword
		// L = literal
		// ^ This is a custom match format for testing lines
		// of tokens, it tests for token types easily and in
		// a readable way, instead of a 2-line if statement
		private static bool Test(token_t[] t, string format) {
			// this would mean they are inequal
			if(t.Length != format.Length) 
				return false; 
			// check for a match between format and t
			for(int i = 0; i < format.Length; i++) {
				if(format[i] == '*') continue; // no need to check with this
				else if(format[i] == '!') break; // end search here
				else if(t[i]._t != tmatch[format[i]]) return false; // inequal, end
			}
			return true;
		}
	};
}
