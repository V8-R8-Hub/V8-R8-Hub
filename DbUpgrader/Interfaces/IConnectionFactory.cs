using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Interfaces {
    internal interface IConnectionFactory {
        Task<NpgsqlConnection> GetConnection();
        string GetDatabaseName();
    }
}
