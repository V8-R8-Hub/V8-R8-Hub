using DBUpgrader.Interfaces;
using DBUpgrader;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBUpgrader.Extensions;

namespace DbUpgrader.Upgraders {
	internal class AddGameTable : Upgrader {
		public AddGameTable(IConnectionFactory connectionFactory)
			: base(nameof(AddGameTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				CREATE TABLE games (
					id SERIAL PRIMARY KEY,
					public_id UUID UNIQUE DEFAULT gen_random_uuid(),
					name TEXT,
					description TEXT,
					thumbnail_file_id INTEGER REFERENCES public_files(id),
					game_file_id INTEGER REFERENCES public_files(id)
				);
				CREATE INDEX games_name_idx ON games(name);
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				DROP TABLE games;
			");
		}


	}
}
