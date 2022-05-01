function compile_s {
	for i in $@ ; do
		printf "compiling \'Assembly\' source %s ... " $i
		nasm -felf32 -o $i.o $i
		printf "done! (%s.o)\0\n" $i
	done
}
function compile_c {
	for i in $@ ; do
		printf "compiling \'C\' source %s ... " $i
		gcc -m32 -I lib -o $i.o -w -c $i
		printf "done! (%s.o)\0\n" $i
	done
}

compile_s */*.s
compile_c */*.c

gcc */*.o -w -o bs -m32 -I lib
rm */*.o
