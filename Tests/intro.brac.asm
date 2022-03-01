
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocated new memory block, index=0
	jmp 	end_func_global
func_global:
	sub 	esp, 0
	ret
end_func_global:
	nop
	nop
	
; 	allocate (byte)global::b
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	; 	call function func_global
	
; 	allocated new memory block, index=5
	
; 	copy: global::b -> address [ebp-0x5]
	sub 	esp, 0
	mov 	esi, esp
	sub 	esp, 1
	mov 	edi, esp
	mov 	ecx, 1
	std
	rep 	movsb
	cld
	add 	esp, 5
	call 	func_global
	; 	dealloc variable 
endloop:
jmp endloop
