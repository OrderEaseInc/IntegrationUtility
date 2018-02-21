using System;

namespace DataTransfer.AccessDatabase.Models
{
    public class Task
    {
        public string TaskName { get; set; }

        public string TaskDisplayName { get; set; }

        public DateTime StartDateTime { get; set; }

        public int RepeatInterval { get; set; }

        public string LastExecuteStatus { get; set; }

        public string Status { get; set; }

        public DateTime? ExecutionStartDateTime { get; set; }

        public DateTime? ExecutionEndDateTime { get; set; }

        public TimeSpan? ExecutionDuration { get; set; }

        public string ExternalExecutable { get; set; }

        public string JobParameters { get; set; }
    }
}
