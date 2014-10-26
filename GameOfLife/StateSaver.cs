namespace GameOfLife
{
    using System;
    using System.IO;
    using System.Collections.Generic;
    using System.Runtime.Serialization.Formatters.Binary;

    public class StateSaver : ObservableObject
    {
        #region Fields

        private readonly BinaryFormatter bf = new BinaryFormatter();
        private readonly List<CellModel> cells;
        private bool[,] currentState;

        #endregion

        #region Properties

        public string Filepath
        {
            get { return _filepath; }
            set
            {
                _filepath = value;
                this.OnPropertyChanged("AppTitle");
            }
        }
        private string _filepath;

        public string AppTitle
        {
            get
            {
                if (String.IsNullOrEmpty(this._filepath))
                {
                    return "Game of Life";
                }
                return "Game of Life (" + Path.GetFileName(this._filepath) + ")";
            }
        }

        #endregion

        #region Constructors and destructors

        public StateSaver(List<CellModel> gameCells, int width)
        {
            this.cells = gameCells;
            this.currentState = new bool[width, width];
            this.GetState();
        }

        #endregion

        #region Public methods

        public void GetState()
        {
            foreach (var cell in this.cells)
            {
                this.currentState[cell.RowIndex, cell.ColIndex] = cell.IsAlive;
            }
        }

        public void SaveAs()
        {
            var dlg = new Microsoft.Win32.SaveFileDialog();

            dlg.Filter = "Game of Life files (*.GOL)|*.GOL|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            // Show save file dialog box
            if ((bool)dlg.ShowDialog())
                this.Filepath = dlg.FileName;
            else
                return;

            this.SaveToFile();
        }

        public void SaveToFile()
        {
            // Check a filepath has been defined
            if (String.IsNullOrEmpty(this._filepath))
            {
                this.SaveAs();
                if (String.IsNullOrEmpty(this._filepath))
                    return;
            }

            // Update the current state
            this.GetState();

            // Serialize the array
            using (var file = new FileStream(this._filepath, FileMode.Create, FileAccess.Write))
            {
                this.bf.Serialize(file, this.currentState);
            }
        }

        public void LoadFromFile()
        {
            var dlg = new Microsoft.Win32.OpenFileDialog();

            dlg.Filter = "Game of Life files (*.GOL)|*.GOL|All files (*.*)|*.*";
            dlg.FilterIndex = 1;
            dlg.RestoreDirectory = true;

            if ((bool)dlg.ShowDialog())
                this.Filepath = dlg.FileName;
            else
                return;

            using (var file = new FileStream(this._filepath, FileMode.Open, FileAccess.Read))
            {
                this.currentState = (bool[,])this.bf.Deserialize(file);
            }

            /* Force main thread to finish executing first so residual mouse down
               from opening file doesn't inadvertanly paint the screen! */
            var slowThread = new System.Threading.Thread(this.LoadState);
            slowThread.Start();
            
        }

        public void LoadState()
        {
            // Ensure this executes second
            System.Threading.Thread.Sleep(10);

            foreach (var cell in this.cells)
            {
                cell.IsAlive = this.currentState[cell.RowIndex, cell.ColIndex];
            }
        }

        #endregion
    }
}
