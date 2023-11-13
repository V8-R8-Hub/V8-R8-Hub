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
	internal class AddUserTable : Upgrader {
		public AddUserTable(IConnectionFactory connectionFactory) : 
			base(nameof(AddUserTable), connectionFactory) {
		}

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				CREATE TABLE users (
					id SERIAL PRIMARY KEY
				);
				GRANT
					SELECT, INSERT, UPDATE, DELETE
				ON 
					users
				TO v8_r8_hub_api_group;
				
				GRANT USAGE ON SEQUENCE
					users_id_seq
				TO v8_r8_hub_api_group;
				""");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				REVOKE USAGE ON SEQUENCE 
					users_id_seq
				FROM v8_r8_hub_api_group;
			
				REVOKE 
					SELECT, INSERT, UPDATE, DELETE
				ON
					users
				FROM v8_r8_hub_api_group;

				DROP TABLE users;
				""");
		}
	}
}
