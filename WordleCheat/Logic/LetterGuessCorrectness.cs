using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WordleCheat.Logic
{
    /// <summary>
    /// The correctness of a letter in a submitted wordle guess
    /// </summary>
    public enum LetterGuessCorrectness
    {
        NotIncuded, IncorrectLocation, Right
    }
}
