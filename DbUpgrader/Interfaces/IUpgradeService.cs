using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Interfaces {
    internal interface IUpgradeService {
        Task UpgradeTo(string upgraderName, UpgradeConfig config);
        Task UpgradeToLatest(UpgradeConfig config);
    }
}
