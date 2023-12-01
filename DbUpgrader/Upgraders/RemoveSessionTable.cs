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
	class RemoveSessionTable : Upgrader {
		public RemoveSessionTable(IConnectionFactory connectionFactory)
				: base(nameof(RemoveSessionTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				REVOKE USAGE ON SEQUENCE 
					sessions_id_seq
				FROM v8_r8_hub_api_group;
				
				REVOKE SELECT, INSERT, UPDATE, DELETE ON
					sessions
				FROM v8_r8_hub_api_group;

				DROP TABLE sessions;
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				CREATE TABLE sessions (
					id SERIAL PRIMARY KEY,
					session_key TEXT NOT NULL UNIQUE
				);

				GRANT USAGE ON SEQUENCE
					sessions_id_seq
				TO v8_r8_hub_api_group;

				GRANT SELECT, 
					INSERT, 
					UPDATE, 
					DELETE 
				ON
					sessions
				TO v8_r8_hub_api_group;
			");
		}
	}
}
