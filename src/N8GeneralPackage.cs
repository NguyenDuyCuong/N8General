using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Interop;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using N8General.ConfigueForm;
using N8General.CreateFileStrategy;
using N8General.Helpers;
using System.Windows.Forms;

namespace N8General
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidN8GeneralPkgString)]
    public sealed class N8GeneralPackage : ExtensionPointPackage
    {
        private static N8GeneralPackage _instance;
        public static N8GeneralPackage Instance => _instance;

        public static DTE2 _dte;

        protected override void Initialize()
        {
            _dte = GetService(typeof(DTE)) as DTE2;

            Logger.Initialize(this, Vsix.Name);

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(PackageGuids.guidN8GeneralCmdSet, PackageIds.cmdidMyCommand);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }

            _instance = this;
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            var solution = _dte.Solution;
            if (solution == null && !solution.IsOpen)
                return;

            var rootFolder = ProjectHelpers.GetSolutionRootFolder();
            var config = PromptForConfigue(rootFolder);

            if (config == null)
                return;


            try
            {
                CreateFileContext context = new CreateFileContext(new CreateRequestFile());
                //context.ExecuteCreateFile(config);

                context = new CreateFileContext(new CreateResponseFile());
                //context.ExecuteCreateFile(config);

                context = new CreateFileContext(new CreateParameterFile());
                //context.ExecuteCreateFile(config);

                context = new CreateFileContext(new CreateResultFile());
                //context.ExecuteCreateFile(config);

                context = new CreateFileContext(new CreateAPIFunction());
                context.ExecuteCreateFile(config);
            }
            catch (Exception ex)
            {
                Logger.Log(ex.Message);
            }
        }
        
        
        private ConfigueModel PromptForConfigue(string folder)
        {
            ConfigueModel result = null;
            var dialog = new ConfigueDialog();
            var vm = dialog.DataContext as ConfigueViewModel;

            var hwnd = new IntPtr(_dte.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(hwnd);
            if (hwndSource != null)
            {
                var window = (System.Windows.Window)hwndSource.RootVisual;
                dialog.Owner = window;
            }

            vm?.InitDialog(folder);
            
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                result = vm?.Model;
            }

            return result;
        }

        private static string FindFolder(object item)
        {
            if (item == null)
                return null;

            Window2 window = _dte.ActiveWindow as Window2;

            if (window != null && window.Type == vsWindowType.vsWindowTypeDocument)
            {
                // if a document is active, use the document's containing directory
                Document doc = _dte.ActiveDocument;
                if (doc != null && !string.IsNullOrEmpty(doc.FullName))
                {
                    ProjectItem docItem = _dte.Solution.FindProjectItem(doc.FullName);

                    if (docItem != null && docItem.Properties != null)
                    {
                        string fileName = docItem.Properties.Item("FullPath").Value.ToString();
                        if (File.Exists(fileName))
                            return Path.GetDirectoryName(fileName);
                    }
                }
            }

            string folder = null;

            ProjectItem projectItem = item as ProjectItem;
            if (projectItem != null && EnvDTE.Constants.vsProjectItemKindVirtualFolder == projectItem.Kind)
            {
                var items = projectItem.ProjectItems;
                foreach (ProjectItem it in items)
                {
                    if (File.Exists(it.FileNames[1]))
                    {
                        folder = Path.GetDirectoryName(it.FileNames[1]);
                        break;
                    }
                }
            }
            else
            {
                Project project = item as Project;
                if (projectItem != null)
                {
                    string fileName = projectItem.FileNames[1];

                    if (File.Exists(fileName))
                    {
                        folder = Path.GetDirectoryName(fileName);
                    }
                    else
                    {
                        folder = fileName;
                    }


                }
                else if (project != null)
                {
                    folder = project.GetRootFolder();
                }
            }
            return folder;
        }
    }
}