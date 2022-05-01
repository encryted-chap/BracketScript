#include <stdio.h>
#include <string.h>
#include "lexer.h"

extern char *_switch; // extern from src/main.s
extern char *file; // extern from src/main.s

#define EOF 0x05

// pass file, and it is processed
void pass_file() {

	FILE *f = fopen(file, "r+");

	// get length of file
	fseek(f, 0L, SEEK_END);
	int sz = ftell(f);
	rewind(f);

	if((int)f == 0) {
		// error
		return;
	}
	
	unsigned char c = 0;
	unsigned char *fdat = (unsigned char*)malloc(sz);
	do {
		c = (unsigned char)fgetc(f); // get next char from stream
		*fdat++ = c;
	} while(!feof(f));
	
	fdat -= sz;
	
	char lnbuff[255]; // 255 chars max per line
	int buffer_i = 0;
	for(int i = 0; i < sz; i++) {
		// parse lines and send to
		// pass_line(char*)
		if(fdat[i] == '\n') {
			line(lnbuff); // pass line buffer
			for(int j = 0; j < 255; j++)
				lnbuff[j] = '\0'; // clear it out
			buffer_i = 0; // clear
		} else {
			lnbuff[buffer_i++] = fdat[i];
			continue;
		}
	}
	fclose(f); // and finally, close file
}
