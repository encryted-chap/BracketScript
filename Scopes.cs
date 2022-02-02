using System.Collections.Generic;


namespace BracketScript
{
    using static global;

    
    public class Variable {
        public string name;
    }
    public class Scope {
        // things accessible within this scope
        public Dictionary<string, Class> contained_c;
        public Dictionary<string, Function> contained_f;
        public Dictionary<string, Variable> contained_v;

        public string refid;

        public Dictionary<string, Scope> internal_scopes=new Dictionary<string, Scope>();
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
        Scope CreateUnder() {
            Scope ns = new Scope();
            ns = inheritall(ns);
            return ns;
        }
        Scope inheritall(Scope s) {
            s = inheritc(s);
            s = inheritf(s);
            s = inheritv(s);
            return s;
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