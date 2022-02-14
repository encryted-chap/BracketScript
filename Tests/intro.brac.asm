section .data
	arg9: 	dd 0
	arg8: 	dd 0
	arg7: 	dd 0
	arg6: 	dd 0
	arg5: 	dd 0
	arg4: 	dd 0
	arg3: 	dd 0
	arg2: 	dd 0
	arg1: 	dd 0
	arg0: 	dd 0
	tempret: 	dd 0

section .text
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocate (byte)global::c
	mov 	eax, ebp
	sub 	eax, 0
	mov 	byte [eax-0], 0x0
testFunc_global:
	
; 	allocate (byte)zY9YYlxszj5p::argument_1
	mov 	eax, ebp
	sub 	eax, 1
	mov 	byte [eax-0], 0x0
	
; 	allocate (byte)zY9YYlxszj5p::argument_2
	mov 	eax, ebp
	sub 	eax, 2
	mov 	byte [eax-0], 0x0
	nop
endloop:
jmp endloop
