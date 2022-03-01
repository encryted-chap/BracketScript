
	tempret: 	dd 0
	global 	_start
_start:
	mov 	esp, ebp
	
; 	allocated new memory block, index=0
	jmp 	end_new_ovA4H5ACjSVT
new_ovA4H5ACjSVT:
	sub 	esp, 0
	ret
end_new_ovA4H5ACjSVT:
	
; 	allocated new memory block, index=4
	jmp 	end_main_global
main_global:
	sub 	esp, 4
	ret
end_main_global:
	nop
	; 	call function main_global
	sub 	esp, 0
	call 	main_global
endloop:
jmp endloop
