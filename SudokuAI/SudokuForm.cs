using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SudokuAI
{
    public partial class SudokuForm : Form
    {
        private int _iterations;
        private int _guessCount;
        private int _playsMade;

        private const string INPUT = "\\Input\\Medium\\Puzzle188Medium.sd";

        public SudokuForm()
        {
            InitializeComponent();
            fileTextBox.Text = Directory.GetCurrentDirectory() + INPUT;
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            Board board = new Board(new List<List<int>>());
            board.Parse(fileTextBox.Text);

            board = Solve(board);

            solutionBox.Text = PrintBoard(board);

        }

        private string PrintBoard(List<List<int>> board)
        {
            string output = "";

            foreach (List<int> row in board)
            {
                for (int i = 0; i < row.Count; i++)
                {
                    if (i < row.Count - 1)
                        output += row[i] + ",";
                    else
                        output += row[i];
                }
                output += "\n";
            }

            output += "\n";
            output += "Iterations: " + _iterations + "\n";
            output += "Guesses: " + _guessCount + "\n";
            output += "Plays Made: " + _playsMade + "\n";

            return output;
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            //Only update the text box if the user selected OK
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                //Update the text box with the new file name
                fileTextBox.Text = dialog.FileName;
            }
        }

        private Board Solve(Board board)
        {
            List<Guess> currentGuesses = new List<Guess>();

            int bestGuess = 1;
            _iterations = 0;
            _guessCount = 0;
            _playsMade = 0;

            while (board.Unsolved)
            {
                bool exhaustedGuesses = false;
                bool madeProgress = false;
                bool noPlays = true;
                _iterations++;

                for (int i = 0; i < board.Count; i++)
                {
                    for (int j = 0; j < board[i].Count; j++)
                    {
                        if (board[i][j] == -1)
                        {
                            List<int> solutions = board.GetPossibleSolutions(i, j);

                            //If we aren't stuck in a dead end
                            if (solutions.Count != 0)
                                noPlays = false;

                            //Make sure we only guess if we have to
                            if (solutions.Count != bestGuess) continue;
                            //If were guessing
                            if (bestGuess > 1)
                            {
                                Console.WriteLine(board.ToString());

                                currentGuesses.Add(new Guess()
                                {
                                    GameBoard = board,
                                    Guesses = new List<int>()
                                });

                                int index = 0;
                                //Check to see if we already used this guess
                                while (index < solutions.Count && currentGuesses[currentGuesses.Count - 1].Guesses.Contains(solutions[index]))
                                    index++;

                                //If we've trired all our guesses
                                if(index == solutions.Count)
                                    exhaustedGuesses = true;

                                //Else try the next guess
                                else
                                {
                                    currentGuesses[currentGuesses.Count - 1].Guesses.Add(solutions[index]);
                                    board[i][j] = solutions[index];
                                    _playsMade++;
                                    madeProgress = true;
                                   

                                    bestGuess = 1;
                                        
                                    _guessCount++;

                                    List<List<List<int>>> emptyG = new List<List<List<int>>>();
                                    for (int l = 0; l < 9; l++)
                                    {
                                        emptyG.Add(new List<List<int>>());
                                        for (int m = 0; m < 9; m++)
                                        {
                                            emptyG[l].Add(new List<int>());
                                        }
                                    }
                                }
                            }
                            else
                            {
                                board[i][j] = solutions[0];
                                _playsMade++;
                                madeProgress = true;
                                bestGuess = 1;
                            }
                        }
                    }
                }

                //if we weren't able to make any progress make the next reasonable guess
                if (!madeProgress)
                    bestGuess++;

                //If there are no more possible plays reset to last board state and guess again
                if (bestGuess >= 10 && currentGuesses.Count > 0)
                {
                    board = currentGuesses[currentGuesses.Count - 1].GameBoard;
                    bestGuess = 1;
                }

                //If we've used all the guesses we can, revert to previous guess board with that branch marked as invalid
                if ((exhaustedGuesses || noPlays) && (currentGuesses.Count > 1))
                {
                    //pop the last guess off the top
                    currentGuesses.RemoveAt(currentGuesses.Count - 1);

                    //set the current guess to the previous guess
                    Board lastGuessBoard = currentGuesses[currentGuesses.Count - 1].GameBoard;
                    board = lastGuessBoard;
                    bestGuess = 1;

                    
                }

            }


            return board;
        }


        struct Guess
        {
            public Board GameBoard { get; set; }
            public List<int> Guesses;

        }
        
    }
}
