using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PLINQ_Benchmark
{
    public static class pfac
    {
        /*	primes.c        

	Compute the prime factorization of an integer.

	by: Steven Skiena
	begun: February 18, 2002
*/


        /*
        Copyright 2003 by Steven S. Skiena; all rights reserved. 

        Permission is granted for use in non-commerical applications
        provided this copyright notice remains intact and unchanged.

        This program appears in my book:

        "Programming Challenges: The Programming Contest Training Manual"
        by Steven Skiena and Miguel Revilla, Springer-Verlag, New York 2003.

        See our website www.programming-challenges.com for additional information.

        This book can be ordered from Amazon.com at

        http://www.amazon.com/exec/obidos/ASIN/0387001638/thealgorithmrepo/

        */


        public static List<long> prime_factorization(long x)
        {
            long i;			/* counter */
            long c;			/* remaining product to factor */
            List<long> lst = new List<long>();
            c = x;

            while ((c % 2) == 0)
            {
                lst.Add(2);
                //printf("%ld\n",2);
                c = c / 2;
            }

            i = 3;

            while (i <= (Math.Sqrt(c) + 1))
            {
                if ((c % i) == 0)
                {
                    lst.Add(i);
                    //printf("%ld\n",i);
                    c = c / i;
                }
                else
                    i = i + 2;
            }

            if (c > 1) lst.Add(c); // printf("%ld\n",c);
            return lst;
        }
    }
}
