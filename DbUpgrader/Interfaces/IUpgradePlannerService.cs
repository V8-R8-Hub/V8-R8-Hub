using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Interfaces {
    internal interface IUpgradePlannerService {
        Task<IEnumerable<UpgraderAction>> PlanUpgradePath(string targetUpgrader);
        Task<IEnumerable<UpgraderAction>> PlanUpgradePathToLatest();
    }
}
