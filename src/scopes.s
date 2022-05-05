section .text
extern malloc

global get_global, create_scope

%define SIZE_SCOPE_T 14
%include "lib/scopes.s" ; for structs

scope_null: ; int scope_null(scope_t*)
	mov eax, [esp+4] ; get scope

	cmp eax, 0
	je .isnull ; if nullptr, its null

	mov ecx, scope_t.size - scope_t ; get size of scope_t
.loop:
	cmp byte [eax], 0
	jne .return ; if not 0, not null

	jecxz .isnull ; if ecx = 0, null
	
	; iterate
	dec ecx ; one less
	inc eax ; next byte

	jmp .loop

.isnull:
	mov eax, 1
.return:
	ret

create_scope: ; scope_t *create_scope(scope_t *parent, char *name)
	push scope_t.size - scope_t ; sizeof scope_t
	call malloc ; allocate size for ret

	add esp, 4 ; clean stack
	mov ebx, eax ; offload void* to ebx

	mov eax, [esp+4] ; grab parent
	mov dword [ebx + (scope_t.parent - scope_t)], eax ; store parent

	mov eax, [esp+8] ; grab name
	mov dword [ebx + (scope_t.refid - scope_t)], eax ; store name

	mov eax, ebx ; eax = scope_t*
	ret ; return

; returns a pointer to the global scope
get_global: ; scope_t *get_global()
	cmp dword [global_scope], 0
	jne .return

	; initialize global
	push dword _g ; push name
	push 0x0 ; nullptr

	call create_scope ; create global
	add esp, 8 ; cleanup stack

	mov dword [global_scope], eax ; store scope
	ret ; return

.return:
	mov eax, dword [global_scope] ; return scope
	ret

section .data

_g: db "global",0
global_scope: dd 0
