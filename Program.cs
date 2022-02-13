using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;

// LoPy source
namespace BracketScript
{
    
    public static class global {
        // generate string id
        public static void gen_id(out string str, int size=12) {
            str = string.Empty; // reset str
            for(int i = 0; i < size; i++) {
                // choose random char to add to string
                str += "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890"[new System.Random().Next(61)];
            }
        }
        // add a single asm instruction
        public static void asm(string code) {
            BracketScript.Program.ASM.Add(code);
        }
        // adds a range of assembly instructions
        public static void asm(string[] code) {
            Program.ASM.AddRange(code);
        }
        // adds code to section .data
        public static void sec_dat(string code) {
            Program.ASM.Insert(1, code);
        }
    }
    class Program
    {
        public static List<string> ASM; // main assembly code, is written then compiled
        static string outp, inp; // used for file access (inp is source code, outp is output)
        static void Main(string[] args) {
           
            if(!Directory.Exists("lua")) Directory.CreateDirectory("lua");
            // get lua from embedded resources
            WriteResource("lexer", "lua/lexer.lua");

            bool debug = false;
            // initialization code
            ASM = new List<string>() {
                "section .data",
                "",
                "section .text",
                "global _start", // for the linker
                "_start:", // entry point
                "mov esp, ebp", // initialize stack
                
            };
            global.sec_dat("tempret: dd 0"); // used for passing the return value
            // used for passing arguments (automated for convenience)
            for(int i = 0; i < 10; i++) 
                global.sec_dat($"arg{i}: dd 0");
            outp = inp = string.Empty; // null reference proof :)
            // parse the command line arguments
            for(int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    default:
                        // if it's none of the following args, it means that its an input file
                        inp = args[i];
                        var tokens = Lexer.Lexify(args[i]);
                        break;
                    case "--output": case "-o":
                        // select output
                        outp = args[++i];
                        break;
                    case "--debug": 
                        // when debug mode is enabled, keep asm for debug purposes
                        debug = true;
                        break;
                }
            }
            // format assembly
            for(int i = 0; i < ASM.Count; i++) {
                // just make it pretty in a few ways
                if(ASM[i].Contains(' ') && !ASM[i].StartsWith("section")) {
                    ASM[i] = ASM[i].Insert(ASM[i].IndexOf(' ')+1, "\t");
                    if(!ASM[i].EndsWith(':')) ASM[i] = "\t" + ASM[i];
                }
            }
            // if output isn't set, set it manually
            if(string.IsNullOrEmpty(outp))
                outp = inp + ".asm";
            ASM.Add("endloop:"); // just keep the program from "stopped working" error
            ASM.Add("jmp endloop"); // infinite loop until the user ends the program manually
            File.WriteAllLines(outp, ASM.ToArray()); // write assembly code to asm file
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            
            WriteResource("nasm", "nasm.exe");
            
            // now invoke NASM.exe
            var proc = new Process();
            proc.StartInfo.CreateNoWindow = true;
            proc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            
            proc.StartInfo.FileName = "cmd.exe";
            proc.StartInfo.Arguments = $"/c nasm.exe {outp} -f win32 -o {outp}.obj";
            
            Process.Start(proc.StartInfo).WaitForExit(); 
            Debug.Success("obj gen: " + outp + ".obj");
            proc.StartInfo.Arguments = $"/c gcc {outp}.obj -o {outp.Replace(".brac.asm", "")}.exe -nostdlib";
            Process.Start(proc.StartInfo).WaitForExit();
            Debug.Success("exe gen: " + outp.Replace(".brac.asm", "") + ".exe");
            
            
            if(!debug)
                File.Delete(outp);
            File.Delete(outp + ".obj");
            
                
            File.Delete("nasm.exe");
            System.Environment.Exit(0);
        }
        public static void WriteResource(string resource_name, string path) {
            // get names and iterate
            foreach(var n in Assembly.GetExecutingAssembly().GetManifestResourceNames()) {
                if(n.Contains(resource_name)) {
                    byte[] rstream=new byte[Assembly.GetExecutingAssembly().GetManifestResourceStream(n).Length]; // allocate new buffer
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(n).Read(rstream); // now move bytes into buffer
                    File.WriteAllBytes(path, rstream); // now write buffer to path
                    return;
                }
            }
        } 
    }
}
