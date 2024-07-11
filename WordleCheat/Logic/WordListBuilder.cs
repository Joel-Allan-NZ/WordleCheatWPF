using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    public class WordListBuilder
    {
        const int _wordLength = 5;
        /// <summary>
        /// Read the list of possible words, building basic frequency tables.
        /// </summary>
        /// <returns></returns>
        public static List<Word> ReadWords()
        {
            List<Word> words = new();
            var path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            var wordDir = @"D:\Files From C\Old C Repos\WordleCheatWPF\WordleCheatWPF\words\WordleWords.json";/*Path.Combine(path, @"Wordle\WordLists\WordleWords2.json");*/
            var read = File.ReadAllText(wordDir);
            var wordsRead = (Dictionary<string, string>[])System.Text.Json.JsonSerializer.Deserialize(read, typeof(Dictionary<string, string>[]));
            //var wordsRead = (List<string>)System.Text.Json.JsonSerializer.Deserialize(read, typeof(List<string>));

            words = wordsRead.Select(x => new Word(x.Keys.First().ToUpper(), 0, 0, int.Parse(x[x.Keys.First()]))).ToList();

            RebuildFrequencyScores(words);
            

            return words;
        }

        public static void RebuildFrequencyScores(List<Word> words)
        {
            Dictionary<char, int> totalCount = new();
            Dictionary<char, int>[] countByIndex = new Dictionary<char, int>[_wordLength];
            for (int i = 0; i < _wordLength; i++)
                countByIndex[i] = new Dictionary<char, int>();

            

            foreach(var word in words)
            {
                for(int i =0; i<_wordLength; i++)
                {
                    char currentChar = word.Value[i];

                    if (totalCount.ContainsKey(currentChar))
                        totalCount[currentChar]++;
                    else
                        totalCount[currentChar] = 1;

                    if (countByIndex[i].ContainsKey(currentChar))
                        countByIndex[i][currentChar]++;
                    else
                        countByIndex[i][currentChar] = 1;
                }
            }

            double[] sumByIndex = new double[5];
            for(int i=0;i<_wordLength; i++)
            {
                sumByIndex[i] = countByIndex[i].Sum(x => x.Value);
            }

            double maxDouble = 0.0, maxSingle = 0.0;

            foreach (var word in words)
            {
                HashSet<char> noRepeats = new();
                var val = 0;
                var valWithDoubles = 0;
                var indexScore = 0.0;
                
                for(int i=0; i<_wordLength; i++)
                {
                    noRepeats.Add(word.Value[i]);
                    valWithDoubles += totalCount[word.Value[i]];
                    indexScore += 0.2*countByIndex[i][word.Value[i]] / sumByIndex[i];

                }
                foreach (char c in noRepeats)
                    val += totalCount[c];

                word.LetterPositionScore = indexScore;
                word.WithRepeatLetterFrequencyScore = valWithDoubles;
                word.NoRepeatLetterFrequencyScore = val;

                maxDouble = Math.Max(maxDouble, word.WithRepeatLetterFrequencyScore);
                maxSingle = Math.Max(maxSingle, word.NoRepeatLetterFrequencyScore);
               
            }

            foreach(var word in words)
            {
                word.WithRepeatLetterFrequencyScore /= maxDouble;
                word.NoRepeatLetterFrequencyScore /= maxSingle;
            }
        }
    }
}
