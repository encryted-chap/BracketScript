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
	mov 	byte [eax-0], 104
	mov 	byte [eax-1], 101
	mov 	byte [eax-2], 108
	mov 	byte [eax-3], 108
	mov 	byte [eax-4], 111
	mov 	byte [eax-5], 44
	mov 	byte [eax-6], 32
	mov 	byte [eax-7], 119
	mov 	byte [eax-8], 111
	mov 	byte [eax-9], 114
	mov 	byte [eax-10], 108
	mov 	byte [eax-11], 100
	mov 	byte [eax-12], 33
	xor 	eax, eax
	xor 	ebx, ebx
	xor 	ecx, ecx
	xor 	edx, edx
