using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleCheat.Extensions;

namespace WordleCheat.Logic
{
    /// <summary>
    /// The active words (and appropriate frequency data) for the solver.
    /// </summary>
    public class WordSet
    {
        const int _wordLength = 5;
        public List<Word> Words { get; set; }

        public WordSet(List<Word> wordsWithBuiltFrequency)
        {
            Words = wordsWithBuiltFrequency;
        }

        /// <summary>
        /// Indices of active (ie valid) words for tracked information
        /// </summary>
        private List<int> _virtualWordIndices;

        /// <summary>
        /// The frequency of letters at specific indices for the virtually active word list.
        /// </summary>
        private Dictionary<char, int>[] _virtualLetterCountByIndex;

        /// <summary>
        /// The frequency of all letters for the virtually active word list.
        /// </summary>
        private Dictionary<char, int> _virtualTotalLetterCount;

        /// <summary>
        /// The total frequency score of all letters (without repeats) for a given virtual word index.
        /// </summary>
        private Dictionary<int, int> _virtualWordSingleLetterScore;

        /// <summary>
        /// The total frequency score of all letters (including repeats) for a given virtual word index.
        /// </summary>
        private Dictionary<int, int> _virtualWordDoubleLetterScore;

        /// <summary>
        /// The total frequency score of all letters at specific positions for a given virtual word index.
        /// </summary>
        private Dictionary<int, double> _virtualWordIndexScore;

        /// <summary>
        /// The highest frequency score (without repeats) for all virtually active words. Useful for quickly calculating the 'ratio' score.
        /// </summary>
        private double _virtualHighestSingleLetterScore;

        /// <summary>
        /// The highest frequency score (including repeats) for all virtually active words. Useful for quickly calculating the 'ratio' score.
        /// </summary>
        private double _virtualHighestDoubleLetterScore;

        /// <summary>
        /// The total sum word frequency score for all virtually active words. Useful for quickly calculating the 'ratio' score.
        /// </summary>
        private double _virtualWordFrequencyScoreSum;


        private void CullValidVirtual(Predicate<Word> wordValidityTester)
        {
            List<int> newActiveWordIndices = new();
            _virtualWordFrequencyScoreSum = 0;
            foreach (var index in _virtualWordIndices)
            {
                if (wordValidityTester(Words[index]))
                {
                    newActiveWordIndices.Add(index);
                    _virtualWordFrequencyScoreSum += Words[index].WordFrequency;
                }
                   
            }
            _virtualWordIndices = newActiveWordIndices;
        }

        /// <summary>
        /// Update the count of letter frequencies and placements for the virtual active words.
        /// </summary>
        private void BuildVirtualFrequencyData()
        {
            _virtualTotalLetterCount = new();
            _virtualLetterCountByIndex = new Dictionary<char, int>[_wordLength];

            for (int i = 0; i < _wordLength; i++)
                _virtualLetterCountByIndex[i] = new();

            foreach(var wordIndex in _virtualWordIndices)
            {
                var word = Words[wordIndex];

                for(int i =0; i< _wordLength; i++)
                {
                    char currentChar = word.Value[i];

                    _virtualTotalLetterCount.AddOrSumValue(currentChar, 1);

                    _virtualLetterCountByIndex[i].AddOrSumValue(currentChar, 1);

                }
            }
        }

        /// <summary>
        /// Update the word specific letter counts and placement6s for the virtual active words.
        /// </summary>
        private void BuildVirtualFrequencyScores()
        {
            _virtualWordDoubleLetterScore = new Dictionary<int, int>();
            _virtualWordSingleLetterScore = new Dictionary<int, int>();
            _virtualWordIndexScore = new Dictionary<int, double>();

            _virtualHighestSingleLetterScore = 0;
            _virtualHighestDoubleLetterScore = 0;

            HashSet<char> noRepeats = new();
            foreach (var index in _virtualWordIndices)
            {
                var word = Words[index];

                noRepeats.Clear();
                var singleVal = 0;
                var doubleVal = 0;
                var indexScore = 0.0;

                for(int i =0; i<_wordLength; i++)
                {
                    noRepeats.Add(word.Value[i]);
                    doubleVal += _virtualTotalLetterCount[word.Value[i]];
                    indexScore += 0.2 * _virtualLetterCountByIndex[i][word.Value[i]] / _virtualWordIndices.Count;
                }

                foreach (char c in noRepeats)
                    singleVal += _virtualTotalLetterCount[c];

                _virtualHighestDoubleLetterScore = Math.Max(_virtualHighestDoubleLetterScore, doubleVal);
                _virtualHighestSingleLetterScore = Math.Max(_virtualHighestSingleLetterScore, singleVal);

                _virtualWordSingleLetterScore[index] = singleVal;
                _virtualWordDoubleLetterScore[index] = doubleVal;
                _virtualWordIndexScore[index] = indexScore;
            }
        }

