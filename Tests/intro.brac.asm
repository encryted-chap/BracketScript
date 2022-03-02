
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocated new memory block, index=0
	jmp 	end_new_global
new_global:
nop
	sub 	esp, 0
	ret
end_new_global:
	
; 	allocate (byte)global::b
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	
; 	allocate (int)global::i
	
; 	allocated new memory block, index=5
	sub 	esp, 1
	mov 	dword [esp-0], dword 0x0
	; 	not implemented exception
endloop:
jmp endloop
