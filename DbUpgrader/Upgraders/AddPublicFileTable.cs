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
	internal class AddPublicFileTable : Upgrader {
		public AddPublicFileTable(IConnectionFactory connectionFactory)
			: base(nameof(AddPublicFileTable), connectionFactory) { }

		protected override async Task UpInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				CREATE TABLE public_files (
					id SERIAL PRIMARY KEY,
					public_id UUID NOT NULL UNIQUE DEFAULT gen_random_uuid(),
					file_name TEXT NOT NULL,
					mime_type TEXT NOT NULL,
					content_blob bytea NOT NULL
				);
			");
		}

		protected override async Task DownInternal(NpgsqlConnection connection) {
			await connection.ExecuteQuery(@"
				DROP TABLE public_files;
			");
		}
	}
}
