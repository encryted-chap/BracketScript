enum {
	CLASS, // static class
	VARIABLE, // instance of class
	FUNCTION, // local function
};

struct _scopeptr {
	uint64_t id; // scopes id
	char *refid; // the plaintext id
	
	void *_typeptr; // the attached object
	uint8_t type_n; // type of above object
	
	// contained objects
	void 
		*contained_f, // taken as a (_funcptr*)
		*contained_c, // taken as a (_classptr*)
		*contained_v; // taken as a (_varptr*)

	// parent _scopeptr*
	struct _scopeptr *parent;
} *_global;

struct _classptr {
	struct _scopeptr *_scope; // scope of this class
};

struct _varptr {
	struct _scopeptr *_scope;
	struct _classptr *_class;
};

struct _funcptr {
	// write this later
};


