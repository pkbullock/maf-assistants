using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static MAF.Assistants.Utility.Clients;

namespace MAF.Assistants.Interfaces
{
    internal interface RunChatModel
    {
        public bool RequiresTools { get; init; }

        /// <summary>
        /// Entry point to start a model process
        /// </summary>
        abstract Task StartAsync(ChatClient client);
    }
}
