using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WordleCheat.Logic;

namespace WordleCheatWPF
{
    /// <summary>
    /// Primary (and only!) view model
    /// </summary>
    public class MainVM : INotifyPropertyChanged
    {
        /// <summary>
        /// INPC implementation
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// INPC implementation
        /// </summary>
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private ICollection<SuggestionWord> _suggestions;

        /// <summary>
        /// Words that are suggested as possible solutions.
        /// </summary>
        public ICollection<SuggestionWord> Suggestions
        {
            get => _suggestions;
            set
            {
                _suggestions = value;
                OnPropertyChanged();
                ClickSuggest.RaiseCanExecuteChanged();
            }
        }

        private string _guess;

        /// <summary>
        /// Currrent unsubmitted guess word. IE what you could be typing as a possible answer.
        /// </summary>
        public string Guess
        {
            get => _guess;
            set
            {
                _guess = value;
                OnPropertyChanged();
            }
        }

        private WordleSolver _solver;


        private readonly DelegateCommand<string> _deleteKeyCommand;
        /// <summary>
        /// Effectively backspace for the current Guess, command for binding to global keyboard reading behavior
        /// </summary>
        public DelegateCommand<string> DeleteKeyCommand
        {
            get { return _deleteKeyCommand; }
        }


        private readonly DelegateCommand<string> _typeKeyCommand;
        /// <summary>
        /// Adds a letter to the current guess, for use with commandbinding to global keyboard reading behavior.
        /// </summary>
        public DelegateCommand<string> TypeKeyCommand
        {
            get { return _typeKeyCommand; }
        }

        private readonly DelegateCommand<GuessedLetterVM> _clickCommand;

        /// <summary>
        /// Command for cycling guessed letter correctness.
        /// </summary>
        public DelegateCommand<GuessedLetterVM> Click
        {
            get { return _clickCommand; }
        }

        private readonly DelegateCommand<string> _resetCommand;

        /// <summary>
        /// Command for restarting guesses/beginning new word.
        /// </summary>
        public DelegateCommand<string> ResetCommand
        {
            get => _resetCommand;
        }


        private readonly DelegateCommand<string> _shareCommand;
        /// <summary>
        /// Command to copy solution to clipboard.
        /// </summary>
        public DelegateCommand<string> ShareCommand
        {
            get => _shareCommand;
        }

        private readonly DelegateCommand<SuggestionWord> _clickSuggest;
        /// <summary>
        /// Command to select and submit a suggested solutions.
        /// </summary>
        public DelegateCommand<SuggestionWord> ClickSuggest
        {
            get => _clickSuggest;
        }


        private readonly DelegateCommand<string> _guessCommand;
        /// <summary>
        /// Command to submit a keyboard-typed guess
        /// </summary>
        public DelegateCommand<string> GuessCommand
        {
            get => _guessCommand;
        }


        private readonly DelegateCommand<string> _suggestionCommand;

        /// <summary>
        /// Command to generate suggested solutions
        /// </summary>
        public DelegateCommand<string> SuggestionCommand
        {
            get => _suggestionCommand;
        }

        private readonly DelegateCommand<GuessedLetterVM> _rightClickCommand;
        /// <summary>
        /// Command to cycle correctness on a guessed letter when right-clicked (ie reverse direction).
        /// </summary>
        public DelegateCommand<GuessedLetterVM> RightClick
        {
            get { return _rightClickCommand; }
        }

        public MainVM()
        {
            _solver = new WordleSolver(new WordSet(WordListBuilder.ReadWords()), SuggestWeight.Best());

            _suggestionCommand = new DelegateCommand<string>(
                (s) => { Suggest(); },
                (s) => { return true; });

            _clickCommand = new DelegateCommand<GuessedLetterVM>(
                (s) => { CorrectnessCycle(s); },
                (s) => { return true; });

            _guessCommand = new DelegateCommand<string>(
                (s) => { AddGuessCommand(); },
                (s) => { return ValidGuess(); });

            _clickSuggest = new DelegateCommand<SuggestionWord>(
                (s) => { ClickSuggestion(s); },
                (s) => { return CanSuggestClick(); }
            );

            _rightClickCommand = new DelegateCommand<GuessedLetterVM>(
                (s) => { CorrectnessCycle(s, -1); },
                (s) => { return true; }
            );

            _resetCommand = new DelegateCommand<string>(
                (s) => { Reset(); },
                (s) => { return true; }
            );

            _shareCommand = new DelegateCommand<string>(
                (s) => { CopyAnswerToClipboard(); },
                (s) => { return true; }
                );

            _deleteKeyCommand = new DelegateCommand<string>(
                (s) => { DeleteLastLetterFromGuess(); },
                (s) => { return true; }
                );

            _typeKeyCommand = new DelegateCommand<string>(
                (s) => { AddLetterToGuess(s); },
                (s) => { return true; }
                );

            Guesses = new ObservableCollection<GuessedVM>();
            Guess = "";
            GuessedLetters = new ObservableCollection<char>();
            Suggestions = new ObservableCollection<SuggestionWord>();


        }

