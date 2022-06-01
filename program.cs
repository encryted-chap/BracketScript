using bs;

namespace bs {
	struct program_t {
		public readonly string ifile = string.Empty; // input file
		public readonly string? ofile = string.Empty; // output file (optional)

		public readonly scope _program; // parsed scope of program
		public program_t(scope p, string ifile, string? ofile=null) {
			_program = p;
			this.ifile = ifile;
			this.ofile = ofile;
		}
	};
}
