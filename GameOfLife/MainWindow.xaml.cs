namespace GameOfLife
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using System.Windows.Controls.Primitives;

    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = new GameViewModel(20);
        }

        private void cellMouseEnter(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var tog = (ToggleButton)sender;
                tog.IsChecked = true;
            }
        }
    }
}
