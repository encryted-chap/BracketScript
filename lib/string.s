section .text

extern malloc, strcmp, strlen
global subs, begins

subs: ; char *subs(char *s, int start, int end)
	mov eax, dword [esp+8] ; grab start
	add dword [esp+4], eax ; increment to start

	sub dword [esp+12], eax ; end -= start

	; allocate buffer
	push dword [esp+12] ; push len
	call malloc ; now allocate

	add esp, 4 ; restore

	mov ecx, dword [esp+12] ; grab len
	mov esi, dword [esp+4] ; source=s
	mov edi, eax ; dest=void*

	push edi ; store buffer
	rep movsb ; copy

	pop eax ; now restore dest
	ret

begins: ; int begins(char *s, char *begin)
	push dword [esp+8] ; pass begin
	call strlen ; get length

	add esp, 4 ; stack cleanup

	push eax ; end=len
	push 0 ; start at 0
	push dword [esp+4] ; push s

	call subs ; get substring
	add esp, 12 ; stack cleanup

	push eax ; push substring
	push dword [esp+8]

	call strcmp
	add esp, 8 ; stack cleanup

	not eax ; flip eax
	ret
