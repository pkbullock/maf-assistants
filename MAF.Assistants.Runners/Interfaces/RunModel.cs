using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Interfaces
{
    internal interface RunModel
    {
        /// <summary>
        /// Entry point to start a model process
        /// </summary>
        abstract Task StartAsync(ChatClient client);
    }
}
