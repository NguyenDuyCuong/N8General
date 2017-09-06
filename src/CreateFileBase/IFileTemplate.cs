using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N8General.CreateFileBase
{
    public interface IFileTemplate
    {
        string GetTemplate(string path);
    }
}
