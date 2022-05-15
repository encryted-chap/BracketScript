section .bss

struc token_t
	.type: resb 1 ; type of token
	.raws: resd 1 ; raw string data
	.line: resd 1 ; line number
	.init: resb 1

	.size:
endstruc

; gets a token var, argument is 
; var to get (example, token_t.size)
%macro token_getv 1
	mov eax, token_t ; grab token
	sub eax, dword %1 ; calculate offset
%endmacro
