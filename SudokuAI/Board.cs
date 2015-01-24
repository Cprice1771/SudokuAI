using System;
using System.Collections.Generic;

namespace SudokuAI
{
    public class Board : List<List<int>>
    {
        protected bool Equals(Board other)
        {
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++)
                        if (this[i][j] != other[i][j])
                            return false;

                return true;
        }

        public override int GetHashCode()
        {
            int sum = 0;

            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    sum += this[i][j];

            return sum;
        }

        public Board(IEnumerable<List<int>> board): base(board)
        {

        }

        public Board(IEnumerable<List<int>> board, string file)
            : base(board)
        {
            Parse(file);
        }

        //Clears out the current board and replaces it with the new board from the file
        public void Parse(string file)
        {

            Clear();

            string[] lines = System.IO.File.ReadAllLines(file);

            foreach (string line in lines)
            {
                Add(new List<int>());

                for (int i = 0; i < line.Length; i++)
                {
                    if (line[i] == ',')
                        this[Count - 1].Add(-1);
                    else
                    {
                        this[Count - 1].Add(Int32.Parse(line.Substring(i, 1)));
                        i++;
                    }
                }

                //edge case for when the line ends in an empty cell
                if (this[Count - 1].Count == 8)
                    this[Count - 1].Add(-1);

            }

        }

        /// <summary>
        /// Returns true if the given board has a blank
        /// </summary>
        /// <returns></returns>
        public bool Unsolved
        {
            get
            {
                foreach (List<int> row in this)
                    foreach (int num in row)
                        if (num == -1)
                            return true;

                return false;

            }
        }

        internal List<int> GetPossibleSolutions(int row, int col)
        {
            List<int> solutions = new List<int>();

            for (int num = 1; num < 10; num++)
                solutions.Add(num);
            foreach (int num in GetRowNums(row))
                solutions.Remove(num);

            foreach (int num in GetColoumnNums(col))
                solutions.Remove(num);

            foreach (int num in GetQuadNums(row, col))
                solutions.Remove(num);

            return solutions;

        }

        private List<int> GetQuadNums(int row, int col)
        {
            int quad = GetQuad(row, col);
            List<int> nums = new List<int>();

            switch (quad)
            {
                case 1:
                    for (int i = 0; i < 3; i++)
                        for (int j = 0; j < 3; j++)
                            nums.Add(this[i][j]);
                    break;
                case 2:
                    for (int i = 0; i < 3; i++)
                        for (int j = 3; j < 6; j++)
                            nums.Add(this[i][j]);
                    break;
                case 3:
                    for (int i = 0; i < 3; i++)
                        for (int j = 6; j < 9; j++)
                            nums.Add(this[i][j]);
                    break;
                case 4:
                    for (int i = 3; i < 6; i++)
                        for (int j = 0; j < 3; j++)
                            nums.Add(this[i][j]);
                    break;
                case 5:
                    for (int i = 3; i < 6; i++)
                        for (int j = 3; j < 6; j++)
                            nums.Add(this[i][j]);
                    break;
                case 6:
                    for (int i = 3; i < 6; i++)
                        for (int j = 6; j < 9; j++)
                            nums.Add(this[i][j]);
                    break;
                case 7:
                    for (int i = 6; i < 9; i++)
                        for (int j = 0; j < 3; j++)
                            nums.Add(this[i][j]);
                    break;
                case 8:
                    for (int i = 6; i < 9; i++)
                        for (int j = 3; j < 6; j++)
                            nums.Add(this[i][j]);
                    break;
                case 9:
                    for (int i = 6; i < 9; i++)
                        for (int j = 6; j < 9; j++)
                            nums.Add(this[i][j]);
                    break;
                default:
                    throw new FormatException("Invalid Quad Given");
            }

            return nums;

        }

        private int GetQuad(int row, int col)
        {
            int quad = -1;

            if (row > 5)
            {
                if (col > 5)
                    quad = 9;
                else if (col > 2)
                    quad = 8;
                else if (col >= 0)
                    quad = 7;
            }
            else if (row > 2)
            {
                if (col > 5)
                    quad = 6;
                else if (col > 2)
                    quad = 5;
                else if (col >= 0)
                    quad = 4;
            }
            else
            {
                if (col > 5)
                    quad = 3;
                else if (col > 2)
                    quad = 2;
                else if (col >= 0)
                    quad = 1;
            }

            return quad;
        }

        private List<int> GetColoumnNums(int col)
        {
            List<int> foundNumbers = new List<int>();

            foreach (List<int> row in this)
            {
                if (row[col] != -1)
                    foundNumbers.Add(row[col]);
            }

            return foundNumbers;
        }

        private List<int> GetRowNums(int row)
        {
            List<int> foundNumbers = new List<int>();

            foreach (int num in this[row])
            {
                if(num != -1)
                    foundNumbers.Add(num);
            }

            return foundNumbers;
        }

        public int GetEmptySpaceCount()
        {
            int count = 0;
            foreach (List<int> row in this)
                foreach (int num in row)
                    if (num == -1)
                        count++;
            return count;

        }

        public override bool Equals(object obj)
        {
            if (!(obj is Board)) return false;

            Board other = obj as Board;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (this[i][j] != other[i][j])
                        return false;

            return true;
        }

        public override string ToString()
        {
            string output = "";

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                    output += this[i][j] + " ";

                output += "\n";
            }

            return output;

        }

        public Board Clone()
        {
            List<List<int>> cloneList = new List<List<int>>();

            for (int i = 0; i < 9; i++)
            {
                cloneList.Add(new List<int>());
                for (int j = 0; j < 9; j++)
                    cloneList[i].Add(this[i][j]);
            }

            return new Board(cloneList);

        }

    }
}
