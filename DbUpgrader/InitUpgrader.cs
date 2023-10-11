using DBUpgrader.Extensions;
using DBUpgrader.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader {
    class InitUpgrader : Upgrader {
        public InitUpgrader(IConnectionFactory connectionFactory)
            : base(nameof(InitUpgrader), connectionFactory) { }

        protected override Task DownInternal(NpgsqlConnection connection) {
            return Task.CompletedTask;
        }

        protected override Task UpInternal(NpgsqlConnection connection) {
            return Task.CompletedTask;
        }
    }
}
