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
	
; 	allocate (byte)global::a
	mov 	eax, ebp
	sub 	eax, 0
	mov 	byte [eax-0], 0x0
	
; 	allocate (byte)global::b
	mov 	eax, ebp
	sub 	eax, 1
	mov 	byte [eax-0], 0x0
	
; 	copy: JJPJvH8XEdgM:: -> address [ebp-0x2]
	mov 	eax, ebp
	sub 	eax, 0
	mov 	ebx, ebp
	sub 	ebx, 2

; 	transfer data:
	mov 	dl, byte [eax]
	mov 	byte [ebx], dl
	dec 	eax
	dec 	ebx
endloop:
jmp endloop
