using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.Win32;
using EnvDTE80;
using EnvDTE;
using N8General.Helpers;
using Microsoft.VisualStudio.Text;
using System.IO;
using System.Windows.Interop;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace N8General
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the
    /// IVsPackage interface and uses the registration attributes defined in the framework to
    /// register itself and its components with the shell. These attributes tell the pkgdef creation
    /// utility what data to put into .pkgdef file.
    /// </para>
    /// <para>
    /// To get loaded into VS, the package must be referred by &lt;Asset Type="Microsoft.VisualStudio.VsPackage" ...&gt; in .vsixmanifest file.
    /// </para>
    /// </remarks>
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)] // Info on this package for Help/About
    [Guid(N8GeneralPackage.PackageGuidString)]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "pkgdef, VS and vsixmanifest are valid VS terms")]
    public sealed class N8GeneralPackage : Package
    {
        /// <summary>
        /// N8GeneralPackage GUID string.
        /// </summary>
        public const string PackageGuidString = "5a1403a6-1d1e-4606-98f0-6c6331982618";

        /// <summary>
        /// Initializes a new instance of the <see cref="N8GeneralPackage"/> class.
        /// </summary>
        public N8GeneralPackage()
        {
            // Inside this method you can place any initialization code that does not require
            // any Visual Studio service because at this point the package object is created but
            // not sited yet inside Visual Studio environment. The place to do all the other
            // initialization is the Initialize method.
        }

        #region Package Members

        public static DTE2 _dte;

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            _dte = GetService(typeof(DTE)) as DTE2;

            OleMenuCommandService mcs = GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                CommandID menuCommandID = new CommandID(PackageGuids.guidN8GeneralCmdSet, PackageIds.cmdidMyCommand);
                var menuItem = new OleMenuCommand(MenuItemCallback, menuCommandID);
                mcs.AddCommand(menuItem);
            }
        }

        private async void MenuItemCallback(object sender, EventArgs e)
        {
            var item = ProjectHelpers.GetSelectedItem();
            var folder = FindFolder(item);

            if (string.IsNullOrEmpty(folder) || !Directory.Exists(folder))
                return;

            var selectedItem = item as ProjectItem;
            var selectedProject = item as Project;
            var project = selectedItem?.ContainingProject ?? selectedProject ?? ProjectHelpers.GetActiveProject();

            if (project == null)
                return;

            string input = PromptForFileName(folder).TrimStart('/', '\\').Replace("/", "\\");

            if (string.IsNullOrEmpty(input))
                return;

            string[] parsedInputs = GetParsedInput(input);

            foreach (string inputItem in parsedInputs)
            {
                input = inputItem;

                if (input.EndsWith("\\", StringComparison.Ordinal))
                {
                    input = input + "__dummy__";
                }

                var file = new FileInfo(Path.Combine(folder, input));
                string dir = file.DirectoryName;

                PackageUtilities.EnsureOutputPath(dir);

                if (!file.Exists)
                {
                    int position = await WriteFile(project, file.FullName);

                    try
                    {
                        ProjectItem projectItem = null;
                        var projItem = item as ProjectItem;
                        if (projItem != null)
                        {
                            if (EnvDTE.Constants.vsProjectItemKindVirtualFolder == projItem.Kind)
                            {
                                projectItem = projItem.ProjectItems.AddFromFile(file.FullName);
                            }
                        }
                        if (projectItem == null)
                        {
                            projectItem = project.AddFileToProject(file);
                        }

                        if (file.FullName.EndsWith("__dummy__"))
                        {
                            projectItem?.Delete();
                            continue;
                        }

                        VsShellUtilities.OpenDocument(this, file.FullName);

                        // Move cursor into position
                        if (position > 0)
                        {
                            var view = ProjectHelpers.GetCurentTextView();

                            if (view != null)
                                view.Caret.MoveTo(new SnapshotPoint(view.TextBuffer.CurrentSnapshot, position));
                        }

                        _dte.ExecuteCommand("SolutionExplorer.SyncWithActiveDocument");
                        _dte.ActiveDocument.Activate();
                    }
                    catch (Exception ex)
                    {
                        Logger.Log(ex);
                    }
                }
                else
                {
                    System.Windows.Forms.MessageBox.Show("The file '" + file + "' already exist.");
                }
            }
        }

        private static async Task<int> WriteFile(Project project, string file)
        {
            string extension = Path.GetExtension(file);
            string template = await TemplateMap.GetTemplateFilePath(project, file);

            if (!string.IsNullOrEmpty(template))
            {
                int index = template.IndexOf('$');

                if (index > -1)
                {
                    template = template.Remove(index, 1);
                }

                await WriteToDisk(file, template);
                return index;
            }

            await WriteToDisk(file, string.Empty);

            return 0;
        }

        static string[] GetParsedInput(string input)
        {
            // var tests = new string[] { "file1.txt", "file1.txt, file2.txt", ".ignore", ".ignore.(old,new)", "license", "folder/",
            //    "folder\\", "folder\\file.txt", "folder/.thing", "page.aspx.cs", "widget-1.(html,js)", "pages\\home.(aspx, aspx.cs)",
            //    "home.(html,js), about.(html,js,css)", "backup.2016.(old, new)", "file.(txt,txt,,)", "file_@#d+|%.3-2...3^&.txt" };
            Regex pattern = new Regex(@"[,]?([^(,]*)([\.\/\\]?)[(]?((?<=[^(])[^,]*|[^)]+)[)]?");
            List<string> results = new List<string>();
            Match match = pattern.Match(input);

            while (match.Success)
            {
                // Always 4 matches w. Group[3] being the extension, extension list, folder terminator ("/" or "\"), or empty string
                string path = match.Groups[1].Value.Trim() + match.Groups[2].Value;
                string[] extensions = match.Groups[3].Value.Split(',');

                foreach (string ext in extensions)
                {
                    string value = path + ext.Trim();

                    // ensure "file.(txt,,txt)" or "file.txt,,file.txt,File.TXT" returns as just ["file.txt"]
                    if (value != "" && !value.EndsWith(".", StringComparison.Ordinal) && !results.Contains(value, StringComparer.OrdinalIgnoreCase))
                    {
                        results.Add(value);
                    }
                }
                match = match.NextMatch();
            }
            return results.ToArray();
        }


        private static async System.Threading.Tasks.Task WriteToDisk(string file, string content)
        {
            using (var writer = new StreamWriter(file, false, GetFileEncoding(file)))
            {
                await writer.WriteAsync(content);
            }
        }

        private static Encoding GetFileEncoding(string file)
        {
            string[] noBom = { ".cmd", ".bat", ".json" };
            string ext = Path.GetExtension(file).ToLowerInvariant();

            if (noBom.Contains(ext))
                return new UTF8Encoding(false);

            return new UTF8Encoding(true);
        }

        private string PromptForFileName(string folder)
        {
            DirectoryInfo dir = new DirectoryInfo(folder);
            var dialog = new ConfigueDialog(dir.Name);

            var hwnd = new IntPtr(_dte.MainWindow.HWnd);
            var window = (System.Windows.Window)HwndSource.FromHwnd(hwnd).RootVisual;
            dialog.Owner = window;

            var result = dialog.ShowDialog();
            return (result.HasValue && result.Value) ? dialog.Input : string.Empty;
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
        #endregion
    }
}
