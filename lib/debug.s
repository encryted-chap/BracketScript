section .data

;; ----====[ DATA FOR DIAGNOSTIC MACROS ]====---- ;;

; int 1 = __LINE__, int 2 = source line, string 1 = err message
MSG: db "FATAL ERROR (src:%d): %s",0xa,0


section .text
extern printf

; throws error if eax is nonzero
%macro error 2
	push dword %1 ; get message (char*)
	push %2 ; grab line number (int)

	push MSG ; message
	call printf

	add esp, 12
%endmacro
