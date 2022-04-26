extern strlen, malloc
global lexer

%define TOKEN_SIZE 0x8

section .text

;;;
; struct _token
; uint32_t line (0x0)
; char *raw (0x4)

; a function dedicated to cutting up a line
; into useable chunks
lexer: ; struct _token lexer(char* line)
	; check if lexer initialized
	mov dl, byte [lexer_init] ; store init byte
	cmp dl, 0x0 ; if dl == 0, call .lexer_i
	je .lexer_i

.get_vals
	pop dword [lexer_ret] ; store ret address

	push dword [esp] ; clone line for arg
	call len ; get line length

	pop ecx ; length
	pop eax ; line

	; reserve space for token
	push TOKEN_SIZE
	call malloc ; reserve space

	; returns void*
	pop ebx

	; return
	push dword [lexer_ret]
	ret
.lexer_i

	; end initialization:
	xor edx, edx
	jmp .get_vals

len: ; int len(char*)
	pop edx
	pop eax
	
	; zero result
	xor ebx, ebx
.loop:
	; if [eax] == 0, end
	cmp byte [eax], 0
	je .end

	inc eax ; increment
	inc ebx ; inc result
	jmp .loop ; jump up to loop
.end:
	push ebx ; result
	push edx ; ret

	ret ; return

find: ; int find(char* text, char* substring);
	pop dword [lexer_ret.find] ; ret addr
	pop eax ; text
	pop ebx ; substring

	xor ecx, ecx ; zero result
.loop:
	mov dl, byte [eax] ; get char from substring
	cmp dl, byte [ebx] ; compare
	je .compare

	inc eax
	jmp .loop

.compare:
	pusha ; store regs

.cmploop:
	; increment
	inc ebx
	inc eax

	cmp byte [ebx], 0
	je .cmploop_f

	mov dl, byte [eax]
	cmp dl, byte [ebx]
	jne .cmploop_nf

	jmp .cmploop
.cmploop_nf:
	popa

	; increment for next iteration and ret
	inc eax
	jmp .loop
.cmploop_f:
	popa ; restore regs
	jmp .end ; end

.not_found:
	mov ecx, -1
.end:
	push ecx
	push edx

	ret
section .data
lexer_init: db 0

lexer_ret:
	dd 0
.retval:
	dd 0
.find:
	dd 0
