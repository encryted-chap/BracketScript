using System.Collections.Generic;


namespace BracketScript
{
    using static global;

    
    public class Variable {
        public string name;
        public Class retType;
        public int stack_index;
        public bool isNull=true;

        // loads a pointer to this variable to an assembly register
        public void LoadPtr(_asm_.Regs register = _asm_.Regs.eax) {
            asm(new string[] {
                "mov eax, ebp",
                $"sub eax, {stack_index}", // point to addr
                $"mov {_asm_.RegName(register)}, eax" // load to reg
            });
        }

        // allocates then stores this var into memory
        // if already allocated, writes to this address
        public void Alloc() {
            int index = memory_manager.Find(stack_index); // attempt to find variable memory
            if(index == -1) {
                // allocate new memory block
                index = memory_manager.Alloc(retType.size); 
            } else {
                // get existing memory block
                index = memory_manager.memory_map[index].index;
            }
            int offset = 0; // byte offset for storing to memory
            foreach(var b in Class.GetBytes(this)) {
                b.LoadPtr(_asm_.Regs.eax); // point eax to byte
                // write to block
                asm(new string[] {
                    "mov al, byte [eax]", // get byte value
                    $"mov byte [0x{(index+offset).ToString("X")}], al" // store byte value and increment index
                });
                offset++; // increment offset accordingly
            }
            // now memory block should be filled with byte values :D
            isNull = false; // now it has been allocated
        }
        // deallocates this variable (garbage collection)
        public void Free() {
            if(!isNull) {
                // if not null then freeing is valid
                mblock m =  memory_manager.memory_map[memory_manager.Find(stack_index)]; // get this mblock
                if(!m.free) memory_manager.Free(m); // if not already free, free
                isNull = true; // make sure its declared as null
            }
        }
        // this == v (call t* equals(v) in asm)
        public void Equals(Variable v) {

        }
        // this = v (call t* assign(v) in asm)
        public void Assign(Variable v) {

        }
        // this = v (call t* Mul(v) in asm)
        public void Multiply(Variable v) {
            
        }
        public Variable(Class type, string name, Variable[] args = new Variable[0]) {
            // call type.new()
            if(type.classScope.contained_f.ContainsKey("new")) {
                Function f = type.classScope.contained_f["new"];
                if(args.Length[0] == 0) f.Call(); // call with no args
            }
            
        }
    }
    public class Scope {
        // things accessible within this scope
        public Dictionary<string, Class> contained_c;
        public Dictionary<string, Function> contained_f;
        public Dictionary<string, Variable> contained_v;

        // reference id of this scope (to find in internal_scopes)
        public string refid;
        public int Indentation; // the indentation of this Scope (amount of tabs)

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
            if(contained_c.Count != 0)
                s = inheritc(s); // inherit classes,
            if(contained_f.Count != 0)
                s = inheritf(s); // functions,
            if(contained_v.Count != 0)
                s = inheritv(s); // and variables
            s.Indentation = this.Indentation+1; // make sure to insert proper indentation 
            return s; // assimilated scope :P
        }
        Scope inheritv(Scope s) {
            foreach(var v in this.contained_v)
                s.contained_v.Add(v.Key, v.Value);
            
            return s;
        }
        Scope inheritc(Scope s) {
            foreach(var c in this.contained_c)
                s.contained_c.Add(c.Key, c.Value);
            return s;
        }
        Scope inheritf(Scope s) {
            foreach(var f in this.contained_f)
                s.contained_f.Add(f.Key, f.Value);
            return s;
        }
    }
    public class Function {
        public List<string> instructions=new List<string>(); // the assembly code of this Function
        public string fullname, name; // identifiers of function

        public Class return_type; // the return type of this function
        public Variable[] args; // arguments passed to this function
        public Scope FunctionScope;
        public Function(string name, Scope s, Variable[] args=null) {
            FunctionScope = s.inheritall(new Scope()); // create new inherited scope
            fullname = $"{name}_{s.refid}"; // assign fullname to avoid errors
            if(!object.Equals(args, null)) {
                // check if args are already defined in current scope
                foreach(Variable a in args) {
                    // if contains variable,
                    if(FunctionScope.contained_v.ContainsKey(a.name)) {
                        // should always be caught by ThrowHere()
                        throw new System.Exception("Scope already contains a definition for variable " + a.name); // throw if Var exists
                    }
                }
                
            }
        }
        // defines the asm for this code
        public void DefineASM() {

        }
        // inserts the asm for calling this function
        public void Call() {
            // loads arguments
            for(int i = 0; i < args.Length; i++) {
                asm("mov eax, ebp"); // get stack val
                asm($"sub eax, {args[i].stack_index}"); // point to address
                asm("mov [arg{i}], eax"); // store variable address
            }
            // set up stack such that address doesnt overwrite allocated memory
            int ret = memory_manager.Alloc(4); // allocate 4 bytes for return address
            asm("mov eax, ebp");
            asm($"sub eax, {ret}"); // point eax to allocated memory
            asm($"call {fullname}"); // call function (will push return address to allocated memory)
            // clear arguments
            for(int i = 0; i < args.Length; i++) 
                asm($"mov dword [arg{i}], 0");
            memory_manager.Free(ret); // free address memory
        }
        public void Call(Variable[] args=new Variable[0]) {
            // if all arguments passed, call success
            if(this.args.Length == args.Length) {
                for(int i = 0; i < args.Length; i++) {

                }
            } 
            else throw new System.Exception (
                $"Function call Call({name}) passed invalid arguments"
            );
        }
    }
    public class Class {
        public Scope classScope; // the scope containing everything inside this class
        public string id; // the identifier used for this class
        public int size; // size (in bytes) to be allocated for a class instance

        public Class(string name, Scope s) {
            id = name;
            
            classScope = new Scope(); // initialize new scope
            classScope = classScope.inheritall(s); // inherit higher scope
            s.contained_c.Add(id, this); // place in scope
            
        }
        
        // gets the bytes from a single variable
        public static Variable[] GetBytes(Variable toGet) {
            List<Variable> ret = new List<Variable>();
            // recursive funny business
            foreach(var v in toGet.retType.classScope.contained_v) {
                // search variables for byte classes recursively
                if(v.Value.retType.id == "byte") {
                    ret.Add(v.Value); // add byte class
                } else ret.AddRange(GetBytes(v.Value)); // otherwise find bytes in THIS variable
            }
            return ret.ToArray(); // finally return the bytes
        }
    }
    
}