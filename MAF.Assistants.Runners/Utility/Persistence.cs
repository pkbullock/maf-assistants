using Microsoft.Agents.AI;
using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace MAF.Assistants.Utility
{
    public static class Persistence
    {
        private static string TempFileLocation = "C:\\Agent-Logs";

        /// <summary>
        /// Saves the state of the specified <see cref="AgentThread"/> to a file in JSON format.
        /// </summary>
        /// <remarks>The method serializes the state of the provided <see cref="AgentThread"/> into JSON
        /// format and writes it to a file  in the temporary file location. The default file name is
        /// "agent_thread.json", but a custom file name can be provided.</remarks>
        /// <param name="thread">The <see cref="AgentThread"/> instance representing the conversation to save. Cannot be <see
        /// langword="null"/>.</param>
        /// <param name="fileName">The name of the file to save the conversation to. Defaults to "agent_thread.json" if not specified.</param>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        public static async Task SaveConversationToFileAsync(AgentThread thread, string fileName = "agent_thread.json")
        {
            
            // Serialize the thread state
            string serializedJson = thread.Serialize(JsonSerializerOptions.Web).GetRawText();

            // Example: save to a local file (replace with DB or blob storage in production)
            string filePath = Path.Combine(TempFileLocation, fileName);
            await File.WriteAllTextAsync(filePath, serializedJson);

            WriteOut.MsgBlankLine();
            WriteOut.MsgCyan($"Conversation saved to {filePath}");
        }

        /// <summary>
        /// Asynchronously loads a conversation from a specified JSON file and restores the thread state for the given
        /// agent.
        /// </summary>
        /// <remarks>The method reads the serialized conversation from the specified file, deserializes
        /// it, and restores the thread state for the provided agent. The file is expected to be located in the
        /// temporary file directory.</remarks>
        /// <param name="agent">The <see cref="AIAgent"/> instance for which the conversation thread will be restored.</param>
        /// <param name="fileName">The name of the JSON file containing the serialized conversation. Defaults to "agent_thread.json".</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the restored <see
        /// cref="AgentThread"/> instance representing the loaded conversation.</returns>
        /// <exception cref="FileNotFoundException">Thrown if the specified file does not exist at the expected location.</exception>
        public static async Task<AgentThread> LoadConversationFromFileAsync(AIAgent agent, string fileName = "agent_thread.json")
        {
            string filePath = Path.Combine(TempFileLocation, fileName);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("The specified conversation file was not found.", filePath);
            }

            // Read the serialized JSON from the file
            string serializedJson = await File.ReadAllTextAsync(filePath);
            JsonElement reloaded = JsonSerializer.Deserialize<JsonElement>(serializedJson, JsonSerializerOptions.Web);

            // Deserialize to restore the thread state
            var thread = agent.DeserializeThread(reloaded, JsonSerializerOptions.Web);
            
            WriteOut.MsgBlankLine();
            WriteOut.MsgCyan($"Conversation loaded from {filePath}");
            
            return thread;
        }
    }
}
