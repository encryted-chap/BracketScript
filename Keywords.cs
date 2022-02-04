using System.Collections.Generic;

namespace BracketScript {
    public class Keyword {
        public string plaintext=string.Empty; // the plaintext code for this token
        public List<string> ASM=new List<string>(); // executed when keyword is used

        public void Execute(string Line) {
            // break data from code line into data C# can use
            // return tokens to execute said instructions
        }
    }
}