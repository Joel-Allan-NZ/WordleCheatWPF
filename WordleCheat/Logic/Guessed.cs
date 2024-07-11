using System.Collections.Generic;


namespace WordleCheat.Logic
{
    /// <summary>
    /// Represents a submitted wordle guess, containing 5 guessed letters each with a character value and level of correctness.
    /// </summary>
    public class Guessed
    {
        /// <summary>
        /// The letters comprising this guess.
        /// </summary>
        public List<GuessedLetter> Letters { get; set; }

        public Guessed()
        {
            Letters = new List<GuessedLetter>();
        }

        public Guessed(string word)
        {
            Letters = new List<GuessedLetter>();

            foreach(char c in word)
            {
                Letters.Add(new GuessedLetter(c));
            }
        }
    }
}
