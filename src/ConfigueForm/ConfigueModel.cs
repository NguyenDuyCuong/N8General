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

        #region Call Service
        private string _controllerName;

        public string ControllerName
        {
            get { return _controllerName; }
            set { _controllerName = value; }
        }

        private string _controllerNameAlternative;

        public string ControllerNameAlternative
        {
            get { return _controllerNameAlternative; }
            set { _controllerNameAlternative = value; }
        }

        private string _adminScriptControllerRelaytive;

        public string AdminScriptControllerRelaytive
        {
            get { return _adminScriptControllerRelaytive; }
            set { _adminScriptControllerRelaytive = value; }
        }

        private string _adminScriptController;

        public string AdminScriptController
        {
            get { return _adminScriptController; }
            set { _adminScriptController = value; }
        }


        private string _adminScriptServiceRelaytive;

        public string AdminScriptServiceRelaytive
        {
            get { return _adminScriptServiceRelaytive; }
            set { _adminScriptServiceRelaytive = value; }
        }

        private string _adminScriptService;

        public string AdminScriptService
        {
            get { return _adminScriptService; }
            set { _adminScriptService = value; }
        }

        private string _apiActionRelation;

        public string ApiActionRelation
        {
            get { return _apiActionRelation; }
            set { _apiActionRelation = value; }
        }

        private string _apiAction;

        public string ApiAction
        {
            get { return _apiAction; }
            set { _apiAction = value; }
        }

        private string _blMessageRequestRelation;

        public string BLMessageRequestRelation
        {
            get { return _blMessageRequestRelation; }
            set { _blMessageRequestRelation = value; }
        }

        private string _blMessagesrequest;

        public string BLMessageRequest
        {
            get { return _blMessagesrequest; }
            set { _blMessagesrequest = value; }
        }

        private string _blMessageResponseRelation;

        public string BLMessageResponseRelation
        {
            get { return _blMessageResponseRelation; }
            set { _blMessageResponseRelation = value; }
        }

        private string _blMessagesResponse;

        public string BLMessageResponse
        {
            get { return _blMessagesResponse; }
            set { _blMessagesResponse = value; }
        }

        private string _blMapperConfiguration;

        public string BLMapperConfiguration
        {
            get { return _blMapperConfiguration; }
            set { _blMapperConfiguration = value; }
        }

        private string _blMappingExtension;

        public string BLMappingExtension
        {
            get { return _blMappingExtension; }
            set { _blMappingExtension = value; }
        }

        private string _interfaceCompanyBusiness;

        public string InterfacesCompanyBusiness
        {
            get { return _interfaceCompanyBusiness; }
            set { _interfaceCompanyBusiness = value; }
        }

        private string _implementsCompanyBL;

        public string ImplementsCompanyBL
        {
            get { return _implementsCompanyBL; }
            set { _implementsCompanyBL = value; }
        }

        private string _daMessagesParameter;

        public string DAMessageParameter
        {
            get { return _daMessagesParameter; }
            set { _daMessagesParameter = value; }
        }

        private string _daMessagesResult;

        public string DAMessageResult
        {
            get { return _daMessagesResult; }
            set { _daMessagesResult = value; }
        }


        private string _daMapperConfiguration;

        public string DAMapperConfiguration
        {
            get { return _daMapperConfiguration; }
            set { _daMapperConfiguration = value; }
        }


        private string _daMappingExtension;

        public string DAMappingExtension
        {
            get { return _daMappingExtension; }
            set { _daMappingExtension = value; }
        }

        private string _interfacesCompanyDA;

        public string InterfacesCompanyDA
        {
            get { return _interfacesCompanyDA; }
            set { _interfacesCompanyDA = value; }
        }

        private string _implementsCompanyDA;

        public string ImplementsCompanyDA
        {
            get { return _implementsCompanyDA; }
            set { _implementsCompanyDA = value; }
        }

        #endregion




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
