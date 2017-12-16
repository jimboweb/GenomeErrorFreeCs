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
        /// <summary>
        /// <ol>
        /// <li>get the input</li>
        /// <li>remove all the duplicates</li>
        /// <li>gets the genome using returnGenome method</li>
        /// <li>prints it out</li>
        /// </ol>
        /// </summary>
        private void run()
        {
            var input = getInput();
            var finalString = returnGenome(input);
            Console.WriteLine(finalString);
        }

        /// <summary>
        /// returns the genome from a list of strings
        /// <ol>
        /// <li>remove all the duplicates</li>
        /// <li>build the overlap graph</li>
        /// <li>draw greedy hamiltonian path</li>
        /// <li>assembles the string from the graph and path</li>
        /// <li>returns genome string</li>
        /// </ol>
        /// </summary>
        /// <param name="input">the input strings</param>
        /// <returns>the genome</returns>
        public string returnGenome(List<string> input)
        {
            input = removeDupes(input);
            OverlapGraph gr = new OverlapGraph(input);
            HamiltonianPath hp = new HamiltonianPath(gr);
            var finalString = assembleString(gr, hp);
            return finalString;
        }
        /// <summary>
        /// gets input as list
        /// </summary>
        /// <returns></returns>
        private List<string> getInput()
        {
            var input = new List<String>(); ;
            for (int i = 0; i < inputSize; i++)
            {
                input.Add(Console.ReadLine());
            }
            return input;

        }

        private string assembleString(OverlapGraph gr, HamiltonianPath hp)
        {
            var rtrn = "";
            //TODO: assemble the string from the graph
            return rtrn;
        }

        /// <summary>
        /// gets rid of any duplicate inputs
        /// </summary>
        /// <param name="input">the original string inputs</param>
        /// <returns>the string without the inputs</returns>
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

    class HamiltonianPath
    {
        List<PathNode> nodes { get; set; }
        public HamiltonianPath()
        {
            nodes = new List<PathNode>();
        }
        public HamiltonianPath(OverlapGraph gr)
        {
            nodes = new List<PathNode>();
            //TODO: whatever I did in the other program to create the path
        }
        public void AddNode(int NextString, int OverlapLength)
        {
            nodes.Add(new PathNode(NextString, OverlapLength));
        }
    }

    class PathNode
    {
        int NextString { get; set; };
        int OverlapLength { get; set; }
        public PathNode (int NextString, int OverlapLength)
        {
            this.NextString = NextString;
            this.OverlapLength = OverlapLength;
        }
    }

    /// <summary>
    /// a graph of string segments with overlaps
    /// </summary>
    class OverlapGraph
    {
        List<StringSegment> StringSegments { get; }

        /// <summary>
        /// <ol>
        ///     <li>adds all the strings in a list of strings to StringSegments as a new StringSegment</li>
        ///     <li>then goes through and finds all the possible overlaps</li>
        /// </ol>
        /// </summary>
        /// <param name="strings">list of raw string segments</param>
        public OverlapGraph(List<String> strings)
        {
            StringSegments = new List<StringSegment>();
            foreach(var str in strings)
            {
                StringSegments.Add(new StringSegment(this, str, StringSegments.Count));
;           }

            foreach(var overlappedString in StringSegments)
            {
                overlappedString.findAllOverlaps(StringSegments);
            }
        }
    }

    class StringSegment
    {
        OverlapGraph Parent { get;  }
        public List<SuffixOverlap> SuffixOverlaps { get; set; }
        public string Str { get; }
        public int Index { get; }
        public int Length { get { return Str.Length; } }

        public StringSegment(OverlapGraph Parent, String Str, int Index)
        {
            this.Parent = Parent;
            this.Str = Str;
            this.Index = Index;
            this.SuffixOverlaps = new List<SuffixOverlap>();
         }

        public string Substring(int i)
        {
            return Str.Substring(i);
        }

        public string Substring(int start, int end)
        {
            return Str.Substring(start, end);
        }

        /// <summary>
        /// add all string segments that overlap to SuffixOverlaps
        /// </summary>
        /// <param name="segs">the list of string segments to search through</param>
        public void findAllOverlaps(List<StringSegment> segs)
        {
            foreach(var seg in segs)
            {
                if (Index != seg.Index)
                {
                    findOverlap(seg);
                }
            }
        }

        /// <summary>
        /// naively find overlaps with another string segment
        /// </summary>
        /// <param name="overlappingString">the string segment that could overlap</param>
        /// <param name="shortestOverlap">optional parameter for shortest possible overlap</param>
        public void findOverlap(StringSegment overlappingString, int shortestOverlap = 0)
        {
            int maxOverlap = Length - shortestOverlap;
            int minOverlap = Length - overlappingString.Length;
            for (var i = maxOverlap; i > minOverlap; i--)
            {
                if (Str.Substring(i).Equals(overlappingString.Substring(0, i)))
                {
                    AddOverlap(overlappingString, i);
                }
            }
        }

        /// <summary>
        /// adds an overlapping string to the suffix overlaps
        /// </summary>
        /// <param name="OverlappingString">the string that overlaps</param>
        /// <param name="OverlapPoint">the point where it overlaps</param>
        public void AddOverlap(StringSegment OverlappingString, int OverlapPoint)
        {
            SuffixOverlaps.Add(new SuffixOverlap(this, OverlappingString, OverlapPoint));
        }
    }

    /// <summary>
    /// pointer to a StringSegment that overlaps parent StringSegment
    /// </summary>
    class SuffixOverlap:IComparable<SuffixOverlap>
    {
        StringSegment Parent { get; }
        StringSegment OverlappingString { get; }
        public int OverlappingStringIndex { get;  }
        public int OverlapPoint { get;  }
        public int LengthOfOverlap { get;  }

        public SuffixOverlap(StringSegment Parent, StringSegment OverlappingString, int OverlapPoint)
        {
            this.Parent = Parent;
            this.OverlappingString = OverlappingString;
            this.OverlapPoint = OverlapPoint;
            OverlappingStringIndex = OverlappingString.Index;
            LengthOfOverlap = OverlappingString.Length - OverlapPoint;
        }

        /// <summary>
        /// gets overlapping string and overlap point as array
        /// </summary>
        /// <returns>
        /// [0] = index of overlapping string <br/>
        /// [1] = point of overlap
        /// </returns>
        public int[] GetValuesAsArray()
        {
            int[] rtrn = new int[2];
            rtrn[0] = OverlappingStringIndex;
            rtrn[1] = OverlapPoint;
            return rtrn;
        }

        /// <summary>
        /// for ordering overlaps by length of overlap descending
        /// I'll put the descending part in the LINQ
        /// </summary>
        /// <param name="other">the other SuffixOverlap</param>
        /// <returns>true if this one's overlap length is bigger</returns>
        public int CompareTo(SuffixOverlap other)
        {
            return LengthOfOverlap.CompareTo(other.LengthOfOverlap);
        }
    }
}
