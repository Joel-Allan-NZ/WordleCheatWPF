# WordleCheatWPF

A word-suggesting tool for 'cheating' at Wordle, saved from an old hard-drive.

Written circa 2021, and solver aspect originally part of a twitterbot to add some extra competition to the daily wordle. The WPF GUI was an after-thought, for ease-of-use on the local machine if/when you might get stuck on a Wordle question.


The word list is likely very far out-of-date, and no effort has been made to clean up the code or repo from what appears to have been some in-development experimental approaches at suggesting the next word. File-locations are hard-coded; it was never intended to be used by anyone else.

Word-selection is explicitly *not* the ideal guess (ie the one that gathers the most information towards a solution), rather words are suggested based on letter-frequency, word-frequency, and some other basic weighting measures. The idea is that it emulates a 'more human' approach to solving the puzzle.

Public purely as an example of some old code.
