using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Models {
    internal record UpgraderAction {
        public required UpgraderActionType UpgraderActionType { get; set; }
        public required Upgrader Upgrader { get; set; } 
    }
}
