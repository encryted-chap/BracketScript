using System.Collections.Generic;


namespace BracketScript
{
    using static global;

    
    public class Variable {
        public string name; 
        public Class retType; // the class type of this variable
        public int stack_index; // where this variable is allocated on the stack
        public bool isNull=true; // set if this variable is not allocated

        // point esp to this stack index
        public void LoadPtr() {
            _asm_.point(this.stack_index);
        }

        // allocates then stores this var into memory
        // if already allocated, writes to this address
        public void Alloc() {
            stack_index = memory_manager.Alloc(retType.size); // attempt to find variable memory
            
            int offset = 0; // byte offset for storing to memory
            foreach(var b in Class.GetBytes(this)) {
                b.LoadPtr(); // point esp to byte
                // write to block
                asm(new string[] {
                    "mov al, byte [esp]", // get byte value
                    $"mov byte [0x{(stack_index+offset).ToString("X")}], al" // store byte value and increment index
                });
                offset++; // increment offset accordingly
            }
            // now memory block should be filled with byte values :D
            isNull = false; // now it has been allocated
            memory_manager.memory_map[memory_manager.Find(stack_index)].Write(new byte[retType.size]); // write null to clear memory
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
        // allocates another instance completely identical to this variable
        public Variable Copy() {
            int s_index = memory_manager.Alloc(this.retType.size); // allocate variable
            Variable ret = new Variable(); // fill return variable with the exact same functions as this
            ret.stack_index = s_index;
            
            // now we must use assembly to copy the memory from one var to another
            asm($"\n; copy: {Lexer.currentScope.refid}::{this.name} -> address [ebp-0x{s_index.ToString("X")}]"); // comment, for debugging
            // copy [this.stack_index] -> [s_index]
            this.LoadPtr(); 
            asm("mov esi, esp"); // set source reg 
            ret.LoadPtr();
            asm("mov edi, esp"); // set dest reg
            
            // now transfer data:
            asm(new string[] {
                $"mov ecx, {this.retType.size}", // how many bytes to move
                "\tstd", // set direction flag (copy backwards)
                "rep movsb", // move data
                "\tcld" // now clear the direction flag, just for fun
            });
            return ret; // now we have a copied variable
        }
        // this == v (call t* equals(v) in asm)
        public void Equals(Variable v) {
        }
        // this = v (call t* assign(v) in asm)
        public void Assign(Variable v) {
            if(v.retType.id == this.retType.id) {
                memory_manager.Free(this.stack_index); // free var
                Variable clone = v.Copy(); // clone variable

                stack_index = clone.stack_index; // point this variable to v clone
                isNull = clone.isNull; // if the clone is null this should be too
            } else {
                asm("; not implemented exception");
            }
        }
        // this = v (call t* Mul(v) in asm)
        public void Multiply(Variable v) {
            
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
        public List<Token> toInstructions=new List<Token>(); // list of tokens to be converted to assembly
        public string fullname, name; // identifiers of function
        mblock ret_addrs; // the constant reserved space for this return address

        public Class return_type; // the return type of this function
        public Variable[] args; // arguments passed to this function
        public Scope FunctionScope;
        public Function(string name, Scope s, Variable[] args=null) {
            FunctionScope = s.inheritall(new Scope()); // create new inherited scope
            fullname = $"{name}_{s.refid}"; // assign fullname to avoid errors
            this.name = name;
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
            // now define the return address mem block
            int rs_index = memory_manager.Alloc(4); // get stack index
            int mmap_index = memory_manager.Find(rs_index); // get the index in memory_map
            ret_addrs = memory_manager.memory_map[mmap_index]; // now get the memory block
        }
        // defines the asm for this code
        public void DefineASM() {
            asm($"jmp end_{fullname}");
            asm($"{fullname}:");
            toInstructions = Lexer.Parse(FunctionScope, toInstructions); // parse token instructions
            asm(instructions.ToArray()); // allow for raw assembly

            // now manditory return protocol:
            _asm_.point(ret_addrs.index); // point to return address
            asm("\tret"); // and return

            asm($"end_{fullname}:");
        }
        // inserts the asm for calling this function
        public void Call() {
            if(!object.Equals(null, args)) {
                // loads arguments
                for(int i = 0; i < args.Length; i++) {
                    asm("mov eax, ebp"); // get stack val
                    asm($"sub eax, {args[i].stack_index}"); // point to address
                    asm($"mov [arg{i}], eax"); // store variable address
                }
            }
            _asm_.point(ret_addrs.index); // point for call
            asm($"call {fullname}"); // call function (will push return address to allocated memory)
            if(!object.Equals(null, args)) {
                // clear arguments
                for(int i = 0; i < args.Length; i++) 
                    asm($"mov dword [arg{i}], 0");
            }
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