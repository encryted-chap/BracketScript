;; a source file responsible for reading/writing files
global pass_file
extern fgets ; char *fgets(char*,int,FILE*)
extern malloc

%define EOF 0x05 ; end of file char

section .text

pass_file: ; void pass_file(FILE*)
	pop edx ; ret
	pop eax ; FILE*

.loop:
	pusha ; store regs
	
	; push args
	push eax ; FILE*
	push 255 ; int
	push templn ; char*

	call fgets

	pop eax ; get char*
	cmp eax, 0 ; done
	je .end
	
	popa ; restore
	jmp .loop
.end:
	popa ; pop regs
	push edx ; ret addr
	ret ; return

section .data

templn:
	times 255 db 0 ; 255b buffer
