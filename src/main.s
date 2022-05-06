section .text

extern fopen, lexer
global main

main: ; int main(int,char**)
	dec dword [esp+4] ; decrement arg
	add dword [esp+8], 4 ; increment past path

	cmp dword [esp+4], 0 ; if no args, end
	je .return

	; free up registers
	xor eax, eax
	mov ebx, eax
	mov ecx, eax
	mov edx, eax

.args: ; now parse arguments
	mov eax, [esp+8] ; get char**
	mov ecx, [esp+4] ; get int

	jecxz .end ; if arg count = 0, break

	mov ebx, [eax] ; get char*
	cmp byte [ebx], '-' ; switch value

	je .switch ; parse switch

	; this is an input file
	cmp dword [_file], 0
	jne .return ; end if more than one input file

	; now get FILE* object and store it

	push _io.r ; push file permissions
	push ebx ; push file path

	call fopen ; open file

	mov dword [_file], eax ; store FILE*
	add esp, 8

	cmp eax, 0
	je .return ; error opening input file

	jmp .continue

.end: ; end argument loop
	push dword [_file] ; pass FILE*
	
	call lexer ; start compilation
	add esp, 4 ; cleanup stack

	xor eax, eax
	ret
.continue:
	add dword [esp+8], 4
	dec dword [esp+4]

	jmp .args ; continue

.switch:
	

.return:
	ret

section .data

_file: dd 0

_io:
	.r db "r+",0
	.w db "w+",0
