using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Models
{
    internal record UpgradeLogEntry
    {
        public required string UpgraderName { get; set; }
        public required UpgraderActionType ActionType { get; set; }
        public required bool Success { get; set; }
        public string? Message { get; set; }
    }
}
