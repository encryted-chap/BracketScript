;; a source file responsible for reading/writing files
global pass_file
extern fgetc ; char *fgets(char*,int,FILE*)
extern malloc

%define EOF 0x05 ; end of file char

section .text

pass_file: ; void pass_file(FILE*)
	pop edx ; ret addr
	pop eax ; FILE*

	mov dword [temp_fptr], eax ; store FILE*
	mov ebx, templn ; get line buffer

	push edx ; push address again
	xor edx, edx ; remove addr

.loop:
	pusha ; push regs

	cmp dword [temp_fptr], 0x0 ; if FILE* = 0, end
	je .end ; return

	call get_ln ; get line of code
	pop eax ; char* line

	; todo: pass line to lexer

	popa
	jmp .loop ; continue
.end:
	popa ; restore regs
	ret ; return

;; called to get a single line from the character buffer
get_ln: ; char *get_ln()
	mov ebx, templn ; pass templn
.loop:
	push dword [temp_fptr] ; pass arg
	call fgetc ; get char from file
	
	pop eax ; int (char casted to int)
	cmp eax, EOF ; if EOF, done
	je .endf

	cmp eax, 0x0a ; if newline, end
	je .end

	; now append to templn
	mov byte [ebx], al ; store char
	inc ebx ; increment tempbuffer

	jmp .loop ; continue

.endf:
	xor eax, eax ; clear eax
	mov dword [temp_fptr], eax ; zero out FILE*
.end:
	pop edx ; ret addr
	push dword templn ; push char*
	
	push edx ; restore ret addr
	ret ; return

section .data
temp_fptr:
dd 0
templn:
	times 255 db 0 ; 255b buffer
