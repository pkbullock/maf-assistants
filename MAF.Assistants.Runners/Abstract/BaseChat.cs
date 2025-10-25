using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Abstract
{
    internal abstract class BaseChat
    {
        public bool RequiresTools { get; init; }

        public BaseChat()
        {
            RequiresTools = false;
        }
    }
}
