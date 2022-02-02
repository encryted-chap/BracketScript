nasm -fwin64 -o bracketscript_out.obj bracketscript_out.asm
link bracketscript_out.obj /subsystem:console /entry:main /out:out.exe

rm ./bracketscript_out.*