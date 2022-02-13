using System.Collections.Generic;
using System;

namespace BracketScript {
    using static BracketScript.global;
    public class mblock {
        public int size, index; // the size of this memory block in bytes, and the stack index
       

        public bool free; // set if this memory block is not allocated

        public void WriteB(byte b, int offset, bool selected=false) {
            if(!selected)
                asm (new string[] {
                    "mov eax, ebp",
                    $"sub eax, {index}",
                    $"mov byte [eax-{offset}], 0x{b.ToString("X")}"
                });
            else asm($"mov byte [eax-{offset}], 0x{b.ToString("X")}");
        }
        public void WriteW(ushort word, int offset, bool selected=false) {
            if(!selected)
                asm(new string[] {
                    "mov eax, ebp",
                    $"sub eax, {index}",
                    $"mov word [eax-{offset}], word 0x{word.ToString("X")}"
                });
            else asm($"mov word [eax-{offset}], word 0x{word.ToString("X")}");
        }
        public void WriteD(uint dword, int offset, bool selected=false) {
            if(!selected)
                asm(new string[] {
                    "mov eax, ebp",
                    $"sub eax, {index}",
                    $"mov dword [eax-{offset}], dword 0x{dword.ToString("X")}"
                });
            else asm($"mov dword [eax-{offset}], dword 0x{dword.ToString("X")}");
        }
        public void Write(byte[] data) {
            bool sel = false;
            for(int i = 0; i < data.Length; ) {
                bool b = false;
                try {
                    byte[] da = new byte[] {
                        data[i], data[i+1], data[i+2], data[i+3]
                    }; Array.Reverse(da);
                    // write 4 bytes
                    WriteD(BitConverter.ToUInt32(da), i, sel);
                    i += 4;
                    b = true; 
                } catch {
                    try {
                        byte[] da = new byte[] {
                            data[i], data[i+1]
                        }; Array.Reverse(da);
                        // write 2 bytes
                        WriteW(BitConverter.ToUInt16(data, i), i, sel);
                        i += 2;
                        b = true;
                    } catch {b = false;}
                }
                if(!b)
                    WriteB(data[i], i++, sel);
                sel = true; // selected now
            }
        }
    }
    // the class for managing stack allocation 
    public static class memory_manager {
        
        public static List<mblock> memory_map=new List<mblock>(); // all the memory blocks that are created
        static int current_index; // the current index of the stack (ebp-index) to be used
        public static void Free(mblock m) {
            // look for identical block then free
            int i = Find(m.index);
            if(i != -1) memory_map[i].free = true;
        }
        public static void Free(int index) {
            // look for identical block then free
            for(int i = 0; i < memory_map.Count; i++) {
                if(memory_map[i].index == index) 
                    memory_map[i].free = true;
            }
        }
        // returns the index in memory_map that this mblock is located at
        public static int Find(int stack_index) {
            for(int i = 0; i < memory_map.Count; i++)
                if(memory_map[i].index == stack_index) return i;
            return -1;
        }
        // finds the first free memory block and returns the stack index of this mblock 
        public static int Alloc(int memsize) {
            for(int i = 0; i < memory_map.Count; i++) {
                // if memory would fit, return
                if(memory_map[i].free && memory_map[i].size <= memsize) {
                    // if there is leftover memory, fragment
                    if(memory_map[i].size - memsize != 0) {
                        memory_map.Add(
                            // create block from used memory
                            new mblock() {
                                free = true, 
                                size = memory_map[i].size - memsize, // leftover chunk of memory
                                index = memory_map[i].index + (memsize+1) 
                            }
                        );
                    }
                    memory_map[i].size = memsize; // update block size
                    memory_map[i].free = false; // this is going to be used
                    return memory_map[i].index;
                }
            }
            // allocate memory block
            mblock m = new mblock() {
                size = memsize,
                free = false,
                index = current_index
            };
            // prep index for the next one
            current_index += memsize + 1;
            memory_map.Add(m);
            return m.index;
            
        }
    }
}