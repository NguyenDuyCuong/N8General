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

namespace N8General
{
    [PackageRegistration(UseManagedResourcesOnly = true)]
    [InstalledProductRegistration("#110", "#112", Vsix.Version, IconResourceID = 400)]
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(PackageGuids.guidN8GeneralPkgString)]
    public sealed class N8GeneralPackage : ExtensionPointPackage
    {
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

            //string input = PromptForConfigue(folder).TrimStart('/', '\\').Replace("/", "\\");
            var config = PromptForConfigue(folder);

            if (config == null)
                return;

            CreateFileContext context = new CreateFileContext(new CreateRequestFile());
            context.ExecuteCreateFile(config);
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

        private ConfigueModel PromptForConfigue(string folder)
        {
            DirectoryInfo dir = new DirectoryInfo(folder);
            //var dialog = new FileNameDialog(dir.Name);
            var dialog = new ConfigueDialog();
            var vm = dialog.DataContext as ConfigueViewModel;

            var hwnd = new IntPtr(_dte.MainWindow.HWnd);
            var hwndSource = HwndSource.FromHwnd(hwnd);
            if (hwndSource != null)
            {
                var window = (System.Windows.Window)hwndSource.RootVisual;
                dialog.Owner = window;
            }

            vm?.InitDialog(dir.FullName);
            var result = dialog.ShowDialog();
            return vm?.Model;
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