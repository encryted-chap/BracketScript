global entry, _switch, file
extern printf, pass_file

; max amount of args
%define ARG_MAX 32

section .text

entry:
	pop edx ; return address (void*)
	
	pop ecx ; len
	pop eax ; args (char**)

	mov dword [entry_ret], edx ; store ret addr

	mov edx, [eax] ; store char* of path
	mov dword [path], edx ; store char*

	add eax, 4 ; increment past path
	dec ecx ; one less entry

	; clear up unnecissary regs:
	xor ebx, ebx
	mov edx, ebx

.src: ; pass source code
	jecxz .end ; to-do: implement version num
	pusha ; store regvals

	mov eax, [eax] ; get char*
	mov dword [file], eax ; store char* of source
	call pass_file ; call C

	popa
	
	add eax, 4 ; inc past char*
	dec ecx ; one less entry

	mov ebx, _args ; store symbol

	mov dword [ebx], eax ; store char*
	mov dword [_args_len], ecx

.end:
	push dword [entry_ret] ; restore ret addr
	ret ; return

section .data

file: dd 0 ; char *file
_switch: dd 0 ; char *_switch

_args: dd 0
_args_len: ; int
	dd 0


path: dd 0 
entry_ret: dd 0
