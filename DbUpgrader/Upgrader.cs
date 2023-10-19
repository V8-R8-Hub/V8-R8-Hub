using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader {
    internal abstract class Upgrader {
        public string Name { get; }
        private readonly IConnectionFactory _connectionFactory;

        protected Upgrader(string name, IConnectionFactory connectionFactory) {
            Name = name;
            _connectionFactory = connectionFactory;
        }

        protected abstract Task UpInternal(NpgsqlConnection connection);
        protected abstract Task DownInternal(NpgsqlConnection connection);

        public async Task Up(NpgsqlConnection connection) {
            await UpInternal(connection);
            await InsertLogEntry(new UpgradeLogEntry() {
                UpgraderName = Name,
                ActionType = UpgraderActionType.Up,
                Success = true,
                Message = null
            });

            await InsertUpgradeTrackerEntry(new UpgradeTrackerEntry {
                UpgraderName = Name
            });
        }

        public async Task Down(NpgsqlConnection connection) {
            await DownInternal(connection);
            await InsertLogEntry(new UpgradeLogEntry() {
                UpgraderName = Name,
                ActionType = UpgraderActionType.Down,
                Success = true,
                Message = null
            });

            await RemoveUpgraderTrackerEntry(Name);
        }

        private async Task InsertLogEntry(UpgradeLogEntry logEntry) {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO upgrade_log (upgrader_name, action_type, success, message) VALUES ($1, $2, $3, $4)",
                await _connectionFactory.GetConnection()
            );
            cmd.Parameters.Add(new() { Value = logEntry.UpgraderName });
            cmd.Parameters.Add(new() { Value = logEntry.ActionType.ToString(), DataTypeName = "upgrade_action_type" });
            cmd.Parameters.Add(new() { Value = logEntry.Success });
            cmd.Parameters.Add(new() { Value = ((object?)logEntry.Message) ?? DBNull.Value });
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task InsertUpgradeTrackerEntry(UpgradeTrackerEntry entry) {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO upgrade_tracker (upgrader_name) VALUES (@UpgraderName)",
                await _connectionFactory.GetConnection()
            );

            cmd.Parameters.AddWithValue("UpgraderName", entry.UpgraderName);
            await cmd.ExecuteNonQueryAsync();
        }

        private async Task RemoveUpgraderTrackerEntry(string upgraderName) {
            using var cmd = new NpgsqlCommand(
                "DELETE FROM upgrade_tracker WHERE upgrader_name = @UpgraderName",
                await _connectionFactory.GetConnection()
            );

            cmd.Parameters.AddWithValue("UpgraderName", upgraderName);
            await cmd.ExecuteNonQueryAsync();
        }
    }
}
