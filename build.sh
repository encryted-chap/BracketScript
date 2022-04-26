for i in lib/*.s src/*.s ; do
	nasm -felf32 -o $i.o $i
done

gcc */*.o -o bs -m32
rm */*.o
