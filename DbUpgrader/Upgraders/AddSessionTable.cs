using DBUpgrader;
using DBUpgrader.Extensions;
using DBUpgrader.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbUpgrader.Upgraders {
	internal class AddSessionTable : Upgrader {
		public AddSessionTable(IConnectionFactory connectionFactory)
			: base(nameof(AddSessionTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				CREATE TABLE sessions (
					id SERIAL PRIMARY KEY,
					session_key TEXT NOT NULL UNIQUE
				);
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				DROP TABLE sessions;
			");
		}

		
	}
}
