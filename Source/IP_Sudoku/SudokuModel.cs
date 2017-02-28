using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sudoku
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Enums;
    using OPTANO.Modeling.Optimization.Operators;
    /// <summary>
    /// A Sudoku Model
    /// </summary>
    class SudokuModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SudokuModel"/> class and initializes all fields
        /// </summary>
        /// <param name="size">
        /// the size of the Sudoku
        /// </param>
        /// <param name="section">
        /// the sections of the Sudoku
        /// </param>
        /// <param name="game">
        /// the initial setting for the Sudoku
        /// </param>
        public SudokuModel(List<int> size, IEnumerable<IGrouping<int,int>> section, int?[,] game)
        {
            this.Size = size;
            this.Section = section;
            this.Game = game;

            this.Model = new Model();

            // Variable for setting remaining values in the game
            this.field = new VariableCollection<int, int, int>(
                            this.Model,
                            this.Size,
                            this.Size,
                            this.Size,
                            "Value",
                            (col, row, number) => new StringBuilder().AppendFormat($"col{col}, row{row}: {number}"),
                            (col, row, number) => 0,
                            (col, row, number) => 1,
                            VariableType.Binary);

            // Create Constraints

            // Only one number per field
            foreach (var row in this.Size)
            {
                foreach (var col in this.Size)
                {
                    this.Model.AddConstraint(
                        Expression.Sum(this.Size.Select(value => this.field[col, row, value])) == 1,
                        string.Format($"Limit_row{row}_col{col}"));
                }
            }


            // Each number once in every row
            foreach (var row in this.Size)
            {
                foreach (var value in this.Size)
                {
                    this.Model.AddConstraint(
                        Expression.Sum(this.Size.Select(col => this.field[col, row, value])) == 1,
                        string.Format($"Limit_row{row}_value{value}"));
                }
            }

            // Each number once in every column
            foreach (var col in this.Size)
            {
                foreach (var value in this.Size)
                {
                    this.Model.AddConstraint(
                        Expression.Sum(this.Size.Select(row => this.field[col, row, value])) == 1,
                        string.Format($"Limit_col{col}_value{value}"));
                }
            }

            foreach (var colSection in this.Section)
            {
                foreach (var rowSection in this.Section)
                {
                    foreach (var value in this.Size)
                    {
                        this.Model.AddConstraint(
                            Expression.Sum(colSection.SelectMany(col => rowSection.Select(row => this.field[col, row, value]))) == 1,
                            string.Format($"$Limit_colSection{colSection.Key}_rowSection{rowSection.Key}_value{value}"));
                    }
                }
            }


            foreach (var col in this.Size)
            {
                foreach (var row in this.Size)
                {
                    if (this.Game[row, col] != null)
                    {
                        var value = this.Game[row, col].Value - 1;

                        this.Model.AddConstraint(
                            this.field[col, row, value] == 1,
                            string.Format($"Game_row{row}_col{col}_Value{value}"));
                    }
                }
            }

            // Note: this model does not need an objective function since it is a pure constraint problem
        }
        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model;

        /// <summary>
        /// Gets the size of the Sudoku
        /// </summary>
        public List<int> Size;

        /// <summary>
        /// Gets the different sections
        /// </summary>
        public IEnumerable<IGrouping<int,int>> Section;

        /// <summary>
        /// Gets the pre-set game (already set fields)
        /// </summary>
        public int?[,] Game;

        /// <summary>
        /// Gets the collection of decision variables (x,y,z) is there a value of z on field (x,y)
        /// if yes -> 1 if not -> 0
        /// </summary>
        public VariableCollection<int, int, int> field;
    }
}
