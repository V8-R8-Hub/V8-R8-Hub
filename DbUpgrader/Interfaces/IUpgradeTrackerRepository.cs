using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Interfaces {
    internal interface IUpgradeTrackerRepository {
        IAsyncEnumerable<UpgradeTrackerEntry> GetTrackerEntries();
        Task AddTrackerEntry(UpgradeTrackerEntry entry);
        Task RemoveTrackerEntry(string name);
    }
}
