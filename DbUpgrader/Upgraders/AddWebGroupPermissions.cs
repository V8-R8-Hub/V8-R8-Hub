using DBUpgrader;
using DBUpgrader.Extensions;
using DBUpgrader.Interfaces;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbUpgrader.Upgraders {
	internal class AddWebGroupPermissions : Upgrader {
		public AddWebGroupPermissions(IConnectionFactory connectionFactory)
			: base(nameof(AddWebGroupPermissions), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			var commandBuilder = new NpgsqlCommandBuilder();

			await connection.ExecuteQuery(@$"
				GRANT SELECT, 
					INSERT, 
					UPDATE, 
					DELETE 
				ON 
					public_files,
					games,
					sessions
				TO v8_r8_hub_api_group;

				GRANT USAGE ON SEQUENCE
					games_id_seq,
					sessions_id_seq,
					public_files_id_seq
				TO v8_r8_hub_api_group;

				GRANT CONNECT ON DATABASE
					{commandBuilder.QuoteIdentifier(connection.Database)}
				TO v8_r8_hub_api_group
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			var commandBuilder = new NpgsqlCommandBuilder();
			await connection.ExecuteQuery(@$"
				REVOKE SELECT, INSERT, UPDATE, DELETE ON
					public_files,
					games,
					sessions
				FROM v8_r8_hub_api_group;

				REVOKE USAGE ON SEQUENCE 
					public_files_id_seq,
					games_id_seq,
					sessions_id_seq
				FROM v8_r8_hub_api_group;

				REVOKE CONNECT ON DATABASE 
					{commandBuilder.QuoteIdentifier(connection.Database)}
				FROM v8_r8_hub_api_group;
			");
		}
	}
}
