%include "lib/token.s"

section .data

token_def:
	.type db 0 ; type
	.raws dd 0 ; raws
	.line dd 0 ; line
	.init db 1 ; init
	.size:

c_line: dd 0

section .text

global newline, new_token
extern malloc, free

newline:
	inc dword [c_line]
	ret

; makes the default token
new_token: ; token_t *new_token()
	mov eax, dword [c_line] ; get line
	mov dword [token_def.line], eax ; store line num

	push token_def.size - token_def ; get size

	call malloc ; allocate buffer
	add esp, 4 ; clean

	mov ecx, token_def.size - token_def
	mov edi, eax ; void*
	mov esi, token_def

	rep movsb ; copy to new buffer
	ret ; return
