using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Models {
    internal record UpgradeTrackerEntry {
        public required string UpgraderName { get; set; } 
    }
}
