using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleCheat.Logic;

namespace WordleCheatWPF
{
    public class GuessedVM
    {
        private Guessed _guessed;

        public Guessed Guessed
        {
            get => _guessed;
        }

        public List<GuessedLetterVM> Letters { get; set; }
        public GuessedVM(string word)
        {
            Letters = new List<GuessedLetterVM>();
            _guessed = new Guessed(word);

            foreach(var letter in _guessed.Letters)
            {
                Letters.Add(new GuessedLetterVM(letter));
            }
        }
    }
}
