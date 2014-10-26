namespace GameOfLife
{
    using System;
    using System.Collections.Generic;

    class BoardBuilder
    {
        #region Fields and Properties

        private bool isCheckered;

        private int Dimension
        {
            get { return this.dimension; }
            set
            {
                if (value > 0)
                {
                    dimension = value;
                    return;
                }
                throw new ArgumentException("Dimension cannot be less than zero");
            }
        }
        private int dimension = 20;

        private CellModel[,] cellGrid { get; set; }

        #endregion

        #region Builder Methods

        public BoardBuilder WithDimensions(int dim)
        {
            this.Dimension = dim;
            return this;
        }

        public BoardBuilder WithCheckeredStart(bool b)
        {
            this.isCheckered = b;
            return this;
        }

        public List<CellModel> Build() 
        {
            var cells = this.InitialiseCells();
            this.RegisterCellNeighbours(cells);

            return cells;
        }

        #endregion

        #region Private methods

        private List<CellModel> InitialiseCells()
        {
            var cells = new List<CellModel>();
            this.cellGrid = new CellModel[this.dimension, this.dimension];

            for (int i = 0; i < this.dimension; i++)
            {
                for (int j = 0; j < this.dimension; j++)
                {
                    bool alive = this.isCheckered && (((i - j) % 2) == 0);
                    var cell = new CellModel(i, j, alive);

                    this.cellGrid[i, j] = cell;
                    cells.Add(cell);
                }
            }
            return cells;
        }

        private void RegisterCellNeighbours(List<CellModel> cells)
        {
            var max = this.dimension;

            foreach (var cell in cells)
            {
                int i = cell.RowIndex + max;
                int j = cell.ColIndex + max;

                cell.NeighbouringCells = new List<CellModel>();

                for (int n = -1; n <= 1; n++)
                {
                    for (int m = -1; m <= 1; m++)
                    {
                        if ((n == 0) && (m == 0)) continue;

                        var cellToAdd = this.cellGrid[(i + n) % max, (j + m) % max];
                        cell.NeighbouringCells.Add(cellToAdd);
                    }
                }
            }
        }

        #endregion
    }
}
