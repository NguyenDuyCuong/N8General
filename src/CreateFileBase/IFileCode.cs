using N8General.CreateFileBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8General.CreateFileBase
{
    public interface IFileCode
    {
        string Path { get; }
        IFileTemplate Template { get; }

        void WriteToDisk();
    }
}
