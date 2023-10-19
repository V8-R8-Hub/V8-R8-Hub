using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Exceptions {
    internal class UnknownUpgradeLevelException : Exception {
        public string UnknownUpgraderLevel { get; } 

        public UnknownUpgradeLevelException(string unknownUpgraderLevel, string message) : base(message) {
            UnknownUpgraderLevel = unknownUpgraderLevel;
        }

        public UnknownUpgradeLevelException(string unknownUpgraderLevel, string message, Exception innerException) : base(message, innerException) {
            UnknownUpgraderLevel = unknownUpgraderLevel;
        }
    }
}
