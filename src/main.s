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

	mov dword [main_ret.retcode], eax ; zero out retcode
	pop dword [main_ret] ; store ret addr
	
	pop ecx ; store number of args
	dec ecx ; remove # for path
	
	; if 0 args, return
	cmp ecx, 0
	je .end
	
	pop dword [current_path] ; store path to program

	; stack now holds argument values.
	; parse arguments now
.args:
	jecxz .end ; end when ecx = 0
	
	pop eax ; get char* to arg
	
	; check for error on eax
	push dword [main_ret.retcode] ; store og retcode
	mov dword [main_ret.retcode], 0x5 ; set error (arguments invalid error 0x5)

	cmp eax, ebx ; check if eax=0
	je .end ; (end will handle error)

	not ebx
	cmp eax, ebx ; check if eax=FFFFFFFF
	je .end

	; no errors, restore retcode
	pop dword [main_ret.retcode]

	cmp byte [eax], '-' ; if eax[0] == '-', its a switch
	je .switch

	
	; this means its a file! open file
	pusha ; store regs

	mov ebx, io_perm.read ; get char* of "r"
	push ebx ; mode
	push eax ; char* path

	call fopen ; open file (returns FILE*)

	pop dword [tfile] ; get FILE*
	popa ; restore regs
	; continue loop:
	dec ecx ; decrement ecx
	jmp .args ; jump back to loop

.end:
	push dword [main_ret.retcode] ; return code
	push dword [main_ret] ; restore ret address
	ret ; return

.switch:
	; handle switch
section .data
tfile:
	dd 0
io_perm:
.read: db "r",0
.write: db "w",0,0,0

current_path:
	dd 0
main_ret:
	dd 0
.retcode:
	dd 0
error:
	db "FATAL, return=%d:",0x0A,"|--> %s",0

switch:
.debug: db 0
