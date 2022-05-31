using bs;

namespace bs {
	struct program_t {
		public readonly string ifile = string.Empty; // input file
		public readonly string ofile = string.Empty; // output file

		public readonly scope _program; // parsed scope of program
	};
}
