global main

extern parse_cli, lexer, printf
extern fopen, fclose, pass_file

section .text

; the entrypoint of the BracketScript x86 compiler
main:
	; free up registers
	xor eax, eax
	mov ebx, eax
	mov ecx, eax
	mov edx, eax

	; get provided arguments
	pop dword [main_ret] ; store return address
	
	pop ecx ; int
	dec ecx ; remove result for path

	pop dword [current_path] ; path to this executeable
	
	; arguments are now pushed on the stack,
	; retrieve and parse them
.loop:
	cmp ecx, 0
	je .end

	pop eax ; pop argument to eax
	cmp byte [eax], '-' ; check if switch or file path
	je .handle_switch ; handle switch

	; file, open and pass to lexer
	push io_perm.read ; const char *mode
	push eax ; const char *filename

	call fopen ; opens and returns FILE*
	pop dword [tfile] ; store file
	
	cmp dword [tfile], 0 ; zero on error
	je .end ; kill program (todo turn into error msg)

	pusha ; store regs
	; since this is now without error,
	; pass file to be executed
	push dword [tfile] ; FILE*
	call pass_file ; returns nothing

	push dword [tfile] ; FILE*
	call fclose ; close file
	pop ebx ; should = 0
	; continue
	popa ; restore
	dec ecx
	jmp .loop
	
.handle_switch:
	pusha ; save regs

	popa ; restore regs
	dec ecx ; make sure to skip this one
	jmp .loop ; go back to looping

.end:
	push 0
	push dword [main_ret] ; restore ret address
	ret ; return

section .data
tfile:
	dd 0
io_perm:
.read: db "r+",0
.write: db "w",0,0,0

current_path:
	dd 0
main_ret:
	dd 0

error:
	db "FATAL, return=%d:",0x0A,"|--> %s",0

switch:
.debug: db 0
