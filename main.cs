using bs;
using System.IO;
using System.Threading;

namespace bs {
	class Program {
		// for compiler speed measurement
		static int ms=0;
		static bool count=true;

		// -o [ FILE ]  	= output file
		// -h				= help message
		static void Main(string[] args) {
			new Thread(() => {
				while(count) {
					ms++;
					Thread.Sleep(1);
				}
			}).Start();

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

			core.Parse(ifile, ofile);			
			
			count = false; // end ms counting
			Console.WriteLine($"done in {ms}ms ({((float)ms) / ((float)1000)}s)");
		}
	}	
}
