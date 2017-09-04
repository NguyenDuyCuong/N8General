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

        #region readonly fields
        public readonly string BussinessMessageFolder = @"TNT.BusinessLogic\Messages";
        public readonly string BussinessFolder = @"TNT.BusinessLogic";
        public readonly string APIFolder = @"TNT.WebApi";
        public readonly string APIControllerFolder = @"TNT.WebApi\Controllers";
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

        private string _methodFolder;

        public string MethodFolder
        {
            get { return _methodFolder; }
            set { _methodFolder = value; OnPropertyChanged("MethodFolder"); }
        }


        private string _methodName;

        public string MethodName
        {
            get { return _methodName; }
            set { _methodName = value; OnPropertyChanged("MethodName"); }
        }

        private bool _allowReplacing;

        public bool AllowReplacing
        {
            get { return _allowReplacing; }
            set { _allowReplacing = value; OnPropertyChanged("AllowReplacing"); }
        }

    }
}
