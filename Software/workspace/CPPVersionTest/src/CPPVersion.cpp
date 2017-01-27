//============================================================================
// Name        : CPPVersion.cpp
// Author      : 
// Version     :
// Copyright   : Your copyright notice
// Description : Hello World in C++, Ansi-style
//============================================================================

#include <iostream>
using namespace std;

int main() {
		#if __cplusplus==201402L
		std::cout << "C++14" << std::endl;
		#elif __cplusplus==201103L
		std::cout << "C++11" << std::endl;
		#else
		std::cout << "C++" << std::endl;
		std::cout << __cplusplus << std::endl;
		#endif
		
		return 0;
}
