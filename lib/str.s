extern strcmp

%macro scmp 2
	push %1
	push %2

	call strcmp
%endmacro
