using Newtonsoft.Json;

using System;
using System.IO;

namespace InsanityBot.MessageServices.Messages
{
    public class LogEventJsonConverter : JsonConverter<LogEvent>
    {
        public override LogEvent ReadJson(JsonReader reader, Type objectType, LogEvent existingValue, Boolean hasExistingValue, JsonSerializer serializer)
        {
            return ((String)reader.Value).ToLower() switch
            {
                "message_delete" or "messagedelete" => LogEvent.MessageDelete,
                "message_edit" or "messageedit" => LogEvent.MessageEdit,
                "member_join" or "memberjoin" => LogEvent.MemberJoin,
                "member_leave" or "memberleave" => LogEvent.MemberLeave,
                "command" or "commands" => LogEvent.Commands,
                "command_execution" or "commandexecution" => LogEvent.CommandExecution,
                _ => throw new InvalidDataException("Invalid JSON")
            };
        }
        public override void WriteJson(JsonWriter writer, LogEvent value, JsonSerializer serializer)
        {
            writer.WriteValue(value switch
            {
                LogEvent.MessageDelete => "message_delete",
                LogEvent.MessageEdit => "message_edit",
                LogEvent.MemberJoin => "member_join",
                LogEvent.MemberLeave => "member_leave",
                LogEvent.Commands => "commands",
                LogEvent.CommandExecution => "command_execution",
                _ => throw new InvalidProgramException("Non-existent enum field selected")
            });
        }
    }
}
