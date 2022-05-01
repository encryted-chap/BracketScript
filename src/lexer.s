global pass_line
extern line

section .text

pass_line: ; void pass_line(char* line)
	pop edx ; ret addr (void*)
	pop eax ; char*

	pusha ; store regs
	popa ; restore

	push edx
	ret
