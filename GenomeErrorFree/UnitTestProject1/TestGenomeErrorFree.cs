using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using GenomeErrorFree;

namespace TestGenomeErrorFree
{
    [TestClass]
    public class TestGenomeErrorFree
    {
        char[] characters = { 'a', 'c', 't', 'g' };
        Random rnd = new Random();

        [TestMethod]
        public void TestReturnGenome()
        {
            var gef = new GenomeErrorFree.GenomeErrorFree();
            var originalString = getOriginalString(50); //no argument for full string
            var cOriginalString = new CircularString(originalString);
            var segments = getStringSegments(originalString, 20, 10);//1618, 100);
            var rtrnString = gef.returnGenome(segments);
            var cRtnString = new CircularString(rtrnString);
            Assert.AreEqual(cOriginalString,cRtnString);
        }

        [TestMethod]
        public void TestHamiltonianPath()
        {
            var lengthOfSegments = 25;
            var originalString = getOriginalString(200);
            var cOriginalString = new CircularString(originalString);
            var currentLocation = 0;
            var gr = new OverlapGraph();
            var i = 0;
            var prevLocation = 0;
            StringSegment prevSeg=null;
            //break it up into strings and give each string the overlap reference
            while(currentLocation<cOriginalString.Length)
            {
                string newStr = cOriginalString.Substring(currentLocation, currentLocation + lengthOfSegments);
                StringSegment newSeg = new StringSegment(gr, newStr, i);
                i++;
                //add the overlap to every one except the first
                if (prevSeg!=null)
                {
                    prevSeg.AddOverlap(newSeg, currentLocation - prevLocation);
                }
                prevSeg = newSeg;
                //add this string segment to the graph
                gr.StringSegments.Add(newSeg);
                //increase the current location
                prevLocation = currentLocation;
                currentLocation = (currentLocation + rnd.Next(lengthOfSegments) + 1);
            }
            //add the overlap to the first segment
            gr.StringSegments[gr.StringSegments.Count - 1].AddOverlap(gr.StringSegments[0], currentLocation - cOriginalString.Length);
            //create an empty hamiltonian path
            HamiltonianPath correctHp = new HamiltonianPath(gr.StringSegments.Count);
            //sort the segments by string alphabetically
            gr.StringSegments.Sort((a, b) => a.Str.CompareTo(b.Str));
            //change the index of the string segments
            for (i = 0; i < gr.StringSegments.Count; i++)
            {
                gr.StringSegments[i].Index = i;
            }
            //change the OverlappingStringIndex to match the changed index of the overlapping string
            foreach(StringSegment seg in gr.StringSegments)
            {
                seg.LongestOverlap.OverlappingStringIndex = seg.LongestOverlap.OverlappingString.Index;
            }

            //makes the correct hamiltonian path from the overlaps
            SuffixOverlap nextOverlap;
            int nextSegmentInd = gr.StringSegments[0].Index;
            int firstSegment = nextSegmentInd;
            i = 0;
            do
            {
                nextOverlap = gr.StringSegments[nextSegmentInd].LongestOverlap;
                nextSegmentInd = nextOverlap.OverlappingStringIndex;
                correctHp.nodes[i] = new PathNode(nextSegmentInd, nextOverlap.OverlapPoint);
                i++;
            } while (nextSegmentInd!=firstSegment);

            //get the returned path
            HamiltonianPath returnedHp = new HamiltonianPath(gr);

            //this part just finds the place in the correct HP that matches the first element in the returned HP
            i = 0;
            PathNode firstCorrectNode;
            do
            {
                firstCorrectNode = correctHp.nodes[i];
                i++;
            } while (firstCorrectNode.NextString != returnedHp.nodes[0].NextString);

            //offset is the offset between the returned HP and correct HP
            int offset = i - 1;

            //now go through and compare each node
            for (i = 0; i < returnedHp.nodes.Length; i++)
            {
                var returnedNode = returnedHp.nodes[i];
                var correctNode = correctHp.nodes[(i + offset) % correctHp.nodes.Length];
                Assert.AreEqual(returnedNode, correctNode);
            }

        }

