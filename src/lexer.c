#include <stdint.h>

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
		**contained_f, // taken as a (_funcptr**)
		**contained_c, // taken as a (_classptr**)
		**contained_v; // taken as a (_varptr**)

	// parent _scopeptr*
	struct _scopeptr *parent;
	struct _scopeptr **child;
} *_global;

// _token.type
enum {
	JMP, // jump to a diff token
	VAR_CREATE, // initialize variable
};

struct _token {
	// the tokens that come before
	// and after this one
	struct _token *last,
				  *next;
	uint8_t type;
	char *plaintext;
	void *data; // type of this depends on type
} *_t; // tokens that make up executeable code

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

int lexer_init=0;
void LEXER_INIT() {
	struct _scopeptr g; // initialize with defaults
	*_global = g; // assign
}

struct _scopeptr *CREATE_SCOPE(char *name, struct _scopeptr *parent) {
	struct _scopeptr s; // initialize scope
	
	// assign basic struct values
	s.parent = parent;
	s.child  = 0;
	
	// copy name
	char *refid = (char*)malloc(strlen(name)+1); // allocate buffer
	memcpy(refid, name, strlen(name)); // copy
	s.refid = refid; // assign new buffer
	
	return &s;
}

// adds a single 
void ADD_CHILD(struct _scopeptr *s, struct _scopeptr *child) {
	
}

// search for scope within a scope's allowed access
struct _scopeptr *SEARCH_LOCAL(struct _scopeptr *s, char *refid) {
	// search up
	struct _scopeptr *p = s->parent; // get parent
	
	// check children
	for(int i = 0; s->child[i] == 0; i++) {
		if(s->child[i]->refid) { return s->child[i]; }
	}

	if(!strcmp(s->refid, refid)) { 
		return s; // return struct
	} else if(!strcmp(s->refid, "global")) {
		return 0; // not found, return nullptr
	} else {
		// continue searching recursively
		p = SEARCH_LOCAL(s->parent, refid);
		
		if(!p) {
			// error
			return -1;
		} else { return p; }
	}
}

int begins(char *s, char *begin) {

	// if begin size > s, it does not start with begin
	int begin_len = strlen(begin);
	if(begin_len > strlen(s)) { return 0; }

	char s_db[begin_len+1]; // allocate buffer
	for(int i = 0; i < begin_len; i++) {
		// initialize s_db
		s_db[i] = s[i];
	}

	s_db[begin_len] = (char)0; // null terminate
	return !strcmp(begin, s_db); // get answer
}

int ends(char *s, char *end) {
	// get length
	int s_len = strlen(s),
		e_len = strlen(end);

	if(e_len > s_len) { return 0; }
	char *end_s = &s[s_len - e_len]; // get cut string

	return !strcmp(end_s, end);
}

int split_max = 512;

char **split(char *s, char schar) {
	char *r[split_max];
	char **ret = r;
	*r = "\0"; // initialize first val   
	for(int i = 0; i < strlen(s); i++) {
		if(s[i] == schar) {
			s = &s[i+1]; // re-assign s
		}
	}
	return ret;
}

char *substring(char *s, int start_index, int end_index) {
	int s_len = strlen(s); // get s len
	
	// fill cloned buffer
	char *new_buffer = (char*)malloc(s_len);
	for(int i = 0; i < end_index; i++) 
		new_buffer[i] = s[i];

	new_buffer = &new_buffer[start_index];
	return new_buffer;
}

struct _scopeptr *current_scope = 0;
struct _token *_next;
void execute_next() {
	if(current_scope == 0) 
		current_scope = _global;
	
	switch(_next->type) {
		case JMP: {
			_next = (struct _token*)_next->data; // assign
		} return;
	}

	_next = _next->next; // move to next token
}

// split a line into _tokens, then execute them
void line(char *ln) {
	if(begins(ln, "#")) {
		
	} else {
		
	}
}
