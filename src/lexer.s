section .text

global lexer
extern fgetc, malloc

lexer: ; void lexer(FILE*)
	ret
section .data
_tb: dd 0
