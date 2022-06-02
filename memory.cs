using System.Collections.Generic;

namespace bs {

	// ambiguous memory block
	// (both virtual and ram)
	abstract class mblock {
		public readonly bool virt; // virtual memory
		public readonly int length; // size (in bytes) of this mblock

		public bool allocated=false; // set if this block is allocated

		public mblock(int size, bool is_virtual=true) {
			// set readonly values
			length = size;
			virt = is_virtual; 

			allocated = false; // start as non-allocated
		}

		public mblock() { return; }
		
		// functions to be set by inheriting classes
		
		//public abstract void Copy(mblock dst); // copies data to another mblock
		//public abstract mblock Clone(); // returns a copied version of this mblock

		public abstract void Alloc(); // allocate this mblock
		public abstract void Free(); // deallocate this mblock

		//public abstract void Write(byte[] data, int offset); // write data to mblock+offset
		//public abstract byte[] Read(int n, int offset); // read n bytes of data from mblock+offset
	}
	
	// a stack-stored memory block,
	// in actual memory instead of in
	// the compiler.
	class sblock : mblock {
		public int stack_index;
		private static List<sblock> stack = new List<sblock>(); // map of the stack

		public override void Alloc() {
			// asm($"sub esp, {length}");
			stack_index = 0;

			for(int i = 0; i < stack.Count; i++) {
				stack[i].stack_index += length; // shift to correct
				
			}
			stack.Add(this); // register in stack
		}

		public override void Free() {
			
		}
	}

	// a virtual memory block
	class vblock : mblock {
		// virtual stacks to store raw byte memory
		private static readonly List<List<byte>> vstacks = new List<List<byte>>();
		private static readonly List<vblock> blocks = new List<vblock>();

		private int stack, index; // mblock = vstacks[stack][index]
		private int block_index;

		public override void Alloc() {
			if(!allocated) {
				stack = new Random().Next(64); // 64 stacks

				// ensure stack exists by allocating it
				while(vstacks.Count < stack)
					vstacks.Add(new List<byte>());

				index = vstacks[stack].Count; // last + 1
				while(vstacks[stack].Count < index + length)
					vstacks[stack].Add(0x0); // pad to size

				block_index = blocks.Count;
				blocks.Add(this); // add block to block list
			}
		}
		public override void Free() {
			if(!allocated) return; // no need to free if unallocated
			
			// bump back blocks
			for(int i = 0; i < blocks.Count; i++) {
				var v = blocks[i];
				if(v.stack == this.stack && v.block_index > this.block_index) { 
					v.index -= this.length; // bump down	
				}
			}
				
			for(int i = 0; i < length; i++) {
				vstacks[stack].RemoveAt(index);
			}
		}
	}

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
