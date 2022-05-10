section .text

%include "lib/debug.s"

%define L_MAX 255
%define T_MAX 255

global bs_exec, get_line, lexer
extern strlen, printf, fgetc, malloc

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
	mov dword [_ind], 0
	cmp dword [esp+8], 0 ; check for empty line

	je .ret

	cmp dword [esp+4], 0 
	je .ret ; end if null buffer

	mov eax, dword [esp+4] ; grab buffer

	; try to get indent mode
	cmp byte [_imd], 0
	jne .no_mode ; call mode

	mov eax, dword [esp+4] ; get char*
	cmp byte [eax], 0x9 ; tabs?

	jne .sptst

	mov byte [_imd], 1
	jmp .mode

.sptst:
	cmp byte [eax], ' '
	jne .no_mode

	mov byte [_imd], 2
	jmp .mode
.mode:
	; resolve whitespace and
	; remove it
	xor ebx, ebx ; clear ebx
	mov ecx, ebx ; clear up ecx too

	mov bl, byte ' ' ; spare space seperator
	cmp byte [_imd], 2 ; spaces?

	je .ws_remove
	mov bl, byte 0x9 ; tab seperator

.ws_remove: ; remove whitespace
	cmp byte [eax], bl
	jne .end_wsrm

	inc dword [_ind] ; add one for indentation

	inc eax ; next char*
	dec dword [esp+8] ; decrement len

	jmp .ws_remove ; iterate
.end_wsrm:
	mov dword [esp+4], eax ; store new char*
.no_mode:
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
_imd: db 0 ; indent mode, 1 = tab, 2 = spaces

_str: db "token: %s",0x0a,0
