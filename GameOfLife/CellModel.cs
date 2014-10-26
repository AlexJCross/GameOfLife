namespace GameOfLife
{
    using System.Collections.Generic;
    using System.Linq;

    public class CellModel : ObservableObject
    {
        #region Properties

        // Mapping from grid to observable collection
        public int ColIndex { get; set; }
        public int RowIndex { get; set; }

        // Whether the cell is currently alive
        public bool IsAlive
        {
            get { return this._isAlive; }
            set
            {
                if (value == this._isAlive) return;

                this._isAlive = value;
                this.OnPropertyChanged("IsAlive");
            }
        }
        private bool _isAlive;

        // Whether to live or die on next increment
        public bool isAliveNext { get; set; }

        // Cache the list of neighbouring cells
        public List<CellModel> NeighbouringCells { get; set; }

        #endregion

        #region Constructors

        public CellModel(int i, int j) 
            : this(i, j, false)
        {
            this.RowIndex = i;
            this.ColIndex = j;
        }

        public CellModel(int i, int j, bool isAlive)
        {
            this.RowIndex = i;
            this.ColIndex = j;
            this.IsAlive = isAlive;
        }

        #endregion

        #region Methods

        public int CountLivingNeighbours()
        {
            return (from neighbour in this.NeighbouringCells
                    where neighbour.IsAlive
                    select neighbour).Count();
        }

        #endregion
    }
}
