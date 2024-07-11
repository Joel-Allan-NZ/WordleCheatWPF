using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    /// <summary>
    /// Represents a possible wordle submission, along with weighted values in order to suggest the
    /// best solutions.
    /// </summary>
    public class Word
    {
        /// <summary>
        /// The actual string value of the word.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// The frequency score of letters contained in the word, ignoring multiple instances of the same letter (ie eerie only considers eri).
        /// </summary>
        public double NoRepeatLetterFrequencyScore { get; set; }

        /// <summary>
        /// The frequency score of letters contained in the word, including multiple instances of the same letter (ie eerie considers e three times).
        /// </summary>
        public double WithRepeatLetterFrequencyScore { get; set; }

        /// <summary>
        /// The frequency score of letters contained in the word, with respect to their frequency in their position.
        /// </summary>
        public double LetterPositionScore { get; set; }

        /// <summary>
        /// The total score of the word, factoring in a relevant SuggestWeight.
        /// </summary>
        public double WeightedScore { get; private set; }

        /// <summary>
        /// The frequency of the word's usage in the English language; data sourced from a rip of English Wikipedia.
        /// </summary>
        public double WordFrequency { get; set; }

        /// <summary>
        /// Calculate and set the WeightedScore of this Word, with respect to a relevant SuggestWeight.
        /// </summary>
        /// <param name="weight"></param>
        public void SetWeightedScore(SuggestWeight weight)
        {
            WeightedScore = LetterPositionScore * weight.LetterPlacementWeight + WithRepeatLetterFrequencyScore *
                    weight.DoubleLetterWeight + NoRepeatLetterFrequencyScore * weight.SingleLetterWeight + 
                    WordFrequency * weight.WordFrequency + CalculateRawLetterFrequencyScore() * weight.RawLetterFrequency;
        }

        private double CalculateRawLetterFrequencyScore()
        {
            double val = 0;
            foreach (char c in Value)
                val += SuggestWeight.EnglishFrequencies[c];
            return val;
        }

        public Word(string wordValue, int totalLetterScore, int doubleLetterScore, int wordFrequency)
        {
            Value = wordValue;
            NoRepeatLetterFrequencyScore = totalLetterScore;
            WithRepeatLetterFrequencyScore = doubleLetterScore;
            WordFrequency = wordFrequency;
        }
    }
}
