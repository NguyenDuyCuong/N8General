using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using N8General.ConfigueForm;

namespace N8General.CreateFileStrategy
{
    public class CreateFileContext
    {
        private readonly ICreateFile _createFileStrategy;

        public CreateFileContext(ICreateFile strategy)
        {
            _createFileStrategy = strategy;
        }

        public void ExecuteCreateFile(ConfigueModel config)
        {
            _createFileStrategy.DoCreateFile(config);
        }
    }
}
