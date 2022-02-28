
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
	; 	call function func_global
	sub 	esp, 0
	call 	func_global
endloop:
jmp endloop
