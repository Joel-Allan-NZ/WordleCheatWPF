using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    /// <summary>
    /// Represents weightings that can be placed on the different factors considered when evaluating
    /// the value of possible SuggestionWords, with respect to the amount of information known about the current Word.
    /// </summary>
    public class SuggestWeight
    {
        /// <summary>
        /// Weight based on letter frequency, including when a word has the same letter multiple times.
        /// </summary>
        public double DoubleLetterWeight { get; set; }
        /// <summary>
        /// Weight based on letter frequency, excluding when a word has the same letter multiple times.
        /// </summary>
        public double SingleLetterWeight { get; set; }

        /// <summary>
        /// Weight based on frequency of letters at each index.
        /// </summary>
        public double LetterPlacementWeight { get; set; }
        /// <summary>
        /// Weight based on frequency of use of the word being selected (based on a wikipedia snapshot).
        /// </summary>
        public double WordFrequency { get; set; }

        /// <summary>
        /// Weight based on standard english dictionary based letter frequency
        /// </summary>
        public double RawLetterFrequency { get; set;}

        public static Dictionary<char, double> EnglishFrequencies { get; private set; }

        public SuggestWeight(double doubleWeight, double singleWeight, double placementWeight, double wordFrequency, double rawLetterFrequency)
        {
            DoubleLetterWeight = doubleWeight;
            SingleLetterWeight = singleWeight;
            LetterPlacementWeight = placementWeight;
            WordFrequency = wordFrequency;
            RawLetterFrequency = rawLetterFrequency;

            if(EnglishFrequencies == null)
            {
                EnglishFrequencies = GetEnglishFrequencies();
            }
        }

        private static Dictionary<char,double> GetEnglishFrequencies()
        {
            return new Dictionary<char, double>()
            {
                {'E', 0.111607 },
                {'A', 0.084966 },
                {'R', 0.075809 },
                {'I', 0.075448 },
                {'O', 0.071635 },
                {'T', 0.069509 },
                {'N', 0.066544 },
                {'S', 0.057351 },
                {'L', 0.054893 },
                {'C', 0.045388 },
                {'U', 0.036308 },
                {'D', 0.033844 },
                {'P', 0.031671 },
                {'M', 0.030129 },
                {'H', 0.030034 },
                {'G', 0.024705 },
                {'B', 0.020720 },
                {'F', 0.018121 },
                {'Y', 0.017779 },
                {'W', 0.012899 },
                {'K', 0.011016 },
                {'V', 0.010074 },
                {'X', 0.002902 },
                {'Z', 0.002722 },
                {'J', 0.001965 },
                {'Q', 0.001962 }
            };
        }

        public override string ToString()
        {
            return $"{DoubleLetterWeight},{SingleLetterWeight},{LetterPlacementWeight},{WordFrequency}";
        }

        /// <summary>
        /// Pre-computed optimized weightings.
        /// </summary>
        /// <returns></returns>
        public static SuggestWeight[] Best()
        {
            var sugg = new SuggestWeight[]
            {
                new SuggestWeight(0.11582433869174048,0.7894907906207929,0.12245349259112778,0.00, 0.00),
                new SuggestWeight(0.11132083326624118,0.7849708869620727,0.15397507655622986,0.024411558257636766, 0.0001),
                new SuggestWeight(0.10439627561643992,0.7159945290360284,0.12257052021015663,0.020831268296576968, 0.0001),
                new SuggestWeight(0.1401240058250017,0.7963190798817023,0.11322735931250524,0.00598043431275206, 0.0001),
                new SuggestWeight(0.10740926819623417,0.793917165172879,0.11899428081995883,0.007518081426898169, 0.0001),
                new SuggestWeight(0.17721116940276585,0.7869791163451633,0.12077306236810015,0.15193476834449334, 0.0001),
                new SuggestWeight(0.11441918007064747,0.7881591132738149,0.12700649830337632,0.208299656654929181, 0.0001),
                new SuggestWeight(0.10777954403899465,0.7887357634756019,0.11078620111957689,0.21102901388175026, 0.0001),
                new SuggestWeight(0.12120749584686664,0.7618445900602433,0.11870281094299963,0.208388444837592296, 0.0001),
                new SuggestWeight(0.11652140351053918,0.7779717932412709,0.14501161269843785,0.210604868498450315, 0.0001),
                new SuggestWeight(0.10671117385311782,0.7871183298527861,0.11042649412490743,0.218027065827884554, 0.0001),
                new SuggestWeight(0.11965436528882804,0.7594584169867947,0.17742139060850717,0.222337805493446956, 0.0001),
            };
            return sugg;
        }
    }
}
