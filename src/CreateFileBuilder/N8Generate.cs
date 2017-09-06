using N8General.CreateFileBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8General.CreateFileBuilder
{
    public class N8Generate
    {
        private List<IFileCode> _fileCodes = new List<IFileCode>();

        public void AddFileCode(IFileCode file)
        {
            _fileCodes.Add(file);
        }

        public void DoGenerate()
        {

        }
    }
}
