using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QueensProblem
{
    using OPTANO.Modeling.Optimization;
    using OPTANO.Modeling.Optimization.Operators;
    using OPTANO.Modeling.Optimization.Enums;

    /// <summary>
    /// The Model for the N-Queens problem
    /// </summary>
    public class QueensModel
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="QueensModel"/> class and initializes all fields. 
        /// </summary>
        /// <param name="board">
        /// The empty board
        /// </param>
        /// <param name="dimensions">
        /// The dimensions of the board
        /// </param>

        public QueensModel(int dimension)
        {
            this.Dimension = dimension;
            this.Model = new Model();

            // Choose Item Variable
            this.y = new VariableCollection<int, int>(
               this.Model,
               this.Rows = Enumerable.Range(0, this.Dimension),
               this.Columns = Enumerable.Range(0, this.Dimension),
               "y", 
               (r,c) => $"Is there a queen at position {r},{c}",
               (r,c) => 0,
               (r,c) => 1,
               VariableType.Binary); // it is a binary! only bounds of {0;1} are valid.

            // create a tuple based on rows and columns (row, column)
            var tupleRowsColumns = (from row in this.Rows
                                    from column in this.Columns
                                    select new { row, column }).ToList();
            // Create Constraints

            // at most 1 queen per row
            for (int r = 0; r < this.Dimension; r++)
            {
                this.Model.AddConstraint(
                Expression.Sum(tupleRowsColumns
                .Where(tuple => tuple.row == r)
                .Select(tuple => y[r, tuple.column]))
                == 1,
                $"at most 1 queen in row {r}");
            }

            // at most 1 queen per column
            for (int c = 0; c < this.Dimension; c++)
            {
                this.Model.AddConstraint(
                Expression.Sum(tupleRowsColumns
                .Where(tuple => tuple.column == c)
                .Select(tuple => y[tuple.row, c]))
                == 1,
                $"at most 1 queen in column {c}");
            }

            // no queen diagonal to another (right)
            for (int r = (-1)*this.Dimension + 1; r < this.Dimension; r++)
            {
                this.Model.AddConstraint(
                    Expression.Sum(Enumerable.Range(0,this.Dimension).Where(cnt => r+cnt >= 0 && r+cnt < this.Dimension).Select(cnt => y[(r+cnt),cnt]))
                    <= 1,
                    $"diagonal right {r}"
                    );
            }

            // no queen diagonal to another (left)
            for (int c = (-1) * this.Dimension + 1; c < this.Dimension; c++)
            {
                this.Model.AddConstraint(
                    Expression.Sum(Enumerable.Range(0, this.Dimension).Where(cnt => c - cnt >= 0).Select(cnt => y[cnt, (c-cnt)]))
                    <= 1,
                    $"diagonal left {c}"
                    );
            }

            // Add the objective
            // Sum of all item values.
            // \sum_{item \in Items} \{ y_{item} * value_{item}\}
            this.Model.AddObjective(
            new Objective(
                Expression.Sum(tupleRowsColumns.Select(tuple => y[tuple.row, tuple.column])),
            "sum of queens on the board",
            ObjectiveSense.Maximize)
            );
        }



        /// <summary>
        /// Gets the Model instance
        /// </summary>
        public Model Model { get; private set; }

        /// <summary>
        /// Gets the dimensions of the Board
        /// </summary>
        public int Dimension { get; }

        /// <summary>
        /// Gets the Collection of all design variables
        /// </summary>
        public VariableCollection<int, int> y { get; }

        /// <summary>
        /// Enumerator for the Columns
        /// </summary>
        public IEnumerable<int> Columns { get; private set; }
        /// <summary>
        /// Enumerator for the Rows
        /// </summary>
        public IEnumerable<int> Rows { get; private set; }
    }
}
