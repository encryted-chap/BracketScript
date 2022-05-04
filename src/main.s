section .text
global main
extern create_scope, fopen, printf

main:
	; clear up registers:
	xor eax, eax
	mov ebx, eax
	mov ecx, eax
	mov edx, eax

	; skip path
	inc dword [esp+4] ; int argc
	add dword [esp+8],4 ; char **argv

	jecxz .return ; end if no arguments (to do, make this a help screen)

	; increment past path
	add eax, 4
	dec ecx

	mov dword [esp+8], eax ; store skipped path
	mov dword [esp+4], ecx ; store arg len

.args: ; parse
	; restore values
	mov eax, [esp+8] ; char**
	mov ecx, [esp+4] ; int

	jecxz .return ; kill if args = 0
	mov ebx, [eax] ; offload char* to ebx

	cmp byte [ebx], '-' ; this means it is a switch
	je .switch

	;; this means it is an input file
	;; handle it as such

	pusha ; store regs

	; push args
	push io_perm.rp
	push ebx

	call fopen ; get FILE*
	mov dword [file], eax ; store FILE*

	add esp, 8 ; cleanup stack
	jmp .iterate

.iterate:
	add dword [esp+8], 4 ; iterate to next char*
	dec dword [esp+4] ; int -= 1

	jmp .args ; continue
.return:
	cmp byte [printnl], 0 ; if printnl != 0, print
	je .skipnl

	; now print newline
	push 0x0a ; newline char
	push print_strings.chr ; single char

	call printf ; print
	add esp, 8 ; cleanup stack
.skipnl:
	xor eax, eax ; return 0
	ret
.switch:
	inc ebx ; increment char*
	mov dl, [ebx] ; grab char value

	jmp .iterate
; sets the input file program-wide
set_input: ; FILE *set_input(char*)
	; setup subroutine
	push ebp
	mov ebp, esp

	; clear up regs
	xor ebx, ebx
	mov ecx, ebx
	mov edx, ebx

	push io_perm.rp
	push dword [ebp+8] ; push filename

	call fopen ; open file
	add esp, 4 ; clean up stack

	pop ebp ; restore ebp
	ret ; return
section .data

io_perm:

.r: db "r",0
.w: db "w",0

.rp: db "r+",0
.wp: db "w+",0

printnl: db 0 ; if set, print newline at the end

print_strings:
.chr: db "%c",0

file: dd 0
