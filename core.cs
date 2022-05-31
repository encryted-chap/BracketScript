using bs;

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
	}
}
