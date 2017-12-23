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
            var originalString = getOriginalString();
            var cOriginalString = new CircularString(originalString);
            var segments = getStringSegments(originalString, 1618, 100);
            var rtrnString = gef.returnGenome(segments);
            var cRtnString = new CircularString(rtrnString);
            Assert.AreEqual(cRtnString, cOriginalString);
        }

        [TestMethod]
        private void TestFindAllOverlaps()
        {
            //TODO: implement method
            string str = randomString(1000);
            List<StringSegment> segs = getStringSegObjects(str, 200, 100);
            //run the method
            //loop through and compare them
            Assert.Fail("Not implemented yet");
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
            int[] stringStarts = new int[numberOfSegments];
            int currentLocation = 0;
            OverlapGraph gr = new OverlapGraph();
            for(int i=0; i < numberOfSegments; i++)
            {
                string newStr = originalString.Substring(currentLocation, lengthOfSegments);
                StringSegment newSeg = new StringSegment(gr, newStr, i);
                stringStarts[i] = currentLocation;
                gr.StringSegments.Add(newSeg);
            }
            foreach (StringSegment seg in gr.StringSegments)
            {
                int segIndex = seg.Index;
                int nextOverlapPoint = 0;
                int segmentEnds;
                do
                {
                    if (segIndex != seg.Index)
                    {
                        seg.AddOverlap(gr.StringSegments[segIndex], stringStarts[segIndex]);
                    }
                    int segLocation = stringStarts[segIndex];
                    segmentEnds = (segLocation + lengthOfSegments) % originalString.Length;
                    segIndex = (segIndex + 1) % numberOfSegments;
                    nextOverlapPoint = stringStarts[segIndex];
                }
                while (nextOverlapPoint < segmentEnds);
            }
            return gr.StringSegments;
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
                rtrn.Add(cString.subString(start, end));
            }
            for(var i = 0; i < numberOfRandomStrings; i++)
            {
                var start = r.Next(cString.Length);
                var end = start + lengthOfSegments;
                rtrn.Add(cString.subString(start, end));
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

        public string subString(int start, int end)
        {
            var rtrn = "";
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
