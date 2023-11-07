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
	internal class AddMetricTable : Upgrader {
		public AddMetricTable(IConnectionFactory connectionFactory) 
			: base(nameof(AddMetricTable), connectionFactory) {
		}
		
		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				CREATE TABLE user_metrics (
					id SERIAL PRIMARY KEY,
					metric JSON NOT NULL,
					created_at TIMESTAMP NOT NULL DEFAULT NOW(),
					user_id INTEGER NOT NULL REFERENCES users(id),
					game_id INTEGER NOT NULL REFERENCES games(id)
				);
				GRANT
					SELECT, INSERT, UPDATE, DELETE
				ON 
					user_metrics
				TO v8_r8_hub_api_group;

				GRANT USAGE ON SEQUENCE
					user_metrics_id_seq
				TO v8_r8_hub_api_group;
				""");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery("""
				REVOKE USAGE ON SEQUENCE 
					user_metrics_id_seq
				FROM v8_r8_hub_api_group;
			
				REVOKE 
					SELECT, INSERT, UPDATE, DELETE
				ON
					user_metrics
				FROM v8_r8_hub_api_group;

				DROP TABLE user_metrics;
				""");
		}
	}
}