        private void AddLetterToGuess(string s)
        {
            if (GuessedLetters.Count < 5)
            {
                GuessedLetters.Add(s[0]);
                GuessedLettersCollectionToString();
            }
        }

        private void DeleteLastLetterFromGuess()
        {
            if(GuessedLetters.Count > 0)
            {
                GuessedLetters.RemoveAt(GuessedLetters.Count - 1);
                GuessedLettersCollectionToString();
            }
        }

        private void CopyAnswerToClipboard()
        {
            string clip = $"WordleSolver: {Guesses.Count}/6*\n";
            foreach(var guess in Guesses)
            {
                foreach(var letter in guess.Letters)
                {
                    if(letter.Correctness == LetterGuessCorrectness.Right)
                    {
                        Rune rune = new Rune('\uD83D', '\uDFE9');
                        clip += rune.ToString();
                    }
                    else if(letter.Correctness == LetterGuessCorrectness.IncorrectLocation)
                    {
                        Rune rune = new Rune('\uD83D', '\uDFE8');
                        clip += rune.ToString();
                    }
                    else
                    {
                        clip += '\u2B1B';
                    }
                }
                clip += "\n";
            }
            Clipboard.SetText(clip);
        }

        private void Reset()
        {
            _solver.StartGuessingNewWord();
            Guesses.Clear();
            Suggest();
            Guess = "";
            GuessedLetters.Clear();
        }

        private bool CanSuggestClick()
        {
            return Suggestions != null && Suggestions.Count > 0;
        }

        private void ClickSuggestion(SuggestionWord suggestion)
        {
            Guess = suggestion.Value;
            AddGuessCommand();
        }

        private bool ValidGuess()
        {
            var s = Guess.ToLower();
            if (s.Length == 5)
            {
                foreach (char c in s)
                {
                    if (!char.IsLetter(c))
                        return false;
                }
            }
            else
                return false;
            return true;
        }

        /// <summary>
        /// Cycles LetterGuessCorrectness enumeration for a GuessedLetterVM, in a specified direction.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="direction"></param>
        public void CorrectnessCycle(GuessedLetterVM e, int direction = 1)
        {
            e.Correctness = (LetterGuessCorrectness)(((int)e.Correctness + direction+3) % 3);
        }

        private void Suggest()
        {
            if (Guesses.Count > 0)
            {
                //record guess info
                foreach (var guess in Guesses)
                    _solver.RecordGuess(guess.Guessed);
            }
            _solver.Guesses = Guesses.Count;
            var s = _solver.SuggestMatchingWords();

            if (s.Count == 0)
                s.Add(new SuggestionWord("No Words Found!", 0));
            Suggestions = s;

            GuessCommand.RaiseCanExecuteChanged();
        }

        private void AddGuessCommand()
        {
            if (ValidGuess()) //not being triggered by behavior otherwise.
            {
                var g = Guess.ToUpper();

                Guesses.Add(new GuessedVM(g));
                Guess = "";
                GuessedLetters.Clear();
            }
        }

        private ObservableCollection<char> _guessedLetters;

        /// <summary>
        /// Collection of letters representing the current (key-board typed and unsubmitted) guess.
        /// </summary>
        public ObservableCollection<char> GuessedLetters
        {
            get => _guessedLetters;
            set
            {
                _guessedLetters = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<GuessedVM> _guesses;

        /// <summary>
        /// Collection of all submitted guesses
        /// </summary>
        public ObservableCollection<GuessedVM> Guesses
        {
            get => _guesses;
            set
            {
                _guesses = value;
                OnPropertyChanged();
            }
        }
        

        private void GuessedLettersCollectionToString()
        {
            var v = "";
            foreach (char c in _guessedLetters)
                v += c;
            Guess = v;
        }
    }
}
