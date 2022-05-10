section .text

%include "lib/debug.s"

extern bs_exec, get_line, fopen, fclose, fgetc, feof
extern strlen, malloc, free, lexer

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

	push dword __LINE__ ; for error handler

	mov dword [_in], eax ; store FILE*
	cmp eax, 0 ; if nullptr, error

	je .file_nf
	add esp, 4 ; clear line num

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

	mov byte [_last], 1 ; set "last"
.nlast:
	push dword [_in] ; pass line

	call strlen ; get length
	add esp, 4

	push eax ; pass length
	push dword [_ln] ; pass FILE*

	call lexer ; process line
	add esp, 8

	; free line buffer
	push dword [_ln]
	call free

	add esp, 4 ; stack clean

	push dword [_in] ; pass FILE*

	call feof ; check for end of file
	add esp, 4 ; clean stack

	cmp eax, 0
	je .loop
.end:
	; close input file
	push dword [_in] ; pass FILE*
	call fclose

	add esp, 4 ; cleanup
	ret ; return


.file_nf:
	pop eax ; grab line number
	error _err.fnf, eax ; call error

	ret

section .data
_last: db 0

_ln: dd 0
_in: dd 0

_io:
	.w db "w+",0
	.r db "r+",0

_err:
	.fnf: db "source input file not found!",0
