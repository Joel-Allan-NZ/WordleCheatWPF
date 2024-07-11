using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    /// <summary>
    /// Represents a single letter in a wordle guess, complete with a character value and level of correctness.
    /// </summary>
    public class GuessedLetter
    {
        private LetterGuessCorrectness _correctness;

        /// <summary>
        /// The correctness of the letter; if it's in the correct place, if it exists in the word etc.
        /// </summary>
        public LetterGuessCorrectness Correctness
        {
            get => _correctness;
            set
            {
                _correctness = value;
            }
        }

        /// <summary>
        /// The character value of this wordle 'letter'.
        /// </summary>
        public char GuessedValue
        {
            get => _value;
            set
            {
                _value = value;
            }
        }

        private char _value;

        public GuessedLetter(char guessed)
        {
            GuessedValue = guessed;
        }
    }
}
