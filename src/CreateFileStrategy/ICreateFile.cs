using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N8General.ConfigueForm;

namespace N8General.CreateFileStrategy
{
    public interface ICreateFile
    {
        void DoCreateFile(ConfigueModel config);
    }
}
