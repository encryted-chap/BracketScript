section .text

%include "lib/debug.s"
%include "lib/str.s"

%define L_MAX 255
%define T_MAX 255

global lexer
extern printf
extern feof
extern newline, strtok

lexer: ; int lexer(char *line, int ln_len, FILE *_in)
	cmp dword [esp+4], 0
	je .eof ; end of file if nullptr

	push dword [esp+12] ; pass FILE*

	call feof ; check eof
	add esp, 4 ; cleanup

	cmp eax, 0 ; if !0, eof
	jne .eof

	mov eax, dword [esp+4] ; grab char*
	mov ebx, dword [esp+8] ; grab len

	mov byte [eax+ebx], 0 ; add null terminator
	xor ebx, ebx ; free up

	cmp dword [delimiter], 0x0
	jne .skipdef ; skip default delim if set

	mov dword [delimiter], def ; default delimiter
.skipdef:
	mov dword [str], eax ; store char*
.loop:
	push dword [delimiter] ; pass delim
	push dword [str] ; pass char*

	call strtok ; get token
	add esp, 8 ; clean

	mov dword [str], 0 ; nullify string
	push eax ; pass token

	; call token_handler
	add esp, 4

	cmp eax, 0 ; nullptr
	je .ret ; exit

	jmp .loop
.ret:
	ret
.eof:
	mov eax, -1
	jmp .ret ; return

section .data

str: dd 0

delimiter: dd 0
def: db " ",0
