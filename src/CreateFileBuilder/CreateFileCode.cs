using N8General.CreateFileBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8General.CreateFileBuilder
{
    public abstract class CreateFileCode : IFileCode
    {
        public string Path { get; private set; }

        public IFileTemplate Template => new TemplateC();

        public void WriteToDisk()
        {
            throw new NotImplementedException();
        }
    }
}
