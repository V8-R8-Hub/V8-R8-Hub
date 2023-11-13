using DBUpgrader.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Extensions {
    internal static class ExecuteQueryExtension {
        public static async Task ExecuteQuery(this NpgsqlConnection connection, [StringSyntax("sql")] string command) {
            using var cmd = new NpgsqlCommand(
                command,
                connection
            );
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
