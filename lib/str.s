global strlen

strlen: ; char*
	pop edx ; ret
	pop eax ; char*

	xor ebx, ebx ; zero ebx (result)
.loop:
	cmp byte [eax], 0x0 ; look for null term
	je .end ; terminate

	; iterate again
	inc ebx ; result++
	inc eax ; chr++

	jmp .loop ; iterate
.end:
	push ebx ; push result
	push edx ; push ret

	ret ; return
