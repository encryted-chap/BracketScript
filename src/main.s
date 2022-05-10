section .text

extern bs_exec, get_line, fopen, fclose, fgetc, feof
extern strlen, malloc, free
global main

main:
	cmp dword [esp+4], 2 ; check for arg count
	jne .end

	; free up registers
	xor eax, eax
	mov ebx, eax
	mov ecx, eax
	mov edx, eax

	add dword [esp+8], 4 ; next char*
	dec dword [esp+4] ; dec int

	mov eax, dword [esp+8] ; grab char**

	push _io.r ; pass priveledges
	push dword [eax] ; pass char*

	call fopen ; open file
	add esp, 8 ; stack cleanup

	mov dword [_in], eax ; store FILE*
	cmp eax, 0 ; if nullptr, error

	; je .error

.loop:
	; allocate line buffer
	push 512 ; line size max

	call malloc ; allocate memory
	add esp, 4

	; eax = void *line_buffer
	cmp eax, 0
	je .loop ; try to allocate again

	mov dword [_ln], eax ; store void*

	push eax ; pass void*
	push dword [_in] ; pass FILE*

	call get_line ; grab a single line
	add esp, 8 ; clean

	push dword [_in] ; pass line
	call strlen ; get length

	add esp, 4

	; free line buffer
	push dword [_in]
	call free

	add esp, 4
.end:
	ret

section .data
_ln: dd 0
_in: dd 0
_io:
	.w db "w+",0
	.r db "r+",0
