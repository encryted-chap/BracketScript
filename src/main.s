section .text

%include "lib/debug.s"

extern bs_exec, fgets, get_line, fopen, fclose, fgetc, feof
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

	mov dword [_in], eax ; store FILE*
	cmp eax, 0 ; if nullptr, error

	je .file_nf

.loop:
	push dword 255

	call malloc ; allocate line buffer
	add esp, 4 ; clean

	cmp eax, 0x0 ; nullptr
	je .allc_fl ; memory alloc error

	push dword [_in] ; pass FILE*
	push dword 255 ; pass n
	push dword eax ; char*

	call fgets ; grab line
	add esp, 12 ; stack cleanup

	cmp eax, 0x0 ; nullptr
	je .allc_fl ; memory alloc error

	mov dword [_ln], eax ; store char*
	push eax ; pass char*

	call strlen ; get length
	add esp, 4 ; clean

	push dword [_in] ; pass FILE*
	push dword eax ; pass len
	push dword [_ln] ; pass char*

	call lexer ; pass line to program
	add esp, 12 

	cmp eax, -1
	jne .loop ; continue
.end:
	; close input file
	push dword [_in] ; pass FILE*
	call fclose

	add esp, 4 ; cleanup
	ret ; return


.allc_fl: ; allocation fail
	mov eax, __LINE__
	error _err.mae, eax

	ret
.file_nf:
	mov eax, __LINE__
	error _err.fnf, eax ; call error

	ret

.file_rd: ; file read error
	mov eax, __LINE__
	error _err.frd, eax

	ret
section .data
_last: db 0
_ln: dd 0
_in: dd 0
_io:
	.w db "w+",0
	.r db "r+",0
_err:
	.mae: db "memory allocation error",0
	.frd: db "unable to read input file",0
	.fnf: db "source input file not found",0
