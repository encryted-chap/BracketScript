// lists and dictionaries and memory, oh my!
#include <stdint.h>
#define list_max 512

struct list {
	int _id; // id to _list
	int length;

	void (*add)(struct list*, void*); // add a single item to this list
};

// private information
struct _list {
	struct list *ls_object;
	uint8_t flags;
	uint32_t type_size;

	void *_b;
} *_ls = 0;


void add(struct list *self, void *item) {
	struct _list *ls = &_ls[self->_id]; // get _list
	
	void *buffer = malloc(self->length+1);
	memcpy(buffer, self->_b, self->length); // copy old buffer to new one
	buffer[self->length] =
}

int current_id = 0;

struct list create_list(int type_size) {
	if(_ls == 0) { 
		_ls = (struct _list*)malloc(list_max * sizeof(struct _list));
	}

	struct list ls;
	struct _list l;

	l.flags = 1; // init flag
	l._b = malloc(type_size); // allocate one entry

	ls._id = current_id;
	_ls[current_id++] = l;
	
	return ls;
}

struct _list *FIND(int id) {

	return &_ls[id];
}


