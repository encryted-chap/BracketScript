section .text

%include "lib/debug.s"

%define L_MAX 255
%define T_MAX 255

global bs_exec, get_line, lexer
extern strlen, free, printf, fgetc, feof, malloc

lexer: ; int lexer(char *line, int ln_len, FILE *_in)
	push dword [esp+12] ; pass _in

	call feof ; check for eof
	add esp, 4 ; clean

	cmp eax, 0
	jne .eof

	jmp .ret
.eof:
	mov eax, -1
.ret:

	ret

