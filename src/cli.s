global parse_cli

parse_cli: ; parse_cli(char**,int)
	pop edx ; store ret address

	pop eax ; char**
	pop ebx ; int
	xor ecx, ecx ; clear up ecx

	; return
	push edx
	ret


