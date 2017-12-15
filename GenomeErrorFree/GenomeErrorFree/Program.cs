using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenomeErrorFree
{
    class GenomeErrorFree
    {
        int inputSize = 1618; 
        static void Main(string[] args)
        {
            var gef = new GenomeErrorFree();
            gef.run();
        }
        private void run()
        {
            var input = new string[inputSize];
            for (int i = 0; i < inputSize; i++)
            {
                input[i] = Console.ReadLine();
            }


        }
    }
}
