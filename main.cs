using bs;
using System.IO;

namespace bs {
	class Program {
		// -o [ FILE ]  	= output file
		// -h				= help message
		static void Main(string[] args) {
			string ofile = string.Empty;
			string ifile = string.Empty;

			for(int i = 0; i < args.Length; i++) {
				switch(args[i]) {
					case "-o": 
						ofile = args[++i];
						break;
					default:
						ifile = args[i];
						break;

				}
			}
			if(ofile == string.Empty)
				ofile = ifile + ".s";

			
		}
	}	
}
