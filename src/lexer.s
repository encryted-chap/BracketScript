section .text

%define L_MAX 255
%define T_MAX 255

global bs_exec, get_line
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
