using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WordleCheat.Logic;

namespace WordleCheatWPF
{
    /// <summary>
    /// MVVM wrapper for GuessedLetter, implementing INPC for clean binding without polluting model.
    /// </summary>
    public class GuessedLetterVM : INotifyPropertyChanged
    {
        private GuessedLetter _guessedLetter;

        public LetterGuessCorrectness Correctness
        {
            get => _guessedLetter.Correctness;
            set
            {
                _guessedLetter.Correctness = value;
                OnPropertyChanged();
            }
        }

        public string GuessedValue
        {
            get => _guessedLetter.GuessedValue.ToString();
            set
            {
                _guessedLetter.GuessedValue = value.First();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public GuessedLetterVM(GuessedLetter guessedLetter)
        {
            _guessedLetter = guessedLetter;
        }

    }
}
