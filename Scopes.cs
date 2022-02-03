using System.Collections.Generic;


namespace BracketScript
{
    using static global;

    
    public class Variable {
        public string name;
        public Class retType;
        public int stack_index;
    }
    public class Scope {
        // things accessible within this scope
        public Dictionary<string, Class> contained_c;
        public Dictionary<string, Function> contained_f;
        public Dictionary<string, Variable> contained_v;

        // reference id of this scope (to find in internal_scopes)
        public string refid;

        public static Dictionary<string, Scope> internal_scopes=new Dictionary<string, Scope>();
        public Scope(string refid="") {
            // begone, null refs >:(
            contained_c = new Dictionary<string, Class>();
            contained_f = new Dictionary<string, Function>();
            contained_v = new Dictionary<string, Variable>();

            if(refid == "") {
                // generate new id
                gen_id(out this.refid);
            } else this.refid = refid; // assign refid
            internal_scopes.Add(this.refid, this); // pass this to scopes
        }
        // create a new scope contained in this one
        public Scope CreateUnder() {
            Scope ns = new Scope(); // register new scope
            internal_scopes[ns.refid] = inheritall(ns); // inherit values and save
            return ns;
        }
        // inherits all the properties from another scope (InheritFrom)
        public static Scope Inherit(Scope toInherit, Scope InheritFrom) {
            return InheritFrom.inheritall(toInherit);
        }
        public Scope inheritall(Scope s) {
            s = inheritc(s); // inherit classes,
            s = inheritf(s); // functions,
            s = inheritv(s); // and variables
            return s; // assimilated scope :P
        }
        Scope inheritv(Scope s) {
            var en = contained_v.GetEnumerator();
            for(int i = 0; i < contained_v.Count; i++) {
                s.contained_v.Add(en.Current.Key, en.Current.Value);
                en.MoveNext();
            }
            return s;
        }
        Scope inheritc(Scope s) {
            var en = contained_c.GetEnumerator();
            for(int i = 0; i < contained_c.Count; i++) {
                s.contained_c.Add(en.Current.Key, en.Current.Value);
                en.MoveNext();
            }
            return s;
        }
        Scope inheritf(Scope s) {
            var en = contained_f.GetEnumerator();
            for(int i = 0; i < contained_f.Count; i++) {
                s.contained_f.Add(en.Current.Key, en.Current.Value);
                en.MoveNext();
            }
            return s;
        }
    }
    public class Function {
        public List<string> instructions=new List<string>(); // the assembly code of this Function
        public string fullname, name; // identifiers of function

        public Class return_type; // the return type of this function
        public Function(string name, Scope s) {
            fullname = $"{name}_{s.refid}"; 
        }
    }
    public class Class {
        Scope classScope;
        public string id; // the identifier used for this class
        public int size; // size (in bytes) to be allocated for a class instance

        public Class(string name, Scope s) {
            id = name;
            
            classScope = new Scope(); // initialize new scope
            classScope = classScope.inheritall(s); // inherit higher scope
            s.contained_c.Add(id, this); // place in scope
            
        }
    }
    
}