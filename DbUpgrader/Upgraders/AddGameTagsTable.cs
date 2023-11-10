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
	internal class AddGameTagsTable : Upgrader {
		public AddGameTagsTable(IConnectionFactory connectionFactory) 
			: base(nameof(AddGameTagsTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				CREATE TABLE tags (
					id SERIAL PRIMARY KEY,
					name TEXT NOT NULL UNIQUE
				);

				CREATE TABLE game_tags (
					game_id INTEGER NOT NULL REFERENCES games(id),
					tag_id INTEGER NOT NULL REFERENCES tags(id),
				);

				GRANT
					SELECT, INSERT, UPDATE, DELETE
				ON 
					tags, game_tags
				TO v8_r8_hub_api_group;
				
				GRANT USAGE ON SEQUENCE
					tags_id_seq, game_tags_id_seq
				TO v8_r8_hub_api_group;
				""");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				REVOKE USAGE ON SEQUENCE 
					tags_id_seq, game_tags_id_seq
				FROM v8_r8_hub_api_group;
			
				REVOKE 
					SELECT, INSERT, UPDATE, DELETE
				ON
					tags, game_tags
				FROM v8_r8_hub_api_group;

				DROP TABLE tags;
				DROP TABLE game_tags;
				""");
		}
	}
}
