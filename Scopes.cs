using System.Collections.Generic;


namespace BracketScript
{
    using static global;

    public class Scope {
        
        public Scope() {

        }
    }
    public class Function {
        public List<string> instructions=new List<string>();
    }
    public class Class {
        public static Dictionary<string, Class> classes =
            new Dictionary<string, Class>();
        public string id; // the identifier used for this class
        public int size; // size (in bytes) to be allocated for a class instance
        public Dictionary<string, Function> functions; // get functions by name

        public Class(string name) {
            id = name;
            functions = new Dictionary<string, Function>();
        }
    }
    
}