using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DBUpgrader.Repositories {
    internal class ConfigRepository : IConfigRepository {
        private static readonly string CONFIG_FILE_PATH = "DBUpgrader.config.json";
        
        PersistentDataBaseConfig? _cachedConfig;

        public async Task<PersistentDataBaseConfig> GetConfig() {
            if (_cachedConfig == null) {
                _cachedConfig = JsonSerializer.Deserialize<PersistentDataBaseConfig>(
                    await File.ReadAllTextAsync(CONFIG_FILE_PATH)
                );
                if (_cachedConfig == null) {
                    throw new Exception("Could not read config file DBUpgrader.config.json");
                }
            }
            return _cachedConfig;
        }

        public async Task SetConfig(PersistentDataBaseConfig config) {
            await File.WriteAllTextAsync(CONFIG_FILE_PATH, JsonSerializer.Serialize(config));
            _cachedConfig = null;
        }
    }
}
