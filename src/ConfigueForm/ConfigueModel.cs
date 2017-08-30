using System.ComponentModel;

namespace N8General.ConfigueForm
{
    public class ConfigueModel : INotifyPropertyChanged
    {
        #region Implement INotifiedPropertyChanged

        /// <summary>
        /// Occurs when [property changed].
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;


        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        private string _solutionFolder;

        public string SolutionFolder
        {
            get { return _solutionFolder; }
            set { _solutionFolder = value; OnPropertyChanged("SolutionFolder"); }
        }

        private string _apiControllerName;

        public string APIControllerName
        {
            get { return _apiControllerName; }
            set { _apiControllerName = value; OnPropertyChanged("APIControllerName"); }
        }

        private string _methodName;

        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; OnPropertyChanged("MethodName"); }
        }

    }
}
