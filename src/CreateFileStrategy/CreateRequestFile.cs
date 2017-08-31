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

namespace N8General.CreateFileStrategy
{
    public class CreateRequestFile : CreateFileBase, ICreateFile
    {
        private readonly string FileExtention = ".cs";

        public async void DoCreateFile(ConfigueModel config)
        {
            var file = new FileInfo(Path.Combine(config.SolutionFolder, config.BussinessMessageFolder, config.MethodFolder, config.MethodName + FileExtention));
            var dir = file.DirectoryName;

            PackageUtilities.EnsureOutputPath(dir);
            if (!file.Exists)
            {
                var bussinessProject = ProjectHelpers.GetProject("");
                int position = await WriteFile(bussinessProject, file.FullName);
            }
            else
            {

            }
            //int position = await WriteFile(project, file.FullName);

            //ProjectItem projectItem = null;
            //var projItem = item as ProjectItem;
            //if (projItem != null)
            //{
            //    if (EnvDTE.Constants.vsProjectItemKindVirtualFolder == projItem.Kind)
            //    {
            //        projectItem = projItem.ProjectItems.AddFromFile(file.FullName);
            //    }
            //}
            //if (projectItem == null)
            //{
            //    projectItem = project.AddFileToProject(file);
            //}

            //VsShellUtilities.OpenDocument(this, file.FullName);

            //_dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
            //_dte.ActiveDocument.Activate();

            //string[] parsedInputs = GetParsedInput(input);

            //foreach (string inputItem in parsedInputs)
            //{
            //    input = inputItem;

            //    if (input.EndsWith("\\", StringComparison.Ordinal))
            //    {
            //        input = input + "__dummy__";
            //    }

            //    var file = new FileInfo(Path.Combine(folder, input));
            //    string dir = file.DirectoryName;

            //    PackageUtilities.EnsureOutputPath(dir);

            //    if (!file.Exists)
            //    {
            //        int position = await WriteFile(project, file.FullName);

            //        try
            //        {
            //            ProjectItem projectItem = null;
            //            var projItem = item as ProjectItem;
            //            if (projItem != null)
            //            {
            //                if (EnvDTE.Constants.vsProjectItemKindVirtualFolder == projItem.Kind)
            //                {
            //                    projectItem = projItem.ProjectItems.AddFromFile(file.FullName);
            //                }
            //            }
            //            if (projectItem == null)
            //            {
            //                projectItem = project.AddFileToProject(file);
            //            }

            //            if (file.FullName.EndsWith("__dummy__"))
            //            {
            //                projectItem?.Delete();
            //                continue;
            //            }

            //            VsShellUtilities.OpenDocument(this, file.FullName);

            //            // Move cursor into position
            //            if (position > 0)
            //            {
            //                var view = ProjectHelpers.GetCurentTextView();

            //                if (view != null)
            //                    view.Caret.MoveTo(new SnapshotPoint(view.TextBuffer.CurrentSnapshot, position));
            //            }

            //            _dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
            //            _dte.ActiveDocument.Activate();
            //        }
            //        catch (Exception ex)
            //        {
            //            Logger.Log(ex);
            //        }
            //    }
            //    else
            //    {
            //        System.Windows.Forms.MessageBox.Show("The file '" + file + "' already exist.");
            //    }
            //}
        }
    }
}
