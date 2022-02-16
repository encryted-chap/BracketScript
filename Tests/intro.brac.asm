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
	
; 	allocate (byte)global::a
	mov 	eax, ebp
	sub 	eax, 5
	mov 	dword [eax-0], dword 0x0
	mov 	byte [eax-4], 0x0
	jmp 	end_somefunc_global
somefunc_global:
	
; 	copy: global:: -> address [ebp-0xA]
	mov 	eax, ebp
	mov 	ebx, eax

	sub 	eax, 0	; src
	sub 	ebx, 10	; dest

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

	jmp 	end_func_0_x85ckNrvz5rx
func_0_x85ckNrvz5rx:
	
; 	allocate (byte)1N1zOPRCL2Bt::d
	mov 	eax, ebp
	sub 	eax, 15
	mov 	dword [eax-0], dword 0x0
	mov 	byte [eax-4], 0x0
	
; 	copy: global:: -> address [ebp-0x14]
	mov 	eax, ebp
	mov 	ebx, eax

	sub 	eax, 0	; src
	sub 	ebx, 20	; dest

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

	nop
end_func_0_x85ckNrvz5rx:
	
; 	copy: global:: -> address [ebp-0x19]
	mov 	eax, ebp
	mov 	ebx, eax

	sub 	eax, 0	; src
	sub 	ebx, 25	; dest

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

	nop
end_somefunc_global:
	nop
endloop:
jmp endloop
