using System;

namespace BracketScript {
    public static class Debug {
        public static bool TryOp (Action a) {
            try {a();}
            catch (Exception e) {
                Console.Write("[ ");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("ERROR! ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"] {e.Message}\n");
                return false;
            }
            return true;
        }
        public static void WriteCol(string msg, ConsoleColor c) {
            Console.ForegroundColor = c;
            Console.Write(msg);
            Console.ResetColor();
        }
        public static void Error(string msg) {
            Console.Write("[ ");
            WriteCol("ERROR ", ConsoleColor.Red);
            Console.Write($"] {msg}\n");
        }
        public static void Success(string msg) {
            Console.Write("[ ");
            WriteCol("SUCCESS ", ConsoleColor.Green);
            Console.Write($"] {msg}\n");
        }
        public static void Message(string msg) {
            Console.Write("[ ");
            WriteCol("INFO ", ConsoleColor.Magenta);
            Console.Write($" ] {msg}\n");
        }
        
    }
}