
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocate (byte)global::a
	
; 	allocated new memory block, index=0
	sub 	esp, 0
	mov 	byte [esp-0], byte 0x0
	
; 	freed used memory block, index=0
	
; 	allocated used block, index=0
	
; 	copy: global:: -> address [ebp-0x0]
	sub 	esp, 0

	mov 	esi, esp

	sub 	esp, 0
	mov 	edi, esp
	mov 	ecx, 1
	std

	rep 	movsb
	cld
	; 	endcopy

endloop:
jmp endloop
