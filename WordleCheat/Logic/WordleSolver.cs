using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WordleCheat.Extensions;

namespace WordleCheat.Logic
{
    public class WordleSolver
    {
        /// <summary>
        /// Length of wordle word.
        /// </summary>
        const int wordLength = 5;

        /// <summary>
        /// The valid words for the solver to use.
        /// </summary>
        public WordSet WordSet { get; set; }

        /// <summary>
        /// The current 'score' of information known about the word, used for selecting the correct SuggestWeight.
        /// </summary>
        public int KnownScore { get; set; }

        /// <summary>
        /// The number of guesses already submitted for this wordle solution.
        /// </summary>
        public int Guesses { get; set; }
        /// <summary>
        /// Letters that have been eliminated.
        /// </summary>
        public HashSet<char>[] EliminatedLetters { get; set; }

        /// <summary>
        /// Represents known (ie green background) letters and their locations. Null for spaces in word that are not known.
        /// </summary>
        public char?[] ConfirmedLocation { get; set; }
        /// <summary>
        /// Represents all letters (yellow and green backgrounds) that are known to be in the wordle word.
        /// </summary>
        public Dictionary<char, int> RequiredLetters { get; set; }

        /// <summary>
        /// All known words that fit the information known about the wordle word.
        /// </summary>
        public List<Word> ValidWords { get; set; }

        /// <summary>
        /// SuggestWeight weightings for various levels of known information.
        /// </summary>
        public SuggestWeight[] Weights { get; set; }

        private Dictionary<char, int> contained;

        private string firstWord;

        /// <summary>
        /// Instantiate an instance of the WordleSolver in order to begin attempting to find a solution.
        /// </summary>
        /// <param name="wordSet">The possible answers and submittable words that the solver must consider</param>
        /// <param name="weights">Weights for various levels of known information about the mystery Wordle word</param>
        /// <param name="forcedFirstWord">Optional parameter; a word to force as a suggestion on the first entry</param>
        public WordleSolver(WordSet wordSet, SuggestWeight[] weights, string forcedFirstWord = null) 
        {
            WordSet = wordSet;        
            Weights = weights;
            StartGuessingNewWord();
            contained =  new Dictionary<char, int>();
            firstWord = forcedFirstWord;

        }

        /// <summary>
        /// Find and return the best possible SuggestionWords matching the current state of known information.
        /// </summary>
        /// <returns></returns>
        public List<SuggestionWord> SuggestMatchingWords()
        {
            var weight = SelectWeight();
            if (Guesses == 0 && firstWord != null)
                return new List<SuggestionWord>() { new SuggestionWord(firstWord, 1.0) };
            return WordSet.GetWordSuggestions(weight);
        }

        private SuggestWeight SelectWeight()
        {
            return Weights[KnownScore];
        }

        /// <summary>
        /// Begin a new word, resetting guesses and known information.
        /// </summary>
        public void StartGuessingNewWord()
        {
            ConfirmedLocation = new char?[wordLength];
            RequiredLetters = new();
            EliminatedLetters = new HashSet<char>[wordLength];
            for (int i = 0; i < EliminatedLetters.Length; i++)
            {
                EliminatedLetters[i] = new HashSet<char>();
            }
            Guesses = 0;
            KnownScore = 0;
            WordSet.NewWord();
        }

        /// <summary>
        /// Record the information gained from a word that has been guessed (letters confirmed to be in wordle word etc).
        /// </summary>
        /// <param name="guess"></param>
        public void RecordGuess(Guessed guess)
        {
            RequiredLetters.Clear();

            for(int i =0; i<guess.Letters.Count; i++)
            {
                var letterGuessed = guess.Letters[i];
                var characterGuessed = letterGuessed.GuessedValue;

                if(letterGuessed.Correctness == LetterGuessCorrectness.Right) //record correct letters
                {
                    ConfirmedLocation[i] = characterGuessed;
                    RequiredLetters.AddOrSumValue(characterGuessed, 1);
                }
                else if (letterGuessed.Correctness == LetterGuessCorrectness.IncorrectLocation) //record incorrectly placed letters
                {
                    EliminatedLetters[i].Add(characterGuessed);
                    RequiredLetters.AddOrSumValue(characterGuessed, 1);

                    //we need a minimum of (Green count of letter) + (discovered yellow count of letter). Because the algorithm never uses fewer letters than we need
                    //we can guarantee that re-counting is a safe action (ie we won't 'forget' an old letter.
                }
            }

            for (int i = 0; i < guess.Letters.Count; i++)
            {
                var letterGuessed = guess.Letters[i];
                var characterGuessed = letterGuessed.GuessedValue;

                if (letterGuessed.Correctness == LetterGuessCorrectness.NotIncuded)
                {
                    if (RequiredLetters.ContainsKey(characterGuessed))
                    {
                        EliminatedLetters[i].Add(characterGuessed);
                    }
                    else
                    {
                        for(int j =0; j < guess.Letters.Count; j++)
                        {
                            EliminatedLetters[j].Add(characterGuessed);
                        }
                    }
                }
            }
            DetermineKnownScore();
            Guesses++;
            WordSet.CullInvalid(IsWordMatch);
        }        

        private void DetermineKnownScore()
        {
            KnownScore = 0;
            foreach (char? c in ConfirmedLocation)
                if (c != null)
                    KnownScore ++;

            KnownScore += RequiredLetters.Count;
        }

        /// <summary>
        /// Check if a word has a value which matches known information about the wordle word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsWordMatch(Word word) => IsWordMatch(word.Value);

        /// <summary>
        /// Check if a string matches known information about the wordle word.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public bool IsWordMatch(string word)
        {

            contained.Clear();


            for (int i = 0; i < word.Length; i++)
            {
                if (ConfirmedLocation[i] != null && ConfirmedLocation[i] != word[i])
                    return false;

                if (EliminatedLetters[i].Contains(word[i]))
                    return false;

                contained.AddOrSumValue(word[i], 1);
            }
            foreach (var kvp in RequiredLetters)
            {
                if (!contained.ContainsKey(kvp.Key) || contained[kvp.Key] < kvp.Value)
                    return false;
                   
            }
            return true;
        }
    }
}