        //[TestMethod]
        public void testLiteHeap()
        {
            List<Comp> nums = new List<Comp>();
            for(int i = 0; i < 1000; i++)
            {
                nums.Add(new Comp(rnd.Next(100)));
            }
            LiteHeap<Comp> hp = new LiteHeap<Comp>(nums);
            nums.Sort();
            nums.Reverse();
            foreach(var num in nums)
            {
                Comp c = hp.getMax();
                Assert.AreEqual<Comp>(num, c);
            }

    }

        class Comp : IComparable<Comp>
        {
            int x { get; set; }
            public Comp(int x)
            {
                this.x = x;
            }

            public override int GetHashCode()
            {
                return base.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                if (!(obj is Comp))
                    return false;
                Comp c = (Comp)obj;
                return this.x==(c.x);
            }

            public int CompareTo(Comp other)
            {
                return x.CompareTo(other.x);
            }

            public override string ToString()
            {
                return this.x + "";
            }

        }

        //[TestMethod]
        public void TestFindAllOverlaps()
        {
            string str = randomString(100);
            //get the string segments with their overlap 
            List<StringSegment> correctSegs = getStringSegObjects(str, 50, 20);
            //make a new list so I can take away the overlap and add it from the
            //tested method
            List<StringSegment> returnedSegs = new List<StringSegment>();
            //this is the strings we'll pass to the method
            OverlapGraph gr = new OverlapGraph();
            foreach(StringSegment seg in correctSegs)
            {
                returnedSegs.Add(new StringSegment(gr, seg.Str, seg.Index));
                //empty the suffix overlap list so other method can make it again
            }
            foreach (StringSegment seg in returnedSegs)
            {
                //add all the overlaps in the empty segment from test method
                seg.findAllOverlaps(returnedSegs);
                //get the segment that has the correct overlaps
                StringSegment otherSeg = correctSegs[seg.Index];
                //sort them both so they will have the same order if they're the same
                foreach(var ol in otherSeg.SuffixOverlaps)
                {
                    var rtrndOl = seg.SuffixOverlaps.Find(rol=>rol.OverlappingStringIndex == ol.OverlappingStringIndex && rol.OverlapPoint==ol.OverlapPoint);
                    if (rtrndOl==null)
                    {
                        Assert.Fail("returned overlap on segment " + seg.Index + " did not contain overlap with string " +
                            ol.OverlappingStringIndex + " at point " + ol.OverlapPoint);
                    }
                    //Assert.AreEqual(ol.OverlapPoint, rtrndOl.OverlapPoint, "returned overlap on segment " + seg.Index + 
                    //    " overlap with " +ol.OverlappingStringIndex +
                    //    " did not have the same overlap point. actual was " +
                    //    ol.OverlapPoint + " returned was " + rtrndOl.OverlapPoint);
                }
            }
        }

