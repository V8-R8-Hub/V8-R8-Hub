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
	internal class AddWebGroup : Upgrader {
		public AddWebGroup(IConnectionFactory connectionFactory)
			: base(nameof(AddWebGroup), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				CREATE ROLE v8_r8_hub_api_group WITH
					NOLOGIN;
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				DROP ROLE v8_r8_hub_api_group;
			");
		}

	}
}
