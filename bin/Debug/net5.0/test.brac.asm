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
	mov 	eax, ebp
	sub 	eax, 0
	mov 	byte [eax-0], 0x0
	mov 	0 edp
	mov 	edp 0
	mov 	bx, byte [eax-0]
	mov 	byte 0, bx
endloop:
jmp endloop