struc scope_t
	.parent: resd 1 ; scope_t*
	.child: resd 1 ; scope_t**

	.chlen: resb 1 ; uint8_t, # of children
	.refid: resd 1 ; char*

	.type: resb 1 ; uint8_t
	.typeptr: resd 1 ; void*

	.size:
endstruc

; type 1
;; a static class scope
struc class_t
	.scope: resd 1 ; scope_t*
	.inscope: resd 1 ; a scope to inherit for instances
endstruc

struc var_t
	.class: resd 1 ; class_t* of class
	.scope: resd 1 ; scope_t* of this class
endstruc
