using System.Collections.Generic;
using System;

namespace BracketScript {
    using static global;
    
    public static class _asm_ {
        static int malloc_index;
        
        // enumerated registers for easy access
        public enum Regs {
            eax, ebx, ecx, edx,
            ax, bx, cx, dx,
            al, ah, bl, bh, 
            cl, ch, dl, dh,  
            esp, ebp, esi, edi,
            sp, bp, si, di
        }
        // gets the name of a Enum Regs value
        public static string RegName(Regs r) {
            return System.Enum.GetName(typeof(Regs), r);
        }
        // clears eax-edx
        public static void ClearRegs() {
            for(char c = 'a'; c != 'e'; c++) {
                string cs = c.ToString(); // just in case lol
                asm($"xor e{cs}x, e{cs}x"); // clear current
            }
        }
    }
}