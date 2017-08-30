using System;
using System.ComponentModel;

namespace N8General.ConfigueForm
{
    public class ConfigueViewModel : INotifyPropertyChanged
    {
        #region Implement INotifiedPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        private ConfigueModel _model;

        public ConfigueModel Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged("Model"); }
        }

        public bool ModelValid()
        {
            return true;
        }

        public void InitDialog(string folder)
        {
            Model = new ConfigueModel()
            {
                SolutionFolder = folder,
            };
        }
    }
}
