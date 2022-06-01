using System.Collections.Generic;

namespace bs {
	
	// block of compiler-space
	// memory.
	class vmem {
		// virtual memory stacks in compiler-space
		// memory (interpreted memory).
		private static readonly List<byte>[] stacks = 
			new List<byte>[3] {
				new List<byte>(),
				new List<byte>(),
				new List<byte>(),
			};

		
		public readonly int length; // size (in bytes)
		public readonly int track; // virtual track this is hosted on (0-2), 3 total
		public readonly ushort addrs; // address of this block on the track (max: 0xFFFF)

		public vmem(int size, int track_num, ushort address) {
			// assign values
			this.length = size;
			this.track = track_num;
			this.addrs = address;

			// ensure addresses exist
			while(stacks[track_num].Count < address + size) 
				stacks[track_num].Add(0x0); // pad null bytes
		}

		// allocates a virtual memory block
		public vmem(int size) {
			this.length = size; // assign size
			
			track = new Random().Next(0,2); // generate random track num
			addrs = (ushort)(stacks[track].Count); // get address

			// ensure allocated
			while(stacks[track].Count < addrs + size)
				stacks[track].Add(0x0); // pad null bytes
		}

		// clears all tracks of virtual memory
		public static void Clear() {
			for(int i = 0; i < stacks.Length; i++) {
				stacks[i] = new List<byte>();
			}
		}

		// writes a block of data to the memory 
		// block, with an optional offset
		public void Write(byte[] data, int offset=0) {
			int len = data.Length + offset; // calculate total len
			
			// if len over boundaries, prevent stack overflow
			if(len > length) { 
				// print error message then
				// kill the program
				Console.WriteLine(
					"FATAL: a stack overflow violation occurred:\n" + 
					$"DEBUG: trk={track},off=0x{offset.ToString("X")},adr=0x{addrs.ToString("X")}"
				);
				Environment.Exit(0); // exit program
			}

			stacks[track].InsertRange(addrs+offset, data); // write to track
		}

		// reads the block of data from 
		// the memory block, as a byte[]
		public byte[] Read() {
			return stacks[track].GetRange(addrs, length).ToArray();
		}
	}
}
