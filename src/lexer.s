section .text

%define L_MAX 255
%define T_MAX 255

global bs_exec, get_line, lexer
extern strlen, fgetc, malloc

get_line: ; get_line(FILE*, char *out)
	push dword [esp+4] ; pass FILE*

	call fgetc ; grab char
	add esp, 4 ; store

	mov ebx, dword [esp+8] ; grab char*
	mov byte [ebx], al ; store al

	inc dword [esp+8] ; next char

	; check for terminating char
	cmp eax, -1
	je .ret ; return if EOF

	cmp eax, 0x0a
	je .ret ; return if \n

	jmp get_line
.ret:
	ret

lexer: ; int lexer(char *line, int ln_len)
	cmp dword [esp+8], 0 ; check for empty line
	je .ret

	cmp dword [esp+4], 0 
	je .ret ; end if null buffer
.ret:
	ret

; execute a single line of bs code
bs_exec: ; void bs_exec(char*)
	push dword [esp+4] ; pass char*
	call strlen
	add esp, 4

	push eax ; size
	push dword [esp+4] ; FILE*

	;call lexer
	add esp, 4 ; clean up

	ret
section .data

_ln: dd 0

_tln: dd 0
_cln: dd 0

_ind: dd 0 ; indentation, 1 tab is 4 indents
