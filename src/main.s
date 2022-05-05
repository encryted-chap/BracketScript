section .text
global main
extern get_global, fopen, printf

main:
	; clear up registers:
	xor eax, eax
	mov ebx, eax
	mov ecx, eax
	mov edx, eax

	; skip path
	dec dword [esp+4] ; int argc
	add dword [esp+8],4 ; char **argv

	cmp dword [esp+4], 0
	je .return ; return if no args

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

	cmp dword [esp+4], 0
	je .return

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
	; grab input value
	mov eax, [esp+8] ; current char*
	add eax, 4 ; next char*

	cmp byte [ebx+1], 'o'
	je .out

	jmp .iterate

;; SWITCH FUNCTIONS

.out:
	; eax = output file

	push io_perm.w ; pass write perms
	push eax ; pass file

	call fopen ; create & open file
	add esp, 8 ; clean up stack

	cmp eax, 0 ; nullptr = error
	je .return ; todo: implement error

	mov dword [file.out], eax ; store FILE*

	; iterate twice
	dec dword [esp+4] ; dec arg count
	add dword [esp+8], 4 ; increment to file

	jmp .iterate ; continue

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
.out: dd 0
