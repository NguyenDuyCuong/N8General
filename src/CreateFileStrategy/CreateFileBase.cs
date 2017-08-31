using EnvDTE;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace N8General.CreateFileStrategy
{
    public class CreateFileBase
    {
        protected async Task<int> WriteFile(Project project, string file)
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

        private async System.Threading.Tasks.Task WriteToDisk(string file, string content)
        {
            using (var writer = new StreamWriter(file, false, GetFileEncoding(file)))
            {
                await writer.WriteAsync(content);
            }
        }

        private Encoding GetFileEncoding(string file)
        {
            string[] noBom = { ".cmd", ".bat", ".json" };
            string ext = Path.GetExtension(file).ToLowerInvariant();

            if (noBom.Contains(ext))
                return new UTF8Encoding(false);

            return new UTF8Encoding(true);
        }

        private string[] GetParsedInput(string input)
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

    }
}
