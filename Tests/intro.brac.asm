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
	
; 	allocate (byte)global::b
	mov 	eax, ebp
	sub 	eax, 0
	mov 	dword [eax-0], dword 0x0
	mov 	byte [eax-4], 0x0
	
; 	copy: global:: -> address [ebp-0x5]
	mov 	eax, ebp
	mov 	ebx, eax

	sub 	eax, 0	; src
	sub 	ebx, 5	; dest

; 	transfer data:

	mov 	dl, byte [eax] ; byte 0
	mov 	byte [ebx], dl
	dec 	eax
	dec 	ebx

	mov 	dl, byte [eax] ; byte 1
	mov 	byte [ebx], dl
	dec 	eax
	dec 	ebx

	mov 	dl, byte [eax] ; byte 2
	mov 	byte [ebx], dl
	dec 	eax
	dec 	ebx

	mov 	dl, byte [eax] ; byte 3
	mov 	byte [ebx], dl
	dec 	eax
	dec 	ebx

	mov 	dl, byte [eax] ; byte 4
	mov 	byte [ebx], dl

	; 	now clear regs
	xor 	eax, eax
	xor 	ebx, ebx
	xor 	ecx, ecx
	xor 	edx, edx
	mov 	esp, ebp ; restore stack

endloop:
jmp endloop