        public void NewWord()
        {
            //reset virtual to default
            _virtualWordIndices = new List<int>();
        }

        /// <summary>
        /// Remove all invalid words from the currently active words.
        /// </summary>
        /// <param name="solver"></param>
        public void CullInvalid(Predicate<Word> wordValidityTester)
        {
            if (_virtualWordIndices == null || _virtualWordIndices.Count == 0)
            {
                List<int> newActiveWordIndices = new();

                for (int index = 0; index < Words.Count; index++)
                {
                    if (wordValidityTester(Words[index]))
                    {
                        newActiveWordIndices.Add(index);
                        _virtualWordFrequencyScoreSum += Words[index].WordFrequency;
                    }


                }
                _virtualWordIndices = newActiveWordIndices;
            }
            else
                CullValidVirtual(wordValidityTester);

            BuildVirtualFrequencyData();
            BuildVirtualFrequencyScores();
        }

        public List<SuggestionWord> GetWordSuggestions(SuggestWeight weight)
        {
            LinkedList<Tuple<double, int>> top = new(); //clunky looking but should be efficient.
            top.AddFirst(Tuple.Create(-1.0, -1));
            LinkedListNode<Tuple<double, int>> current;
            if (_virtualWordIndices == null || _virtualWordIndices.Count == 0)
            {
                for (int index = 0; index < Words.Count; index++)
                {
                    Words[index].SetWeightedScore(weight);
                    var wordScore = Words[index].WeightedScore;
                    current = top.First;
                    for (int i = 0; i < 3; i++)
                    {
                        if (wordScore > current.Value.Item1)
                        {
                            top.AddBefore(current, Tuple.Create(wordScore, index));
                            break;
                        }
                        current = current.Next;
                    }
                }
            }
            else
            {
                foreach (var index in _virtualWordIndices)
                {
                    var wordScore = GetVirtualWordScore(weight, index);
                    current = top.First;
                    for (int i = 0; i < 3; i++)
                    {
                        if (wordScore > current.Value.Item1)
                        {
                            top.AddBefore(current, Tuple.Create(wordScore, index));
                            break;
                        }
                        current = current.Next;
                    }
                }
            }

            List<SuggestionWord> results = new();
            current = top.First;
            for(int i =0; i<3; i++)
            {
                if (current.Value.Item1 > -1)
                    results.Add(new SuggestionWord(Words[current.Value.Item2].Value, current.Value.Item1));
                else
                    break;

                current = current.Next;
            }
            return results;
        }

        private double GetVirtualWordScore(SuggestWeight weight, int index)
        {
            double virtualDoubleScore = _virtualWordDoubleLetterScore[index] / _virtualHighestDoubleLetterScore;
            double virtualSingleScore = _virtualWordSingleLetterScore[index] / _virtualHighestSingleLetterScore;
            double virtualWordFrequencyScore = Words[index].WordFrequency / _virtualWordFrequencyScoreSum;
            double rawFrequencyScore = 0.0;
            foreach (char c in Words[index].Value)
                rawFrequencyScore += SuggestWeight.EnglishFrequencies[c];

            return weight.DoubleLetterWeight * virtualDoubleScore + weight.SingleLetterWeight * virtualSingleScore 
                + weight.LetterPlacementWeight * _virtualWordIndexScore[index] + virtualWordFrequencyScore * weight.WordFrequency
                + weight.RawLetterFrequency * rawFrequencyScore;
        }

    }
}
