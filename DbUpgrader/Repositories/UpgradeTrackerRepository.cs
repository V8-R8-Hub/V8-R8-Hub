using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Repositories {
    internal class UpgradeTrackerRepository : IUpgradeTrackerRepository {

        private readonly IConnectionFactory _connectionFactory;

        public UpgradeTrackerRepository(IConnectionFactory connection) {
            _connectionFactory = connection;
        }

        public async Task AddTrackerEntry(UpgradeTrackerEntry entry) {
            using var cmd = new NpgsqlCommand(
                "INSERT INTO upgrade_tracker (upgrader_name) VALUES (@UpgraderName)",
                await _connectionFactory.GetConnection()
            );

            cmd.Parameters.AddWithValue("UpgraderName", entry.UpgraderName);
            await cmd.ExecuteNonQueryAsync();
        }

        public async Task RemoveTrackerEntry(string name) {
            using var cmd = new NpgsqlCommand(
                "DELETE FROM upgrade_tracker (upgrader_name) WHERE upgrader_name = @UpgraderName",
                await _connectionFactory.GetConnection()
            );

            cmd.Parameters.AddWithValue("UpgraderName", name);
            await cmd.ExecuteNonQueryAsync();
        }

        public async IAsyncEnumerable<UpgradeTrackerEntry> GetTrackerEntries() {
            using var cmd = new NpgsqlCommand(
                "SELECT (upgrader_name) FROM upgrade_tracker ORDER BY id ASC",
                await _connectionFactory.GetConnection()
            );

            await using var reader = await cmd.ExecuteReaderAsync();
            while (reader.Read()) {
                yield return new UpgradeTrackerEntry {
                    UpgraderName = reader.GetString(0)
                };
            }
        }
    }
}
