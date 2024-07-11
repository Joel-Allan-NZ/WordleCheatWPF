using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    public class Genetic
    {
        private readonly List<Word> _baseWordList;
        private WordleSolver _solver;

        private static Random rand;

        public Genetic()
        {
            //_builder = new WordListBuilder();
            _baseWordList = WordListBuilder.ReadWords();
            rand = new Random();
        }

        public SuggestWeight[] FindBest(int iterations = 500)
        {

            var sugW = new SuggestWeight(0.1, 0.8, 0.1, 0.01, 0.0);
            var sugY = new SuggestWeight(0.9, 0.2, 0.9, 0.5, 0.1);
            var suggestWeight = new SuggestWeight[12]
            {
                    sugW, sugW, sugW, sugW, sugW, sugW, sugW, sugW, sugW, sugW, sugW, sugW
            };
            var suggestWeightY = new SuggestWeight[12]
{
                    sugY, sugY, sugY, sugY, sugY, sugY, sugY, sugY, sugY, sugY, sugY, sugY
};

            _solver = new WordleSolver(new WordSet(WordListBuilder.ReadWords()), suggestWeight);
            var gen = GeneticBest(suggestWeight, suggestWeightY);

            for (int i = 0; i < iterations; i++)
            {
                var best = FindBest(gen);
                gen = GeneticBest(best[0], best[1]);
            }
            var finalBest = FindBest(gen);
            return finalBest[0];
        }

        private static List<SuggestWeight[]> GeneticBest(SuggestWeight[] best, SuggestWeight[] secondBest)
        {

            List<SuggestWeight[]> suggests = new()
            {
                best,
                secondBest
            };

            for (int i = 0; i < 10; i++)
            {
                var weights = new SuggestWeight[best.Length];
                for (int j = 0; j < best.Length; j++)
                {
                    var doubleLetter = GetNewWeight(best[j].DoubleLetterWeight, secondBest[j].DoubleLetterWeight);
                    var singleLetter = GetNewWeight(best[j].SingleLetterWeight, secondBest[j].SingleLetterWeight);//, rand.NextDouble(), rand.NextDouble());
                    var indexLetter = GetNewWeight(best[j].LetterPlacementWeight, secondBest[j].LetterPlacementWeight);//, rand.NextDouble(), rand.NextDouble());
                    var wordWeight = GetNewWeight(best[j].WordFrequency, secondBest[j].WordFrequency);
                    var rawWeight = GetNewWeight(best[j].RawLetterFrequency, secondBest[j].RawLetterFrequency); 

                    weights[j] = new SuggestWeight(doubleLetter, singleLetter, indexLetter, wordWeight, rawWeight);
                }
                suggests.Add(weights);
            }
            return suggests;
        }

        private static double GetNewWeight(double weight1, double weight2)
        {
            var average = (weight1 + weight2) / 2.0;
            double max;
            double min;
            if (weight1 < weight2)
            {
                min = (weight1 + average) / 2.0;
                max = (weight2 + average) / 2.0;
            }
            else
            {

                max = (weight1 + average) / 2.0;
                min = (weight2 + average) / 2.0;

            }
            return rand.NextDouble() * (max - min) + min;     
        }

        private List<string> PickAnswers(int numberAnswers)
        {
            List<string> answers = new();
            Random rand = new();
            for (int i = 0; i < numberAnswers; i++)
            {
                answers.Add(_baseWordList[rand.Next(_baseWordList.Count - 1)].Value);
            }
            return answers;
        }

        private static List<SuggestWeight[]> PickTopTwo(double[] totalGuesses, List<SuggestWeight[]> weights)
        {
            int bestIndex = 0, secondBest = 1;
            if (totalGuesses[0] > totalGuesses[1])
            {
                bestIndex = 1;
                secondBest = 0;
            }

            for (int i = 2; i < weights.Count; i++)
            {
                if (totalGuesses[i] < totalGuesses[bestIndex])
                {
                    secondBest = bestIndex;
                    bestIndex = i;
                }
                else if (totalGuesses[i] < totalGuesses[secondBest])
                    secondBest = i;
            }

            return new List<SuggestWeight[]>() { weights[bestIndex], weights[secondBest] };
        }

        private List<SuggestWeight[]> FindBest(List<SuggestWeight[]> weights)
        {

            double[] totalGuesses = new double[weights.Count];

            List<string> wordAnswers = PickAnswers(50);

            for (int i = 0; i < weights.Count; i++)
            {
                _solver.Weights = weights[i];
                foreach (var answer in wordAnswers)
                {
                    _solver.StartGuessingNewWord();
                    
                    var guess = new Guessed(_solver.SuggestMatchingWords()[0].Value);

                    while (!CheckGuess(guess, answer))
                    {
                        if (_solver.Guesses >= 6)
                        {
                            totalGuesses[i] += 10;  //penalty for not getting it in 6 guesses
                            break;
                        }
                        guess = new Guessed(_solver.SuggestMatchingWords()[0].Value);
                    }
                    totalGuesses[i] += _solver.Guesses;

                }
            }
            return PickTopTwo(totalGuesses, weights);
        }

        private bool CheckGuess(Guessed guess, string answer)
        {
            UpdateGuess(guess, answer);
            _solver.RecordGuess(guess);

            foreach (char? c in _solver.ConfirmedLocation)
                if (c == null)
                    return false;
            return true;
        }

        private static void UpdateGuess(Guessed guess, string answer)
        {
            Dictionary<char, int> letters = new();
            foreach (char c in answer)
            {
                if (letters.ContainsKey(c))
                    letters[c]++;
                else
                    letters[c] = 1;
            }

            for (int i = 0; i < answer.Length; i++)
            {
                if (guess.Letters[i].GuessedValue == answer[i])
                {
                    guess.Letters[i].Correctness = LetterGuessCorrectness.Right;
                    letters[answer[i]]--;
                }
            }
            for (int i = 0; i < answer.Length; i++)
            {
                char current = guess.Letters[i].GuessedValue;
                if (current != answer[i])
                {
                    if (letters.TryGetValue(current, out int remaining))
                    {
                        if (remaining > 0)
                        {
                            guess.Letters[i].Correctness = LetterGuessCorrectness.IncorrectLocation;
                            letters[current]--;
                        }
                        else
                            guess.Letters[i].Correctness = LetterGuessCorrectness.NotIncuded;
                    }
                    else
                        guess.Letters[i].Correctness = LetterGuessCorrectness.NotIncuded;
                }
            }
        }
    }
}
