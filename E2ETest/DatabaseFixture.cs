using Npgsql;
using Shared.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Shared.Models;
using System.Threading.Tasks;
using V8_R8_Hub.Services;

namespace E2ETest {
	public class TestConfigProvider : IConfigProvider {
		public string DatabaseName => "v8r8hub_e2e";
		public string DatabaseUser => "v8_r8_api_user";
		public string DatabasePassword => "bobby";
		public string DatabaseHost => "localhost";
	}

	public class DatabaseFixture : IDisposable {
		public DbConnector _dbConnector;

		public DatabaseFixture() {
			_dbConnector = new DbConnector(
				new TestConfigProvider()
			);
		}

		public void Dispose() {
			
		}
	}
}
