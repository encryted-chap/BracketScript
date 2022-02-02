using System.Collections.Generic;
using System;

namespace BracketScript {
    using static BracketScript.global;
    public class mblock {
        public int size, index;
        public static Dictionary<int, mblock> blocks=
            new Dictionary<int, mblock>();

        public bool free;

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
    public static class memory_manager {
        
        public static List<mblock> memory_map=new List<mblock>();
        static int current_index;
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
        public static int Find(int index) {
            for(int i = 0; i < memory_map.Count; i++)
                if(memory_map[i].index == index) return i;
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
    public static class _asm_ {
        static int malloc_index;
        
        public enum Regs {
            eax, ebx, ecx, edx,
            ax, bx, cx, dx,
            al, ah, bl, bh, 
            cl, ch, dl, dh,  
            esp, ebp, esi, edi,
            sp, bp, si, di
        }
        public static string RegName(Regs r) {
            return System.Enum.GetName(typeof(Regs), r);
        }
        // clears eax-edx
        public static void ClearRegs() {
            for(char c = 'a'; c != 'e'; c++) {
                string cs = c.ToString(); // just in case lol
                asm($"xor e{cs}x, e{cs}x");
            }
        }
        public static void Memcpy(int dest_addr, byte[] src, int count) {
            if(count==0) return;

            asm($"mov ebx, dword {dest_addr.ToString("X")}"); // mov ebx, dword 0x[destaddr]
            for(int i = 0; i < count; i++) {
                asm($"mov byte [ebx+{i}], {src[i]}");
            }
        }
        // fills with 0 from startaddress to endaddress
        public static void ClearMem(int startaddress, int endaddress) {
            int size = startaddress - endaddress;
            byte[] blankarr = new byte[size+1];
            Memcpy(startaddress, blankarr, size);
        }
        
    }
}