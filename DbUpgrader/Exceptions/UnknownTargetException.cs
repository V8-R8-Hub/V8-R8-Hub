using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Exceptions {
    internal class UnknownTargetException : Exception {
        public string RequestedUpgrader { get; }

        public UnknownTargetException(string requestedUpgrader, string message) : base(message) {
            RequestedUpgrader = requestedUpgrader;
        }

        public UnknownTargetException(string requestedUpgrader, string message, Exception innerException) : base(message, innerException) {
        
        }
    }
}
