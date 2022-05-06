section .text

global lexer
extern fgetc, malloc

lexer: ; void lexer(FILE*)
	; clear up registers
	xor ebx, ebx
	mov ecx, ebx
	mov edx, ebx

.loop:
	call get_line ; get single line
	cmp dword [_fend], 0

	jne .end

	push eax ; pass char*
	call pass_line

	add esp, 4 ; restore stack
	jmp .loop ; iterate
.end:
	ret

; processes a single line of information
pass_line: ; void pass_line(char*)

	ret

; grab a single line from FILE*
get_line: ; char *get_line(FILE*)
	; allocate 255 byte buffer
	push 255
	call malloc ; allocate 255B

	add esp, 4 ; clean up stack
	cmp eax, 0

	je .end ; end if nullptr

	mov dword [_tb], eax ; offload void* to ebx
	mov eax, [esp+4] ; grab FILE*

	mov dword [_tf], eax ; store FILE*

	; clear regs
	xor ecx, ecx
	mov edx, eax

	push dword [_tb] ; ret value
.loop:
	push dword [_tf] ; pass FILE*
	call fgetc ; get char
	
	add esp, 4 ; cleanup

	cmp al, 0x0a
	je .end

	mov dword [_fend], 1
	cmp al, -1
	je .end

	mov dword [_fend], 0

	mov ebx, dword [_tb] ; grab void*
	mov byte [ebx], al ; store byte

	inc dword [_tb] ; inc buffer
	jmp .loop
.end:
	pop eax ; pop char*
	ret
section .data
_tf: dd 0
_tb: dd 0
_fend: dd 0
