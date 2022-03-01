
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocated new memory block, index=0
	jmp 	end_new_Awria9brnCNe
new_Awria9brnCNe:
	sub 	esp, 0
	ret
end_new_Awria9brnCNe:
	
; 	allocate (int)global::i
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	dword [esp-0], dword 0x0
	
; 	allocate (byte)global::b
	
; 	allocated new memory block, index=8
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	
; 	freed used memory block, index=8
	
; 	allocated used block, index=8
	
; 	copy: global:: -> address [ebp-0x8]
	add 	esp, 8

	mov 	esi, esp

	sub 	esp, 8
	mov 	edi, esp
	mov 	ecx, 1
	std

	rep 	movsb
	cld
	; 	endcopy

endloop:
jmp endloop
