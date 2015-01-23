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

        private const string INPUT = "\\Input\\Medium\\Puzzle189Medium.sd";

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
            List<List<List<int>>> emptyGuesses = new List<List<List<int>>>();
            for (int i = 0; i < 9; i++)
            {
                emptyGuesses.Add(new List<List<int>>());
                for (int j = 0; j < 9; j++)
                {
                    emptyGuesses[i].Add(new List<int>());
                }
            }

            Dictionary<Board, List<List<List<int>>>> previousGuessesDict = new Dictionary<Board, List<List<List<int>>>>();
            KeyValuePair<Board, List<List<List<int>>>> currentGuess = new KeyValuePair<Board, List<List<List<int>>>>(board, emptyGuesses);

            int bestGuess = 1;
            _iterations = 0;
            _guessCount = 0;
            _playsMade = 0;
            while (board.Unsolved)
            {
                bool exhaustedGuesses = false;
                bool madeProgress = false;
                _iterations++;

                for(int i = 0; i < board.Count; i++)
                {
                    for(int j = 0; j < board[i].Count; j++)
                    {
                        if (board[i][j] == -1)
                        {
                            List<int> solutions = board.GetPossibleSolutions(i, j);

                            if (solutions.Count == bestGuess)
                            {
                                //If were guessing
                                if (bestGuess > 1)
                                {
                                    Console.WriteLine(board.ToString());
                                    
                                    
                                    

                                    int k = 0;

                                    //All the different guesses at all the different levels
                                    foreach (KeyValuePair<Board, List<List<List<int>>>> guess in previousGuessesDict)
                                        while (guess.Value[i][j].Contains(solutions[k]))
                                            k++;

                                    //If we've trired all our guesses
                                    if(k == solutions.Count)
                                        exhaustedGuesses = true;
                                    //Else try the next guess
                                    else
                                    {
                                        //Board lastGuessBoard = GetLastGuess(previousGuessesDict);
                                        //previousGuessesDict[lastGuessBoard][i][j].Add(solutions[k]);
                                        currentGuess.Value[i][j].Add(solutions[k]);
                                        board[i][j] = solutions[k];
                                        _playsMade++;
                                        madeProgress = true;
                                    }

                                    bestGuess = 1;
                                    previousGuessesDict.Add(currentGuess.Key, currentGuess.Value);
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

                                    Board b = new Board(board.Clone());
                                    currentGuess = new KeyValuePair<Board, List<List<List<int>>>>(b, emptyG);
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
                }

                //if we weren't able to make any progress mkae the next reasonable guess
                if (!madeProgress)
                    bestGuess++;

                //If there are no more possible plays reset to last board state and guess again
                if (bestGuess >= 10)
                {
                    board = GetLastGuess(previousGuessesDict);
                    bestGuess = 1;
                }

                //If we've used all the guesses we can, revert to previous guess board with that branch marked as invalid
                if(exhaustedGuesses)
                {
                    //remove the last guess
                    Board lastGuessBoard = GetLastGuess(previousGuessesDict);
                    previousGuessesDict.Remove(lastGuessBoard);

                    //set the current guess to the previous guess
                    lastGuessBoard = GetLastGuess(previousGuessesDict);
                    currentGuess = new KeyValuePair<Board,  List<List<List<int>>>>(lastGuessBoard,previousGuessesDict[lastGuessBoard]);
                    board = lastGuessBoard;

                    bestGuess = 1;
                }

            }


            return board;
        }

        private Board GetLastGuess(Dictionary<Board, List<List<List<int>>>> guessDict)
        {
            int leastEmptySpots = 100;
            Board lastGuessBoard = new Board(new List<List<int>>());

            foreach (KeyValuePair<Board, List<List<List<int>>>> guess in guessDict)
            {
                if (guess.Key.GetEmptySpaceCount() < leastEmptySpots)
                {
                    lastGuessBoard = guess.Key;
                    leastEmptySpots = guess.Key.GetEmptySpaceCount();
                }
            }

            return lastGuessBoard;
        }

        
    }
}
