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
	
; 	allocated new memory block, index=0
	jmp 	end_func_global
func_global:
	sub 	esp, 0
	ret
end_func_global:
	nop
	
; 	allocate (byte)global::a
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	; 	call function func_global
	
; 	allocated new memory block, index=5
	
; 	copy: global::a -> address [ebp-0x5]
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
	mov 	dword [arg0], 0
endloop:
jmp endloop
