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
		
		public static program_t Parse(string _ifile, string? _ofile=null) {
			scope _pscope = new scope("global"); // create scope for new file
			string[] lines = File.ReadAllLines(_ifile); // get lines
		
			List<token_t[]> t_lines = new List<token_t[]>(); // token lines

			foreach(var ln in lines) {
				var t = token_t.GetTokens(ln); // get tokens
				
				if(!object.ReferenceEquals(null, t)) {
					// not null
					t_lines.Add(t);
				} else continue;
			}
			
			_pscope = token_t.Process(t_lines, _pscope); // process file
			return new program_t(_pscope, _ifile, _ofile);
		}
	}
}
