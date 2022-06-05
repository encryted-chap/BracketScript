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
		public static int Line { get; private set; }

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
			Line++; // increase line count
			if(string.IsNullOrWhiteSpace(ln)) 
				return null; // skip null line 

			int ind = 0; // indentation for this line

			// preprocess line:
			string line = ln;
			line = line.Replace("    ", "\t"); // use tabs

			string tst = "\t";
			for(; line.StartsWith(tst); ++ind, tst += "\t"); // get indent using "FOR MAGIC!"

			List<string> strlit = new List<string>(); // list of string literals
			while(line.Contains('\"')) {
				// get string literals
				int begin = line.IndexOf('\"'); // get start index
				int end = line.IndexOf('\"', begin+1)+1; // get end index
				
				string lit = line.Substring(begin, end - begin); // get string
				strlit.Add(lit); // add string literal

				line = line.Remove(begin, end - begin).Insert(begin, "\n"); // remove all but one and replace
			}
			line = line.Replace('\n','\"');

			// get all plaintext tokens
			foreach(var t in match[token_type.OPERATOR]) {
				line = line.Replace(t, $" {t} "); // space so that its counted as token
			}
			line = line.Replace("  ", " ").Trim(); // remove double spacing

			string[] spl = line.Split(' '); // split string
			List<token_t> ret = new List<token_t>(); // the list of tokens to return

			Console.WriteLine($"parsing line {Line} ... ");
			Console.WriteLine($"===> indentation: {ind}");

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
					continue; // keep going to next token
				} 

				for(int j = (int)token_type.IDENTIFIER; j <= (int)token_type.KEYWORD; j++) {
					if(!match.ContainsKey((token_type)i)) 
						continue; // no need to check if uncheckable

					// test for match
					if(Array.IndexOf(match[(token_type)i], spl[i]) > -1) {
						// its a match
						ty = (token_type)i;
					}
				}

				if(object.ReferenceEquals(null, ty))
					ty = token_type.IDENTIFIER; // the default

				// add this token
				ret.Add(new token_t() {
					line = _ln,
					_t = ty,
					txt = spl[i],
					indent = ind,
				});

				// debug information
				Console.WriteLine($"===> token: {ret[ret.Count-1]._t},\"{ret[ret.Count-1].txt}\""); 
			}
			Console.WriteLine($"line {Line} parsed: {ret.Count} tokens\n");	

			return ret.ToArray();
		}

		// processes and resolves the scope of a function, class,
		// or namespace
		public static scope Process(List<token_t[]> file, scope g) {
			Console.WriteLine("Executing ... ");
			string PATH = g._asm; // get full path to scope
			for(int i = 0; i < file.Count; i++) {
				// use Test() function to find 
				// match for token[]
					
			}

			return null;	
		}
		// matches for Test()
		private static Dictionary<char,token_type> tmatch = 
			new Dictionary<char,token_type>() {
				{ 'o', token_type.OPERATOR },
				{ 'i', token_type.IDENTIFIER },
				{ 'k', token_type.KEYWORD },
				{ 'L', token_type.LITERAL },
			};

		// * = any tokens
		// . = any single token
		// ! = end search
		// o = operator
		// i = identifier
		// k = keyword
		// L = literal
		// ^ This is a custom match format for testing lines
		// of tokens, it tests for token types easily and in
		// a readable way, instead of a 2-line if statement
		private static bool Test(token_t[] t, string format) {
			// check for a match between format and t
			for(int i = 0; i < format.Length; i++) {
				switch(format[i]) {
					case '.': 
						continue; // no need to check with this
					
					case '*': {
						int j = i; // store j (string index)
					
						// check if this is last char
						if(i == format.Length - 1) {
							return true; // its a match
						}

						while(i < t.Length && t[i]._t != tmatch[format[j+1]]) { 
							i++; // increment until fulfilled
							if(i == t.Length) return false; // not a match if end without match
						} continue; // loop to next char
					} break;
					
					case '!': return true; // end search here

					default: {
						// normal token:
						if(t[i]._t != tmatch[format[i]]) 
							return false; // not a match!
					} break;
				}

			}
			return true;
		}
	}
}
