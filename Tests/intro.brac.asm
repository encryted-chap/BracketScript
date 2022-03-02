
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocated new memory block, index=0
	jmp 	end_new_aIxm2Fle3s7E
new_aIxm2Fle3s7E:
nop
	mov 	eax, 0
	mov 	ebx, 0
	mov 	ecx, 0
	mov 	edx, 0
	sub 	esp, 0
	ret
end_new_aIxm2Fle3s7E:
	
; 	allocate (byte)global::b
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	
; 	allocate (int)global::i
	
; 	allocated new memory block, index=5
	sub 	esp, 1
	mov 	dword [esp-0], dword 0x0
	; 	call function new_aIxm2Fle3s7E
	add 	esp, 5
	call 	new_aIxm2Fle3s7E
	mov 	eax, [tempret]
	mov 	esi, eax
	sub 	esp, 5
	mov 	edi, esp
	mov 	ecx, 4

std
	rep 	movsb
cld
endloop:
jmp endloop
