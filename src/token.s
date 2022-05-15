%include "lib/token.s"

section .data

c_line: dd 0

section .text

global newline, new_token
extern malloc, free

newline:
	inc dword [c_line]
	ret

; makes the default token
new_token: ; token_t *new_token()
	token_getv token_t.size ; get size (eax)
	push eax ; pass size

	call malloc ; allocate new token
	add esp, 4 ; clean

	; eax holds token_t*
	mov ebx, eax ; offload
	token_getv token_t.line ; get line offset

	mov ecx, dword [c_line] ; get line
	mov dword [ebx+eax], ecx ; set line num

	token_getv token_t.init
	mov dword [ebx+eax], 1  ; set init flag

	mov eax, ebx ; return token_t*
	ret ; return
