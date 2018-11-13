using ArrangeMeasureOverridePoC.ViewModels;
using System.Windows;

namespace ArrangeMeasureOverridePoC
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Constructor
        public MainWindow()
        {
            InitializeComponent();
            Setup();
        }
        #endregion

        #region Private Methods
        private void Setup()
        {
            this.DataContext = new MainWindowViewModel();
        }
        #endregion
    }
}
