using DBUpgrader.Interfaces;
using DBUpgrader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;
using DBUpgrader.Extensions;

namespace DbUpgrader.Upgraders {
	internal class AddGameAssetsTable : Upgrader {
		public AddGameAssetsTable(IConnectionFactory connectionFactory)
			: base(nameof(AddGameAssetsTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				CREATE TABLE game_assets (
					id SERIAL PRIMARY KEY,
					game_id INTEGER NOT NULL REFERENCES games(id),
					file_id INTEGER NOT NULL REFERENCES public_files(id),
					path TEXT NOT NULL,
					UNIQUE (game_id, path)
				);

				GRANT
					SELECT, INSERT, UPDATE, DELETE
				ON 
					game_assets
				TO v8_r8_hub_api_group;

				GRANT USAGE ON SEQUENCE
					game_assets_id_seq
				TO v8_r8_hub_api_group;
			""");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				REVOKE USAGE ON SEQUENCE 
					game_assets_id_seq
				FROM v8_r8_hub_api_group;
			
				REVOKE 
					SELECT, INSERT, UPDATE, DELETE
				ON
					game_assets
				FROM v8_r8_hub_api_group;

				DROP TABLE game_assets;
			""");
		}
	}
}
