using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SudokuAI
{
    public partial class SudokuForm : Form
    {
        int Iterations;
        int GuessCount;

        public SudokuForm()
        {
            InitializeComponent();
            fileTextBox.Text = Directory.GetCurrentDirectory() + "\\Input\\Easy\\Puzzle189Easy.sd";
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
            output += "Iterations: " + Iterations + "\n";
            output += "Guesses: " + GuessCount + "\n";

            return output;
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.InitialDirectory = Directory.GetCurrentDirectory();

            //Only update the text box if the user selected OK
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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

            Dictionary<Board, List<List<List<int>>>> PreviousGuessesDict = new Dictionary<Board, List<List<List<int>>>>();
            KeyValuePair<Board, List<List<List<int>>>> CurrentGuess = new KeyValuePair<Board, List<List<List<int>>>>(board, emptyGuesses);
            bool madeProgress;

            int bestGuess = 1;
            Iterations = 0;
            GuessCount = 0;
            while (board.Unsolved)
            {
                bool exhaustedGuesses = false;
                madeProgress = false;
                Iterations++;

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
                                    bestGuess = 1;
                                    PreviousGuessesDict.Add(CurrentGuess.Key, CurrentGuess.Value);
                                    GuessCount++;
                                    CurrentGuess = new KeyValuePair<Board, List<List<List<int>>>>(board, emptyGuesses);

                                    int k = 0;

                                    //All the different guesses at all the different levels
                                    foreach (KeyValuePair<Board, List<List<List<int>>>> guess in PreviousGuessesDict)
                                    {
                                        while (guess.Value[i][j].Contains(solutions[k]))
                                            k++;
                                    }

                                    //If we've trired all our guesses
                                    if(k == solutions.Count)
                                        exhaustedGuesses = true;
                                    //Else try the next guess
                                    else
                                    {
                                        int leastEmptySpots = 100;
                                        Board lastGuessBoard = new Board(new List<List<int>>());
                                        foreach (KeyValuePair<Board, List<List<List<int>>>> guess in PreviousGuessesDict)
                                        {
                                            if (guess.Key.GetEmptySpaceCount() < leastEmptySpots)
                                                lastGuessBoard = guess.Key;
                                        }

                                        PreviousGuessesDict[lastGuessBoard][i][j].Add(solutions[k]);
                                        board[i][j] = solutions[k];
                                        madeProgress = true;
                                    }
                                }
                                else
                                {
                                    board[i][j] = solutions[0];
                                    madeProgress = true;
                                    bestGuess = 1;
                                }
                            }
                        }
                    }
                }

                //if we weren't able to make any progress, break out of everything, we've done all we can do
                if (!madeProgress)
                    bestGuess++;

                //If there are no more possible plays reset to last board state
                if (bestGuess >= 10)
                {
                    int leastEmptySpots = 100;
                    Board lastGuessBoard = new Board(new List<List<int>>());
                    foreach(KeyValuePair<Board,  List<List<List<int>>>> guess in PreviousGuessesDict)
                    {
                        if(guess.Key.GetEmptySpaceCount() < leastEmptySpots)
                            lastGuessBoard = guess.Key;
                    }

                    board = lastGuessBoard;
                    bestGuess = 1;
                }

                //If we've used all the guesses we can, revert to previous guess board with that branch marked as invalid
                if(exhaustedGuesses)
                {
                    int leastEmptySpots = 100;
                    Board lastGuessBoard = new Board(new List<List<int>>());
                    foreach(KeyValuePair<Board,  List<List<List<int>>>> guess in PreviousGuessesDict)
                    {
                        if(guess.Key.GetEmptySpaceCount() < leastEmptySpots)
                            lastGuessBoard = guess.Key;
                    }

                    CurrentGuess = new KeyValuePair<Board,  List<List<List<int>>>>(lastGuessBoard,PreviousGuessesDict[lastGuessBoard]);
                    PreviousGuessesDict.Remove(lastGuessBoard);
                    bestGuess = 1;
                }

            }


            return board;
        }

        
    }
}
