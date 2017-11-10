using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataTransfer.AccessDatabase.Models
{
    public class Task
    {
        public string TaskName { get; set; }

        public string TaskDisplayName { get; set; }

        public DateTime StartDateTime { get; set; }

        public int RepeatInterval { get; set; }
    }
}
