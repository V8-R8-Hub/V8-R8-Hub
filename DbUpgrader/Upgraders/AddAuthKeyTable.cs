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
	internal class AddAuthKeyTable : Upgrader {
		public AddAuthKeyTable(IConnectionFactory connectionFactory)
			: base(nameof(AddAuthKeyTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				CREATE TABLE auth_keys (
					id SERIAL PRIMARY KEY,
					key TEXT NOT NULL UNIQUE,
					user_id INTEGER NOT NULL REFERENCES users(id)
				);

				GRANT
					SELECT, INSERT, UPDATE, DELETE
				ON 
					auth_keys
				TO v8_r8_hub_api_group;

				GRANT USAGE ON SEQUENCE
					auth_keys_id_seq
				TO v8_r8_hub_api_group;
			""");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				REVOKE USAGE ON SEQUENCE 
					auth_keys_id_seq
				FROM v8_r8_hub_api_group;
			
				REVOKE 
					SELECT, INSERT, UPDATE, DELETE
				ON
					auth_keys
				FROM v8_r8_hub_api_group;

				DROP TABLE auth_keys;
			""");
		}
	}
}
