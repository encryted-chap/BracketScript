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
			FUNCTION, VARIABLE,
			CLASS, NAMESPACE,
			SCOPE,
		} 

		public scopetype _type = scopetype.SCOPE; // the type of scope of this object
		public static scope global_scope;

		public readonly scope parent; // the parent of this scope
		public List<scope> children = new List<scope>(); // children of this scope
		public string id; // identifier

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
					s.children.ToArray()
			);
		}
	}

	class bsclass : scope {
		public extern bsclass(string name, scope parent);
	}
}
