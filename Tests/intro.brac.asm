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
	
; 	allocated new memory block, index=0
	sub 	esp, 0
	mov 	byte [esp-0], byte 0x0
	
; 	allocate (byte)global::b
	
; 	allocated new memory block, index=1
	sub 	esp, 1
	mov 	byte [esp-0], byte 0x0
	
; 	allocate (byte)global::c
	
; 	allocated new memory block, index=2
	sub 	esp, 1
	mov 	byte [esp-0], byte 0x0
	
; 	freed used memory block, index=1
	
; 	allocated used block, index=1
	
; 	copy: global:: -> address [ebp-0x1]
	add 	esp, 2
	mov 	esi, esp
	sub 	esp, 1
	mov 	edi, esp
	mov 	ecx, 1
	std
	rep 	movsb
	cld
endloop:
jmp endloop
