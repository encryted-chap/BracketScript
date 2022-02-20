using System.Collections.Generic;
using System;

namespace BracketScript {
    using static global;
    
    public static class _asm_ {
        static int malloc_index;
        public static int esp_current=0; // the index esp is currently pointing at
        
        // enumerated registers for easy access
        public enum Regs {
            eax, ebx, ecx, edx,
            ax, bx, cx, dx,
            al, ah, bl, bh, 
            cl, ch, dl, dh,  
            esp, ebp, esi, edi,
            sp, bp, si, di
        }
        // points esp to a new location
        public static void point(int stack_index) {
            int operation = -(esp_current - stack_index);
            if(operation == 0) return; // already pointing here

            if(operation < 0) asm($"add esp, {-operation}"); // point in the oppisite direction
            else asm($"sub esp, {operation}"); // else move forward on stack

            esp_current = stack_index;
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
            asm("mov esp, ebp ; restore stack");
            asm("");
            esp_current = 0; // reset the stack
        }
    }
}