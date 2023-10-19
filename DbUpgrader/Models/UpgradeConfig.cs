using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Models {
    internal record UpgradeConfig {
        public required bool AllowDown { get; set; }
    }
}
