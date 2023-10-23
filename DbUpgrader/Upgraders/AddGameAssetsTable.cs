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
			await connection.ExecuteQuery(@"
				CREATE TABLE game_assets (
					id SERIAL PRIMARY KEY,
					game_id INTEGER REFERENCES games(id),
					file_id INTEGER REFERENCES public_files(id),
					path TEXT,
					UNIQUE (game_id, path)
				);
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				DROP TABLE game_assets;
			");
		}
	}
}
