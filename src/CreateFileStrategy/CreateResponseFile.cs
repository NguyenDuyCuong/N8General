using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnvDTE;
using Microsoft.VisualStudio.Shell;
using N8General.ConfigueForm;
using N8General.Helpers;
using Microsoft.VisualStudio.Text;

namespace N8General.CreateFileStrategy
{
    public class CreateResponseFile : CreateFileBase, ICreateFile
    {
        private readonly string FileExtention = "Response.cs";
        private readonly string TemplateExtention = ".response";

        public async void DoCreateFile(ConfigueModel config)
        {
            var file = new FileInfo(Path.Combine(config.SolutionFolder, config.BussinessMessageFolder, config.MethodFolder, config.MethodName + FileExtention));
            var dir = file.DirectoryName;

            PackageUtilities.EnsureOutputPath(dir);
            if (!file.Exists)
            {
                var bussinessProject = ProjectHelpers.GetProject(config.BussinessFolder);
                int position = await WriteFile(bussinessProject, file.FullName, TemplateExtention);

                try
                {
                    ProjectItem projectItem = bussinessProject.AddFileToProject(file);

                    VsShellUtilities.OpenDocument(N8GeneralPackage.Instance, file.FullName);

                    // Move cursor into position
                    if (position > 0)
                    {
                        var view = ProjectHelpers.GetCurentTextView();

                        if (view != null)
                            view.Caret.MoveTo(new SnapshotPoint(view.TextBuffer.CurrentSnapshot, position));
                    }

                    N8GeneralPackage._dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
                    N8GeneralPackage._dte.ActiveDocument.Activate();
                }
                catch (Exception ex)
                {
                    Logger.Log(ex);
                }
            }
            else
            {

            }
        }
    }
}
