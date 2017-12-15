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
            var input = getInput();
        }

        private List<string> getInput()
        {
            var input = new List<String>(); ;
            for (int i = 0; i < inputSize; i++)
            {
                input.Add(Console.ReadLine());
            }
            return input;

        }

        private List<String> removeDupes(List<String> input)
        {
            input.Sort();
            var output = new List<string>();
            var currentInput = "";
            foreach(var str in input)
            {
                if (!str.Equals(currentInput))
                {
                    output.Add(str);
                    currentInput = str;
                }
            }
            return output;
        }

    }
}
