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
    public class CreateAPIFunction : CreateFileBase, ICreateFile
    {
        private readonly string FileExtention = "Controller.cs";
        private readonly string TemplateExtention = ".controller";

        public async void DoCreateFile(ConfigueModel config)
        {
            var file = new FileInfo(Path.Combine(config.SolutionFolder, config.APIControllerFolder, config.APIControllerName + FileExtention));
            var dir = file.DirectoryName;

            PackageUtilities.EnsureOutputPath(dir);
            var apiProject = ProjectHelpers.GetProject(config.APIFolder);

            if (!file.Exists)
            {
                int position = await WriteFile(apiProject, file.FullName, TemplateExtention);

                try
                {
                    ProjectItem projectItem = apiProject.AddFileToProject(file);

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
                var itemController = ProjectHelpers.GetProectItemInProject(apiProject, file.Name);
                if (itemController != null)
                {
                    string relative = PackageUtilities.MakeRelative(apiProject.GetRootFolder(), Path.GetDirectoryName(file.FullName));
                    var fullNameClass = String.Format("{0}.{1}.{2}", config.APIFolder, relative, Path.GetFileNameWithoutExtension(file.Name)).Replace("..", ".");

                    VsShellUtilities.OpenDocument(N8GeneralPackage.Instance, file.FullName);

                    TextSelection sel = (TextSelection)N8GeneralPackage._dte.ActiveDocument.Selection;

                    var controllerClass = ProjectHelpers.FindClass(apiProject, fullNameClass);
                    //var codeFunc = controllerClass.AddFunction(config.MethodName, vsCMFunction.vsCMFunctionFunction, vsCMTypeRef.vsCMTypeRefObject, -1, vsCMAccess.vsCMAccessPublic);
                    //codeFunc.AddAttribute("Authorize", "", -1);
                    //codeFunc.AddAttribute("HttpPost", "", -1);

                    TextPoint tp = controllerClass.GetEndPoint(vsCMPart.vsCMPartBody);
                    EditPoint ep = tp.CreateEditPoint();
                    ep.Insert("return _business.DeletePermissionSetById(request);");

                    tp.CreateEditPoint().SmartFormat(ep);
                }
            }
        }
    }
}