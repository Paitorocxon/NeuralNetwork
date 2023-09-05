/*
   SPLAI_2
   Written by Fabian Müller
   Copyright © 2023 Fabian Müller

   Permission is hereby granted, free of charge, to any person obtaining a copy of this software
   and associated documentation files (the "Software"), to deal in the Software without restriction,
   including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense,
   and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so,
   subject to the following conditions:

   The above copyright notice and this permission notice shall be included in all copies or
   substantial portions of the Software.

   Permission is hereby also granted to use the Software, with the requirement that the original author,
   Fabian Müller, is appropriately credited and acknowledged in any derivative work or application.

   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
   INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
   IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
   WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE
   OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPLAI_2
{
    internal class WordProcessing
    {

        public static string mostPossibleWord(string[] knownWords, string targetWord)
        {
            string resultingWord = targetWord;
            foreach (string word in knownWords)
            {
                int distance = LevenshteinDistance(word, targetWord);

                if (distance < 0.3)
                {
                    resultingWord = word;
                }
                else if(distance < 0.1)
                {
                    resultingWord = word;
                    break;
                }
            }
            return resultingWord;
        }

        static int LevenshteinDistance(string word1, string word2)
        {
            int[,] dp = new int[word1.Length + 1, word2.Length + 1];

            for (int i = 0; i <= word1.Length; i++)
            {
                for (int j = 0; j <= word2.Length; j++)
                {
                    if (i == 0)
                        dp[i, j] = j;
                    else if (j == 0)
                        dp[i, j] = i;
                    else if (word1[i - 1] == word2[j - 1])
                        dp[i, j] = dp[i - 1, j - 1];
                    else
                        dp[i, j] = 1 + Math.Min(Math.Min(dp[i - 1, j], dp[i, j - 1]), dp[i - 1, j - 1]);
                }
            }

            return dp[word1.Length, word2.Length];
        }


        static Random random = new Random();

        public static string ShuffleWords(string input, string[] wordArray)
        {
            // Teile den Eingabe-String in Worte auf
            string[] words = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // Mische die Worte zufällig
            string[] shuffledWords = words.OrderBy(w => random.Next()).ToArray();

            // Erstelle einen neuen String, indem die gemischten Worte wieder zusammengesetzt werden
            string shuffledString = string.Join(" ", shuffledWords);

            return shuffledString;
        }

        public static string FindBestMatch(string targetWord, string[] wordArray, double threshold)
        {
            string bestMatch = targetWord;
            int bestDistance = int.MaxValue;

            foreach (string word in wordArray)
            {
                int distance = LevenshteinDistance(word, targetWord);
                double similarity = 1.0 - (double)distance / Math.Max(targetWord.Length, word.Length);

                if (similarity >= threshold && distance < bestDistance)
                {
                    bestDistance = distance;
                    bestMatch = word;
                }
            }

            return bestMatch;
        }

        public static string[] CleanTrainfiles(string Trianfilepath)
        {
            string[] backString = File.ReadAllLines(Trianfilepath);
            string path = @"C:\Users\Pait\Desktop\model\";
            List<string> words = new List<string>(File.ReadAllLines(path + "FILTERFILE.txt"));
            var sortedWords = words.OrderByDescending(word => word.Length);
            for(int i = 0; i < backString.Length; i++)
            {
                foreach (string filterword in sortedWords)
                {
                    backString[i] = backString[i].Replace(filterword, "");
                    backString[i] = backString[i].Replace("?", "").Replace("!", "").Replace(".", "");
                }
                Console.Title = backString[i];
            }

            return backString;
        }

    }
}
