extern strlen, malloc
global lexer

%define TOKEN_SIZE 0x8
extern malloc, free

section .text

;::::::::::::::::::::::::::;.
;     struct _token       ;||
;;;;;;;;;;;;;;;;;;;;;;;;;;;||
; uint8_t flags -- (0x00) ;||
; char *line ----- (0x01) ;||
; uint8_t t_type - (0x05) ;||
; uint16_t line -- (0x06) ;||
;;;;;;;;;;;;;;;;;;;;;;;;;;;||
`````````````````````````````
; a function dedicated to cutting up a line
; into useable chunks
lexer: ; struct _token *lexer(char* line)
	pop dword [lexer_ret.addr] ; pop address
	push dword [esp] ; clone char*
	
	call strlen ; returns int
	pop ecx ; get length

	cmp ecx, 0 ; if len = 0,
	je .end ; end function

	pusha ; prep for malloc

	mov ebx, 4*30 ; size of _token* x num of max tokens per line
	push ebx ; size_t size

	call malloc ; allocate memory
	pop dword [lexer_ret.ret_val] ; store void*

	popa ; restore registers
	
.end:
	push dword [lexer_ret.ret_val] ; push _token**
	push dword [lexer_ret.addr] ; push ret addr
	
	ret ; return

strlen: ; int strlen(char*)
	pop edx ; addr
	pop eax ; char*
	
	xor ebx, ebx ; zero result
.loop:
	cmp byte [eax], 0x0 ; check for null terminator
	je .end ; end

	; increment count and line
	inc eax
	inc ebx
	jmp .loop ; end
.end:
	push edx 
	ret
	
section .data
lexer_ret:
	.addr: dd 0
	.ret_val: dd 0