        private string randomString(int length)
        {
            string rtrn = "";
            for(int i = 0; i < length; i++)
            {
                rtrn += randChar();
            }
            return rtrn;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originalString"></param>
        /// <param name="numberOfSegments"></param>
        /// <param name="lengthOfSegments"></param>
        /// <returns></returns>
        private List<GenomeErrorFree.StringSegment> getStringSegObjects(string originalString, int numberOfSegments, int lengthOfSegments)
        {
            //this is the index of all the absolute locations where the strings start
            int[] stringStarts = new int[numberOfSegments];
            //the current location I'll be getting the next string segment from
            int currentLocation = 0;
            //just a graph to keep the string segments in
            OverlapGraph gr = new OverlapGraph();
            //now we'll go through the string and add the string segments without overlaps
            CircularString cOriginalString = new CircularString(originalString);
            for(int i=0; i < numberOfSegments; i++)
            {
                string newStr = cOriginalString.Substring(currentLocation, currentLocation+lengthOfSegments);
                StringSegment newSeg = new StringSegment(gr, newStr, i);
                //add the location of this index
                stringStarts[i] = currentLocation;
                //add this string segment to the graph
                gr.StringSegments.Add(newSeg);
                //increase the current location
                currentLocation = (currentLocation + rnd.Next(lengthOfSegments)+1)%originalString.Length;
            }
            //now we will go through the string segment and add overlaps based on the stringStarts array
            foreach (StringSegment seg in gr.StringSegments)
            {
                int segIndex = seg.Index;
                int nextOverlapPoint = 0;
                int segmentEnds, distanceBetweenSegments;
                do
                {
                    //skip over itself
                    if (segIndex != seg.Index)
                    {
                        //add the overlap to that string segment
                        //by subtracting the location of the segment added from the
                        //location of this one
                        seg.AddOverlap(gr.StringSegments[segIndex], stringStarts[segIndex]-stringStarts[seg.Index]);
                    }
                    int segLocation = stringStarts[segIndex];
                    segmentEnds = (segLocation + lengthOfSegments) % originalString.Length;
                    segIndex = (segIndex + 1) % numberOfSegments;
                    //get the start point of the next string
                    nextOverlapPoint = stringStarts[segIndex];
                    distanceBetweenSegments = mod((nextOverlapPoint - stringStarts[seg.Index]), originalString.Length);
                } //stop when the start point of the next string is past the end of this one
                while (distanceBetweenSegments<lengthOfSegments);
            }
            return gr.StringSegments;
        }

        private static int mod(int x, int m)
        {
            int r = x % m;
            return r < 0 ? r + m : r;
        }

        private List<string> getStringSegments(string originalString, int numberOfSegments, int lengthOfSegments)
        {
            List<string> rtrn = new List<string>();
            Random r = new Random();
            if (numberOfSegments * lengthOfSegments < originalString.Length)
            {
                throw new ArgumentException("not enough strings to cover the segment");
            }
            int numberOfCoverStrings = (int)Math.Ceiling((double)originalString.Length / (double)lengthOfSegments);
            int numberOfRandomStrings = numberOfSegments - numberOfCoverStrings;
            var cString = new CircularString(originalString);
            for (var i= 0; i < numberOfCoverStrings; i++)
            {
                var start = lengthOfSegments * i;
                var end = start + lengthOfSegments;
                rtrn.Add(cString.Substring(start, end));
            }
            for(var i = 0; i < numberOfRandomStrings; i++)
            {
                var start = r.Next(cString.Length);
                var end = start + lengthOfSegments;
                rtrn.Add(cString.Substring(start, end));
            }
            return rtrn;
        }

        private string getOriginalString(int strLen = -1)
        {
            string rtrn = "gagttttatcgcttccatgacgcagaagttaacactttcggatatttctgatgagtcgaaaaattatcttgataaagcaggaattactactgcttgtttacgaattaaatcgaagtggactgctggcggaaaatgagaaaattcgacctatccttgcgcagctcgagaagctcttactttgcgacctttcgccatcaactaacgattctgtcaaaaactgacgcgttggatgaggagaagtggcttaatatgcttggcacgttcgtcaaggactggtttagatatgagtcacattttgttcatggtagagattctcttgttgacattttaaaagagcgtggattactatctgagtccgatgctgttcaaccactaataggtaagaaatcatgagtcaagttactgaacaatccgtacgtttccagaccgctttggcctctattaagctcattcaggcttctgccgttttggatttaaccgaagatgatttcgattttctgacgagtaacaaagtttggattgctactgaccgctctcgtgctcgtcgctgcgttgaggcttgcgtttatggtacgctggactttgtgggataccctcgctttcctgctcctgttgagtttattgctgccgtcattgcttattatgttcatcccgtcaacattcaaacggcctgtctcatcatggaaggcgctgaatttacggaaaacattattaatggcgtcgagcgtccggttaaagccgctgaattgttcgcgtttaccttgcgtgtacgcgcaggaaacactgacgttcttactgacgcagaagaaaacgtgcgtcaaaaattacgtgcggaaggagtgatgtaatgtctaaaggtaaaaaacgttctggcgctcgccctggtcgtccgcagccgttgcgaggtactaaaggcaagcgtaaaggcgctcgtctttggtatgtaggtggtcaacaattttaattgcaggggcttcggccccttacttgaggataaattatgtctaatattcaaactggcgccgagcgtatgccgcatgacctttcccatcttggcttccttgctggtcagattggtcgtcttattaccatttcaactactccggttatcgctggcgactccttcgagatggacgccgttggcgctctccgtctttctccattgcgtcgtggccttgctattgactctactgtagacatttttactttttatgtccctcatcgtcacgtttatggtgaacagtggattaagttcatgaaggatggtgttaatgccactcctctcccgactgttaacactactggttatattgaccatgccgcttttcttggcacgattaaccctgataccaataaaatccctaagcatttgtttcagggttatttgaatatctataacaactattttaaagcgccgtggatgcctgaccgtaccgaggctaaccctaatgagcttaatcaagatgatgctcgttatggtttccgttgctgccatctcaaaaacatttggactgctccgcttcctcctgagactgagctttctcgccaaatgacgacttctaccacatctattgacattatgggtctgcaagctgcttatgctaatttgcatactgaccaagaacgtgattacttcatgcagcgttaccatgatgttatttcttcatttggaggtaaaacctcttatgacgctgacaaccgtcctttacttgtcatgcgctctaatctctgggcatctggctatgatgttgatggaactgaccaaacgtcgttaggccagttttctggtcgtgttcaacagacctataaacattctgtgccgcgtttctttgttcctgagcatggcactatgtttactcttgcgcttgttcgttttccgcctactgcgactaaagagattcagtaccttaacgctaaaggtgctttgacttataccgatattgctggcgaccctgttttgtatggcaacttgccgccgcgtgaaatttctatgaaggatgttttccgttctggtgattcgtctaagaagtttaagattgctgagggtcagtggtatcgttatgcgccttcgtatgtttctcctgcttatcaccttcttgaaggcttcccattcattcaggaaccgccttctggtgatttgcaagaacgcgtacttattcgccaccatgattatgaccagtgtttccagtccgttcagttgttgcagtggaatagtcaggttaaatttaatgtgaccgtttatcgcaatctgccgaccactcgcgattcaatcatgacttcgtgataaaagattgagtgtgaggttataacgccgaagcggtaaaaattttaatttttgccgctgaggggttgaccaagcgaagcgcggtaggttttctgcttaggagtttaatcatgtttcagacttttatttctcgccataattcaaactttttttctgataagctggttctcacttctgttactccagcttcttcggcacctgttttacagacacctaaagctacatcgtcaacgttatattttgatagtttgacggttaatgctggtaatggtggttttcttcattgcattcagatggatacatctgtcaacgccgctaatcaggttgtttctgttggtgctgatattgcttttgatgccgaccctaaattttttgcctgtttggttcgctttgagtcttcttcggttccgactaccctcccgactgcctatgatgtttatcctttgaatggtcgccatgatggtggttattataccgtcaaggactgtgtgactattgacgtccttccccgtacgccgggcaataacgtttatgttggtttcatggtttggtctaactttaccgctactaaatgccgcggattggtttcgctgaatcaggttattaaagagattatttgtctccagccacttaagtgaggtgatttatgtttggtgctattgctggcggtattgcttctgctcttgctggtggcgccatgtctaaattgtttggaggcggtcaaaaagccgcctccggtggcattcaaggtgatgtgcttgctaccgataacaatactgtaggcatgggtgatgctggtattaaatctgccattcaaggctctaatgttcctaaccctgatgaggccgcccctagttttgtttctggtgctatggctaaagctggtaaaggacttcttgaaggtacgttgcaggctggcacttctgccgtttctgataagttgcttgatttggttggacttggtggcaagtctgccgctgataaaggaaaggatactcgtgattatcttgctgctgcatttcctgagcttaatgcttgggagcgtgctggtgctgatgcttcctctgctggtatggttgacgccggatttgagaatcaaaaagagcttactaaaatgcaactggacaatcagaaagagattgccgagatgcaaaatgagactcaaaaagagattgctggcattcagtcggcgacttcacgccagaatacgaaagaccaggtatatgcacaaaatgagatgcttgcttatcaacagaaggagtctactgctcgcgttgcgtctattatggaaaacaccaatctttccaagcaacagcaggtttccgagattatgcgccaaatgcttactcaagctcaaacggctggtcagtattttaccaatgaccaaatcaaagaaatgactcgcaaggttagtgctgaggttgacttagttcatcagcaaacgcagaatcagcggtatggctcttctcatattggcgctactgcaaaggatatttctaatgtcgtcactgatgctgcttctggtgtggttgatatttttcatggtattgataaagctgttgccgatacttggaacaatttctggaaagacggtaaagctgatggtattggctctaatttgtctaggaaataaccgtcaggattgacaccctcccaattgtatgttttcatgcctccaaatcttggaggcttttttatggttcgttcttattacccttctgaatgtcacgctgattattttgactttgagcgtatcgaggctcttaaacctgctattgaggcttgtggcatttctactctttctcaatccccaatgcttggcttccataagcagatggataaccgcatcaagctcttggaagagattctgtcttttcgtatgcagggcgttgagttcgataatggtgatatgtatgttgacggccataaggctgcttctgacgttcgtgatgagtttgtatctgttactgagaagttaatggatgaattggcacaatgctacaatgtgctcccccaacttgatattaataacactatagaccaccgccccgaaggggacgaaaaatggtttttagagaacgagaagacggttacgcagttttgccgcaagctggctgctgaacgccctcttaaggatattcgcgatgagtataattaccccaaaaagaaaggtattaaggatgagtgttcaagattgctggaggcctccactatgaaatcgcgtagaggctttgctattcagcgtttgatgaatgcaatgcgacaggctcatgctgatggttggtttatcgtttttgacactctcacgttggctgacgaccgattagaggcgttttatgataatcccaatgctttgcgtgactattttcgtgatattggtcgtatggttcttgctgccgagggtcgcaaggctaatgattcacacgccgactgctatcagtatttttgtgtgcctgagtatggtacagctaatggccgtcttcatttccatgcggtgcactttatgcggacacttcctacaggtagcgttgaccctaattttggtcgtcgggtacgcaatcgccgccagttaaatagcttgcaaaatacgtggccttatggttacagtatgcccatcgcagttcgctacacgcaggacgctttttcacgttctggttggttgtggcctgttgatgctaaaggtgagccgcttaaagctaccagttatatggctgttggtttctatgtggctaaatacgttaacaaaaagtcagatatggaccttgctgctaaaggtctaggagctaaagaatggaacaactcactaaaaaccaagctgtcgctacttcccaagaagctgttcagaatcagaatgagccgcaacttcgggatgaaaatgctcacaatgacaaatctgtccacggagtgcttaatccaacttaccaagctgggttacgacgcgacgccgttcaaccagatattgaagcagaacgcaaaaagagagatgagattgaggctgggaaaagttactgtagccgacgttttggcggcgcaacctgtgacgacaaatctgctcaaatttatgcgcgcttcgataaaaatgattggcgtatccaacctgca";
            if (strLen < 0)
            {
                return rtrn;
            }

            return rtrn.Substring(0,strLen);
        }

        private char randChar()
        {
            return characters[rnd.Next(characters.Length)];
        }
    }

    class CircularString
    {
        char[] characters;
        public int Length { get; }

        public CircularString(String Str)
        {
            characters = Str.ToCharArray();
            this.Length = characters.Length;
        }

        public char charAt(int i)
        {
            return characters[i % Length];
        }

        public string Substring(int start, int end)
        {
            var rtrn = "";
            end = end < start ? Length + end : end;
            for(int i = start; i < end; i++)
            {
                rtrn += characters[i%Length];
            }
            return rtrn;
        }

        public override string ToString()
        {
            return new string(characters);
        }

        public override bool Equals(object other)
        {
            if(!(other is CircularString))
            {
                return false;
            }

            var otherCs = (CircularString)other;
            if (otherCs.Length != Length)
            {
                return false;
            }
            var rtrn = false;
            for (int offset = 0; offset < Length; offset++)
            {
                for(int compareIndex = 0; compareIndex < otherCs.Length; compareIndex++)
                {
                    var thisChar = charAt(compareIndex);
                    var otherChar = charAt(offset + compareIndex);
                    if (thisChar != otherChar)
                    {
                        rtrn = false;
                        break;
                    } else
                    {
                        rtrn = true;
                    }
                }
                if (rtrn)
                {
                    break;
                }
            }
            return rtrn;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
