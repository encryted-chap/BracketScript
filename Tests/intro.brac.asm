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
	mov 	dword [eax-0], dword 0x68656C6C
	mov 	dword [eax-4], dword 0x6F2C2077
	mov 	dword [eax-8], dword 0x6F726C64
	mov 	byte [eax-12], 0x21
endloop: jmp endloop
