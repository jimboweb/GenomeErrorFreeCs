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

        //[TestMethod]
        public void TestReturnGenome()
        {
            var gef = new GenomeErrorFree.GenomeErrorFree();
            var originalString = getOriginalString();
            var cOriginalString = new CircularString(originalString);
            var segments = getStringSegments(originalString, 1618, 100);
            var rtrnString = gef.returnGenome(segments);
            var cRtnString = new CircularString(rtrnString);
            Assert.AreEqual(cRtnString, cOriginalString);
        }

        [TestMethod]
        public void TestFindAllOverlaps()
        {
            //TODO: implement method
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
                seg.SuffixOverlaps.Sort();
                otherSeg.SuffixOverlaps.Sort();
                //TODO: this isn't going to work because there will be more
                //overlaps than the ones I put there. and they won't necessarily
                //be in the same place even if I sort them. so I need a better
                //way to check.
                for(int i = 0; i < otherSeg.SuffixOverlaps.Count; i++)
                {
                    Assert.AreEqual(seg.SuffixOverlaps[i], otherSeg.SuffixOverlaps[i], getTestAllOverlapsFailString(seg,otherSeg,i));
                }
            }
        }

        /// <summary>
        /// the fail string for the testGetAllOverlaps method
        /// </summary>
        /// <param name="seg"></param>
        /// <param name="otherSeg"></param>
        /// <returns></returns>
        private string getTestAllOverlapsFailString(StringSegment seg, StringSegment otherSeg, int i)
        {
            return "failed test at StringSegment " + seg.Index + "\n" +
                                        " suffix overlap " + i + " expected string segment \n" +
                                        seg.SuffixOverlaps[i].OverlappingStringIndex + " at index\n" +
                                        seg.SuffixOverlaps[i].OverlapPoint + " but got string segment \n" +
                                        otherSeg.SuffixOverlaps[i].OverlappingStringIndex + " at index " +
                                        otherSeg.SuffixOverlaps[i].OverlapPoint;
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
                currentLocation = (currentLocation + rnd.Next(lengthOfSegments))%originalString.Length;
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

        private string getOriginalString()
        {
            return "gagttttatcgcttccatgacgcagaagttaacactttcggatatttctgatgagtcgaaaaattatcttgataaagcaggaattactactgcttgtttacgaattaaatcgaagtggactgctggcggaaaatgagaaaattcgacctatccttgcgcagctcgagaagctcttactttgcgacctttcgccatcaactaacgattctgtcaaaaactgacgcgttggatgaggagaagtggcttaatatgcttggcacgttcgtcaaggactggtttagatatgagtcacattttgttcatggtagagattctcttgttgacattttaaaagagcgtggattactatctgagtccgatgctgttcaaccactaataggtaagaaatcatgagtcaagttactgaacaatccgtacgtttccagaccgctttggcctctattaagctcattcaggcttctgccgttttggatttaaccgaagatgatttcgattttctgacgagtaacaaagtttggattgctactgaccgctctcgtgctcgtcgctgcgttgaggcttgcgtttatggtacgctggactttgtgggataccctcgctttcctgctcctgttgagtttattgctgccgtcattgcttattatgttcatcccgtcaacattcaaacggcctgtctcatcatggaaggcgctgaatttacggaaaacattattaatggcgtcgagcgtccggttaaagccgctgaattgttcgcgtttaccttgcgtgtacgcgcaggaaacactgacgttcttactgacgcagaagaaaacgtgcgtcaaaaattacgtgcggaaggagtgatgtaatgtctaaaggtaaaaaacgttctggcgctcgccctggtcgtccgcagccgttgcgaggtactaaaggcaagcgtaaaggcgctcgtctttggtatgtaggtggtcaacaattttaattgcaggggcttcggccccttacttgaggataaattatgtctaatattcaaactggcgccgagcgtatgccgcatgacctttcccatcttggcttccttgctggtcagattggtcgtcttattaccatttcaactactccggttatcgctggcgactccttcgagatggacgccgttggcgctctccgtctttctccattgcgtcgtggccttgctattgactctactgtagacatttttactttttatgtccctcatcgtcacgtttatggtgaacagtggattaagttcatgaaggatggtgttaatgccactcctctcccgactgttaacactactggttatattgaccatgccgcttttcttggcacgattaaccctgataccaataaaatccctaagcatttgtttcagggttatttgaatatctataacaactattttaaagcgccgtggatgcctgaccgtaccgaggctaaccctaatgagcttaatcaagatgatgctcgttatggtttccgttgctgccatctcaaaaacatttggactgctccgcttcctcctgagactgagctttctcgccaaatgacgacttctaccacatctattgacattatgggtctgcaagctgcttatgctaatttgcatactgaccaagaacgtgattacttcatgcagcgttaccatgatgttatttcttcatttggaggtaaaacctcttatgacgctgacaaccgtcctttacttgtcatgcgctctaatctctgggcatctggctatgatgttgatggaactgaccaaacgtcgttaggccagttttctggtcgtgttcaacagacctataaacattctgtgccgcgtttctttgttcctgagcatggcactatgtttactcttgcgcttgttcgttttccgcctactgcgactaaagagattcagtaccttaacgctaaaggtgctttgacttataccgatattgctggcgaccctgttttgtatggcaacttgccgccgcgtgaaatttctatgaaggatgttttccgttctggtgattcgtctaagaagtttaagattgctgagggtcagtggtatcgttatgcgccttcgtatgtttctcctgcttatcaccttcttgaaggcttcccattcattcaggaaccgccttctggtgatttgcaagaacgcgtacttattcgccaccatgattatgaccagtgtttccagtccgttcagttgttgcagtggaatagtcaggttaaatttaatgtgaccgtttatcgcaatctgccgaccactcgcgattcaatcatgacttcgtgataaaagattgagtgtgaggttataacgccgaagcggtaaaaattttaatttttgccgctgaggggttgaccaagcgaagcgcggtaggttttctgcttaggagtttaatcatgtttcagacttttatttctcgccataattcaaactttttttctgataagctggttctcacttctgttactccagcttcttcggcacctgttttacagacacctaaagctacatcgtcaacgttatattttgatagtttgacggttaatgctggtaatggtggttttcttcattgcattcagatggatacatctgtcaacgccgctaatcaggttgtttctgttggtgctgatattgcttttgatgccgaccctaaattttttgcctgtttggttcgctttgagtcttcttcggttccgactaccctcccgactgcctatgatgtttatcctttgaatggtcgccatgatggtggttattataccgtcaaggactgtgtgactattgacgtccttccccgtacgccgggcaataacgtttatgttggtttcatggtttggtctaactttaccgctactaaatgccgcggattggtttcgctgaatcaggttattaaagagattatttgtctccagccacttaagtgaggtgatttatgtttggtgctattgctggcggtattgcttctgctcttgctggtggcgccatgtctaaattgtttggaggcggtcaaaaagccgcctccggtggcattcaaggtgatgtgcttgctaccgataacaatactgtaggcatgggtgatgctggtattaaatctgccattcaaggctctaatgttcctaaccctgatgaggccgcccctagttttgtttctggtgctatggctaaagctggtaaaggacttcttgaaggtacgttgcaggctggcacttctgccgtttctgataagttgcttgatttggttggacttggtggcaagtctgccgctgataaaggaaaggatactcgtgattatcttgctgctgcatttcctgagcttaatgcttgggagcgtgctggtgctgatgcttcctctgctggtatggttgacgccggatttgagaatcaaaaagagcttactaaaatgcaactggacaatcagaaagagattgccgagatgcaaaatgagactcaaaaagagattgctggcattcagtcggcgacttcacgccagaatacgaaagaccaggtatatgcacaaaatgagatgcttgcttatcaacagaaggagtctactgctcgcgttgcgtctattatggaaaacaccaatctttccaagcaacagcaggtttccgagattatgcgccaaatgcttactcaagctcaaacggctggtcagtattttaccaatgaccaaatcaaagaaatgactcgcaaggttagtgctgaggttgacttagttcatcagcaaacgcagaatcagcggtatggctcttctcatattggcgctactgcaaaggatatttctaatgtcgtcactgatgctgcttctggtgtggttgatatttttcatggtattgataaagctgttgccgatacttggaacaatttctggaaagacggtaaagctgatggtattggctctaatttgtctaggaaataaccgtcaggattgacaccctcccaattgtatgttttcatgcctccaaatcttggaggcttttttatggttcgttcttattacccttctgaatgtcacgctgattattttgactttgagcgtatcgaggctcttaaacctgctattgaggcttgtggcatttctactctttctcaatccccaatgcttggcttccataagcagatggataaccgcatcaagctcttggaagagattctgtcttttcgtatgcagggcgttgagttcgataatggtgatatgtatgttgacggccataaggctgcttctgacgttcgtgatgagtttgtatctgttactgagaagttaatggatgaattggcacaatgctacaatgtgctcccccaacttgatattaataacactatagaccaccgccccgaaggggacgaaaaatggtttttagagaacgagaagacggttacgcagttttgccgcaagctggctgctgaacgccctcttaaggatattcgcgatgagtataattaccccaaaaagaaaggtattaaggatgagtgttcaagattgctggaggcctccactatgaaatcgcgtagaggctttgctattcagcgtttgatgaatgcaatgcgacaggctcatgctgatggttggtttatcgtttttgacactctcacgttggctgacgaccgattagaggcgttttatgataatcccaatgctttgcgtgactattttcgtgatattggtcgtatggttcttgctgccgagggtcgcaaggctaatgattcacacgccgactgctatcagtatttttgtgtgcctgagtatggtacagctaatggccgtcttcatttccatgcggtgcactttatgcggacacttcctacaggtagcgttgaccctaattttggtcgtcgggtacgcaatcgccgccagttaaatagcttgcaaaatacgtggccttatggttacagtatgcccatcgcagttcgctacacgcaggacgctttttcacgttctggttggttgtggcctgttgatgctaaaggtgagccgcttaaagctaccagttatatggctgttggtttctatgtggctaaatacgttaacaaaaagtcagatatggaccttgctgctaaaggtctaggagctaaagaatggaacaactcactaaaaaccaagctgtcgctacttcccaagaagctgttcagaatcagaatgagccgcaacttcgggatgaaaatgctcacaatgacaaatctgtccacggagtgcttaatccaacttaccaagctgggttacgacgcgacgccgttcaaccagatattgaagcagaacgcaaaaagagagatgagattgaggctgggaaaagttactgtagccgacgttttggcggcgcaacctgtgacgacaaatctgctcaaatttatgcgcgcttcgataaaaatgattggcgtatccaacctgca";
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
