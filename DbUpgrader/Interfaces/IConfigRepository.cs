using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Interfaces {
    internal interface IConfigRepository {
        Task<PersistentDataBaseConfig> GetConfig();
        Task SetConfig(PersistentDataBaseConfig config);
    }
}
