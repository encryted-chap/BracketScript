using System.Collections.Generic;

namespace BracketScript {
    // a generic operation, the final token type before finally compiling to assembly
    public class Operation {
        public enum OpType {
            equation, var_dec,
            func_dec, class_dec,
            func_call, 

        }
        // gets a token and splits it into executeable operations
        //todo: make this do the thing it's supposed to
        public static Operation[] GetOperations(Token[] line) {
            List<Operation> ret = new List<Operation>(); // final tokens to return

            for(int i = 0; i < line.Length; i++) {
                
            }
            return ret.ToArray();
        }
    }
}