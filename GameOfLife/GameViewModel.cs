namespace GameOfLife
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Collections.Generic;
    using System.Windows.Threading;
    using System.Windows.Input;
    using System.Windows.Controls;

    public class GameViewModel : ObservableObject
    {
        #region Observable Properties

        public List<CellModel> Cells { get; set; }
        public StateSaver stateSaver { get; set; }
        
        public string PlayCaption
        {
            get { return IsPaused ? "Start" : "Stop"; }
        }

        public bool IsPaused
        {
            get { return isPaused; }
            set
            {
                isPaused = value;
                this.OnPropertyChanged("IsPaused");
                this.OnPropertyChanged("PlayCaption");
            }
        }
        private bool isPaused = true;

        #endregion

        #region Public Commands

        public ICommand PlayPauseCommand
        {
            get { return new RelayCommand(tog => this.PlayPause(tog)); }
        }

        public ICommand ResetCommand
        {
            get { return new RelayCommand(t => this.Reset()); }
        }

        public ICommand RestoreLastCommand
        {
            get { return new RelayCommand(t => this.stateSaver.LoadState()); }
        }

        public ICommand SaveAsCommand
        {
            get { return new RelayCommand(t => this.stateSaver.SaveAs()); }
        }

        public ICommand SaveCommand
        {
            get { return new RelayCommand(t => this.stateSaver.SaveToFile()); }
        }

        public ICommand OpenCommand
        {
            get { return new RelayCommand(t => this.stateSaver.LoadFromFile()); }
        }

        #endregion

        #region Constructors

        public GameViewModel(int width)
        {
            this.Cells = new BoardBuilder()
                            .WithDimensions(width)
                            .WithCheckeredStart(true)
                            .Build();

            this.stateSaver = new StateSaver(this.Cells, width);
        }

        #endregion

        #region Private Methods

        private void Reset()
        {
            this.IsPaused = true;

            foreach (var cell in this.Cells)
            {
                cell.IsAlive = false;
            }
        }

        private void PlayPause(object grid)
        {
            // First save the state in case we wish to restore.
            if (this.IsPaused)
                return;
            else
                this.stateSaver.GetState();

            var sw = new System.Diagnostics.Stopwatch();
            var refreshTime = TimeSpan.FromMilliseconds(75);
            var icCellGrid = (ItemsControl)grid;

            while (!this.IsPaused && icCellGrid.IsVisible)
            {
                // Start the clock
                sw.Restart();

                this.IncrementStates();

                // Manually refresh the grid
                icCellGrid.Dispatcher.Invoke(
                    DispatcherPriority.ApplicationIdle, 
                    new Action(() => { }));

                // Wait out the remaining time
                int rest = (refreshTime - sw.Elapsed).Milliseconds;
                Thread.Sleep((rest > 0) ? rest : 0);
            }
        }

        private void IncrementStates()
        {
            foreach (var cell in this.Cells)
            {
                int neighbours = cell.CountLivingNeighbours();

                cell.isAliveNext = ((neighbours == 3) ||
                                   ((neighbours == 2) && cell.IsAlive));
            }

            this.Cells
                .ToList()
                .ForEach(cell => cell.IsAlive = cell.isAliveNext);
        }

        #endregion
    }
}
