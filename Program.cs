using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Diagnostics;

// LoPy source
namespace BracketScript
{
    
    public static class global {
        public static void gen_id(out string str, int size=12) {
            str = string.Empty;
            for(int i = 0; i < size; i++) {
                str += "qwertyuiopasdfghjklzxcvbnmQWERTYUIOPASDFGHJKLZXCVBNM1234567890"[new System.Random().Next(61)];
            }
        }
        public static void asm(string code) {
            BracketScript.Program.ASM.Add(code);
        }
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
        public static List<string> ASM;
        static string outp, inp;
        static void Main(string[] args) {
            bool debug = false;
            // initialization code
            ASM = new List<string>() {
                "section .data",
                "",
                "section .text",
                "global _start",
                "_start:",
                "mov esp, ebp", // initialize stack
                
            };
            global.sec_dat("tempret: dd 0"); // used for passing the return value
            for(int i = 0; i < 10; i++) 
                global.sec_dat($"arg{i}: dd 0");
            outp = inp = string.Empty; // null reference proof :)
            for(int i = 0; i < args.Length; i++) {
                switch (args[i]) {
                    default:
                        inp = args[i];
                        Lexer.Lexify(args[i]);
                        break;
                    case "--output": case "-o":
                        outp = args[++i];
                        break;
                    case "--debug": 
                    case "-d":
                        debug = true;
                        break;
                }
            }
            memory_manager.Alloc(13);
            memory_manager.memory_map[0].Write(System.Text.Encoding.ASCII.GetBytes("hello, world!"));
            // format assembly
            for(int i = 0; i < ASM.Count; i++) {
                if(ASM[i].Contains(' ') && !ASM[i].StartsWith("section")) {
                    ASM[i] = ASM[i].Insert(ASM[i].IndexOf(' ')+1, "\t");
                    if(!ASM[i].EndsWith(':')) ASM[i] = "\t" + ASM[i];
                }
            }
            if(string.IsNullOrEmpty(outp))
                outp = inp + ".asm";
            ASM.Add("endloop: jmp endloop"); // just keep the program from "stopped working" error
            File.WriteAllLines(outp, ASM.ToArray());
            string[] names = Assembly.GetExecutingAssembly().GetManifestResourceNames();
            
            foreach(var n in names) {
                if(n.Contains("nasm")) {
                    byte[] rstream=new byte[Assembly.GetExecutingAssembly().GetManifestResourceStream(n).Length]; 
                    Assembly.GetExecutingAssembly().GetManifestResourceStream(n).Read(rstream);
                    File.WriteAllBytes("nasm.exe", rstream);
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
                    
                    File.Delete("nasm.exe");
                    if(!debug)
                        File.Delete(outp);
                    File.Delete(outp + ".obj");
                    System.Environment.Exit(0);
                }
            }
        }
    }
}
