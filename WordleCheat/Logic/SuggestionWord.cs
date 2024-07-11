using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    /// <summary>
    /// A possible solution to the current wordle problem, complete with a percentage representing its relative value as a guess.
    /// </summary>
    public class SuggestionWord
    {
        /// <summary>
        /// The word being suggested.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// A string representation of the relative value of this suggestion when compared to other suggestions.
        /// </summary>
        public string PercentageString { get => " "+Math.Min(1.0,_percent).ToString("P"); }

        private double _percent;

        public SuggestionWord(string value, double wordScore)
        {
            _percent = wordScore;
            Value = value;
        }
    }
}
