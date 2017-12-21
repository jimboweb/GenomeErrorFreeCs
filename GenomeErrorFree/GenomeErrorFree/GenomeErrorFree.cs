using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//TODO: write the tests
namespace GenomeErrorFree
{
    public class GenomeErrorFree
    {
        int inputSize = 1618; 
        static void Main(string[] args)
        {
            var gef = new GenomeErrorFree();
            gef.run();
        }

        public GenomeErrorFree()
        {

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
            var wholeStringOverlap = 0;
            foreach(var node in hp.nodes)
            {
                var overlappingString = gr.StringSegments[node.NextString].Str;
                wholeStringOverlap += node.overlapPoint;
                combineString(rtrn, overlappingString, wholeStringOverlap);
            }
            return rtrn;
        }

        private string combineString(string overlappedString, string overlappingString, int overlapPoint)
        {
            if (overlappedString.Substring(overlapPoint).Equals(overlappingString.Substring(0, overlapPoint))){
                return overlappedString.Substring(0,overlapPoint) + overlappingString;
            }

            throw new ArgumentException("string " + overlappedString + " is not overlapped by " + overlappingString + " at point " + overlapPoint);
            
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
        public PathNode[] nodes { get; set; }
        public HamiltonianPath(OverlapGraph gr)
        {
            int numberOfNodes = gr.StringSegments.Count;
            nodes = new PathNode[numberOfNodes];
            LiteHeap<StringSegment> segHeap = new LiteHeap<StringSegment>(gr.StringSegments);
            bool[] usedNodes = new bool[numberOfNodes];
           while (!segHeap.isEmpty())
            {
                StringSegment nextSeg = (StringSegment)segHeap.getMax();
                PathNode currentNode = nodes[nextSeg.Index];
                SuffixOverlap nextOverlap = getNextUnusedOverlap(nextSeg, usedNodes);
                currentNode = new PathNode(nextOverlap.OverlappingStringIndex, nextOverlap.OverlapPoint);
            }
        }

        private SuffixOverlap getNextUnusedOverlap(StringSegment nextSeg, bool[] usedNodes)
        {
            int nextNode;
            SuffixOverlap nextOverlap;
            LiteHeap<SuffixOverlap> olHeap = new LiteHeap<SuffixOverlap>(nextSeg.SuffixOverlaps);
            do
            {
                nextOverlap = (SuffixOverlap)olHeap.getMax();
                nextNode = nextOverlap.OverlappingStringIndex;
            } while (!usedNodes[nextNode]);
            usedNodes[nextNode] = true;
            return nextOverlap;
        }

    }

    class PathNode
    {
        public int NextString { get; set; }
        public int overlapPoint { get; set; }
        public PathNode (int NextString, int OverlapLength)
        {
            this.NextString = NextString;
            this.overlapPoint = OverlapLength;
        }
    }

    /// <summary>
    /// a graph of string segments with overlaps
    /// </summary>
    class OverlapGraph
    {
        public List<StringSegment> StringSegments { get; }

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

    class StringSegment:IComparable<StringSegment>
    {
        OverlapGraph Parent { get;  }
        public List<SuffixOverlap> SuffixOverlaps { get; set; }
        public SuffixOverlap LongestOverlap { get; private set; }
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
            var newOverlap = new SuffixOverlap(this, OverlappingString, OverlapPoint);
            SuffixOverlaps.Add(newOverlap);
            if (LongestOverlap==null || newOverlap.LengthOfOverlap > LongestOverlap.LengthOfOverlap)
            {
                LongestOverlap = newOverlap;
            }
        }

        /// <summary>
        /// compares by length of longest overlap
        /// </summary>
        /// <param name="other">other String Segment</param>
        /// <returns>comprison of longest overlaps</returns>
        public int CompareTo(StringSegment other)
        {
            return LongestOverlap.LengthOfOverlap.CompareTo(other.LongestOverlap.LengthOfOverlap);
        }
    }

    /// <summary>
    /// pointer to a StringSegment that overlaps parent StringSegment
    /// </summary>
    class SuffixOverlap:IComparable<SuffixOverlap>
    {
        StringSegment Parent { get; }
        public StringSegment OverlappingString { get; }
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

    /// <summary>
    /// very light heap class of comparables. 
    /// can only build heap from list of segments and 
    /// get max. no public insert method.
    /// </summary>
    class LiteHeap<T> where T:IComparable
    {
        T[] HeapArray;
        int HeapSize = 0;
        public LiteHeap(List<T> vals)
        {
            HeapArray = new T[vals.Count()];
            foreach(var val in vals)
            {
                insert(val);
            }
        }

        /// <summary>
        /// gets maximum or null if empty
        /// </summary>
        /// <returns></returns>
        public IComparable getMax()
        {
            if (isEmpty())
                return null;
            var rtrn = HeapArray[0];
            HeapArray[0] = HeapArray[HeapSize];
            HeapArray[HeapSize] = default(T) ;
            HeapSize--;
            shiftDown(0);
            return rtrn;
        }

        public bool isEmpty()
        {
            return HeapSize < 1;
        }

        private void insert(T val)
        {
            if (HeapSize == HeapArray.Length)
                throw new IndexOutOfRangeException("can't insert value");
            HeapArray[HeapSize] = val;
            shiftUp(HeapSize);
            HeapSize++;
        }

        private int parent(int ind)
        {
            return ind == 0 ? -1 : ind / 2;
        }

        private int leftOrRightChild(int ind, int leftOrRight)
        {
            int childIndex = leftOrRight==0? 2 * ind : 2 * ind + 1;
            return childIndex < HeapSize ? childIndex : -1;
        }
        
        private int[] children(int ind)
        {
            int[] children = new int[2];
            for (int i = 0; i< children.Length;i++)
            {
                children[i] = leftOrRightChild(ind, i);
            }
            return children;
        }

        private void swap(int a, int b)
        {
            var temp = HeapArray[a];
            HeapArray[a] = HeapArray[b];
            HeapArray[b] = HeapArray[a];
        }

        private void shiftUp(int i)
        {
            int p = parent(i);
            if (p != -1)
            {
                if (HeapArray[i].CompareTo(HeapArray[p]) == 1)
                {
                    swap(p, i);
                    shiftUp(p);
                }
            }
        }

        private void shiftDown(int i)
        {
            var leftIndex = leftOrRightChild(i, 0);
            var rightIndex = leftOrRightChild(i, 1);
            int largest = i;
            largest = (leftIndex != -1 && HeapArray[leftIndex].CompareTo(HeapArray[largest]) == 1) ? leftIndex : largest;
            largest = (rightIndex != -1 && HeapArray[rightIndex].CompareTo(HeapArray[largest]) == 1) ? rightIndex : largest;
            if (largest != i)
            {
                swap(i, largest);
                shiftDown(largest);
            }
        }
    }
}
