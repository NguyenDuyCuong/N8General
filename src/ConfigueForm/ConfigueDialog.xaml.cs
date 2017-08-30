using System.Windows;
using System.Windows.Forms;

namespace N8General.ConfigueForm
{
    /// <summary>
    /// Interaction logic for ConfigueDialog.xaml
    /// </summary>
    public partial class ConfigueDialog : Window
    {
        public ConfigueDialog()
        {
            InitializeComponent();
        }

        private void ButtonClose_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ConfigueViewModel;
            if (vm != null && vm.ModelValid())
            {
                DialogResult = true;
                Close();
            }
        }

        private void ButtonFolderDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ConfigueViewModel;
            if (vm != null)
            {
                var diaFolder = new FolderBrowserDialog()
                {
                    ShowNewFolderButton = false,
                    SelectedPath = vm.Model.SolutionFolder
                };
                if (diaFolder.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    
                }
            }
        }

        private void ButtonFileDialog_OnClick(object sender, RoutedEventArgs e)
        {
            var vm = this.DataContext as ConfigueViewModel;
            if (vm != null)
            {
                var diaFile = new OpenFileDialog()
                {
                    InitialDirectory = vm.Model.SolutionFolder,
                    
                };
                if (diaFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {

                }
            }
        }
    }
}
