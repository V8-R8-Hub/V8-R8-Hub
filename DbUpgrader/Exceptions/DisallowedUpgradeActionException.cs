using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Exceptions {
    internal class DisallowedUpgradeActionException : Exception {
        public UpgraderAction OffendingAction { get; }
        
        public DisallowedUpgradeActionException(UpgraderAction offendingAction, string message) : base(message) {
            OffendingAction = offendingAction;
        }

        public DisallowedUpgradeActionException(UpgraderAction offendingAction, string message, Exception? innerException) : base(message, innerException) {
            OffendingAction = offendingAction;
        }
    }
}
