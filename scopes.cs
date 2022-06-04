using bs;
using System.Collections.Generic;
using System;

namespace bs {

	// a collection of other scopes, which are all parsed as
	// classes, functions, variables, and namespaces.
	class scope {
		// different types of scopes
		// in the bracketscript language
		public enum scopetype {
			FUNCTION, // <-- point to a label
			VARIABLE, // <-- inherits _local scope from class
			CLASS, NAMESPACE, // <-- allow nested declarations
			SCOPE, // <-- pure scope functionality (uninherited), ex: global
		} 

		public scopetype _type = scopetype.SCOPE; // the type of scope of this object
		public static scope global_scope = new scope("global"); // the "root" scope

		public readonly scope parent; // the parent of this scope
		public readonly List<scope> children = new List<scope>(); // children of this scope

		public readonly string id; // identifier
		public readonly string _asm; // assembly identifier (name of label and such)

		public scope(string name, scope parent) {
			this.parent = parent;
			this.id = name; // assign identifier
			List<scope> path = new List<scope>() { this }; 
			
			// now get asm label name
			do {
				path.Add(path[path.Count-1].parent); // add parent
			} while(path[path.Count-1].id != "global");

			scope[] p = path.ToArray(); // get path as array
			Array.Reverse(p); // reverse array
			_asm = string.Empty; // avoid null reference

			// now get _asm value
			foreach(var pr in p) {
				if(pr != this)
					_asm += $"{pr.id}."; // append scope with dot
				else {
					_asm += pr.id; // add final scope (no dot)
					break; // end loop
				}
			}
		}
		// only to be used for the global scope,
		// not for anything else.
		public scope(string asm_name) { 
			_asm = asm_name;
		}

		// get assembly label name
		public string get_as() => _asm;

		// inherit all properties of scope
		// object s.
		public void Merge(scope s) {
			// inherit children from s
			children.AddRange(
				s.children.ToArray() // ensure it is a scope[]
			);
		}

		// finds a scope object from a "full.name.string",
		// returns the scope on success, returns global on
		// fail.
		public static scope Find(string fullname) {
			string[] path = fullname.Split('.'); // split at separators
			scope current = global_scope; //  recursive scope search

			for(int i = 0; i < path.Length; i++) {
				string sname = path[i]; // name of next scope
				scope o_scope = current; // get current for check later

				// look for next in path
				foreach(var v in current.children) {
					if(v.id == sname) { 
						current = v; 
						break;
					}
				}

				if(current == o_scope) {
					// no match found
					Console.WriteLine($"FATAL: couldn\'t find object or scope {fullname}");
					Environment.Exit(0); // exit
				} else continue; // iterate through 
			}
			return current; // return end object
		}

		// returns a scope, but only if it's of type "stype",
		// kills program on error
		public static scope Find(string fullname, scope.scopetype stype) {
			scope s = scope.Find(fullname); // get scope of any type

			// only return if it is of type "stype"
			if(s._type == stype) return s; 
			else {
				Console.WriteLine($"FATAL: Attempt to get type {stype} was not {stype}");
				Environment.Exit(0);
			}
			return null;
		}
	}

	// a class object
	struct _class {
		// variable instance scope and
		// static scope for this class
		private readonly scope _local; 
		public readonly scope _static;
		
		
		// creates an instance of this classes local scope,
		// essentially creating a new variable.
		public scope CreateInstance(string name, scope parent) {
			scope i = new scope(name, parent); // get instance
			i._type = scope.scopetype.VARIABLE; // set scope type
			
			i.Merge(_local); // inherit local scope of class
			return i; // return variable scope
		}
	};
	class bs_class : scope {
		private readonly scope local;
		public bool def_virt; // store instances in virtual memory by default?

		// normal inherited constructors
		extern public bs_class(string asm_name);	
		extern public bs_class(string name, scope parent);

		public bs_var CreateInstance(string name, scope parent) {
			// creates a new instance of this class object,
			// uses the local scope to create a predefined clone.
			
			bs_var ret = new bs_var(name, parent);
			ret.isvirt = def_virt;

			return null;
		}
	}
	class bs_var : scope {
		public bool isvirt; // stored in vmem?
		private vblock mem; // memory block

		// normal inherited constructors
		extern public bs_var(string asm_name);
		extern public bs_var(string name, scope parent);
	}
}
