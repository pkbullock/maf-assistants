using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MAF.Assistants.Interfaces
{
    internal interface ProcessRunners
    {
        /// <summary>
        /// Start the process runner and entry point for the application
        /// </summary>
        static abstract Task StartChat<T>() where T : RunChatModel, new();
    }
}
