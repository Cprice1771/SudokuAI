using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SudokuAI
{
    public static class Parser
    {
        public static List<List<int>> Parse(string file)
        {
            List<List<int>> board = new List<List<int>>();
            string[] lines = System.IO.File.ReadAllLines(file);

            foreach (string line in lines)
            {
                board.Add(new List<int>());

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ',')
                        board[board.Count - 1].Add(-1);
                    else
                    {
                        board[board.Count - 1].Add(Int32.Parse(line.Substring(i,1)));
                        i++;
                    }
                }

                //edge case for when the line ends in an empty cell
                if (board[board.Count - 1].Count == 8)
                    board[board.Count - 1].Add(-1);

            }

            return board;
        }
    }
}
