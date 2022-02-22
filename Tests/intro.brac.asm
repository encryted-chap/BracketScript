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
	jmp 	end_testfunc_global
testfunc_global:
	
; 	allocate (byte)kciMyVrL1PCS::b
	
; 	allocated new memory block, index=4
	sub 	esp, 4
	mov 	byte [esp-0], byte 0x0
	add 	esp, 4
	ret
end_testfunc_global:
	; 	call function testfunc_global
	sub 	esp, 0
	call 	testfunc_global
	
; 	allocate (byte)global::c
	
; 	allocated new memory block, index=5
	sub 	esp, 5
	mov 	byte [esp-0], byte 0x0
endloop:
jmp endloop
