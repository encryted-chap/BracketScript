section .text
extern malloc

global create_scope
%define SIZE_SCOPE_T 14

struc scope_t
	.parent: resd 1 ; scope_t*
	.child: resd 1 ; scope_t**

	.chlen: resb 1 ; uint8_t, # of children
	.refid: resd 1 ; char*

	.type: resb 1 ; uint8_t
	.typeptr: resd 1 ; void*

	.size:
endstruc

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
