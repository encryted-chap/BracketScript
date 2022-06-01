using bs;
using System.Collections.Generic;

/*
 * this file is dedicated to abstracting the
 * usages of the classes contained in this program.
 * This allows for much faster development of higher
 * level features.
*/ 

namespace bs {
		
	// a class designed to
	// abstract usage of memory,
	// scopes, etc etc
	static class core {
		public static void ExecuteFunction(string fullname, scope[] args) {
			int argc = args.Length; // number of arguments
			scope f = scope.Find(fullname); // look for scope

			// check that it is actually a function
			if(f._type != scope.scopetype.FUNCTION) {
				Console.WriteLine($"FATAL: called {fullname}, but it wasn\'t a function");
			}
		}
		
		public static program_t Parse(string ifile, string ofile) {
			scope _pscope = new scope("global"); // create scope for new file
			string[] lines = File.ReadAllLines(ifile); // get lines
		
			List<token_t[]> t_lines = new List<token_t[]>(); // token lines

			foreach(var ln in lines) {
				t_lines.Add( // add to lines
					token_t.GetTokens(ln) // get tokens from line
				);
			}

			return new program_t();
		}
	}
}
