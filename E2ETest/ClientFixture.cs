using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2ETest {
	public class ClientFixture : IDisposable {
		public HttpClient _client { get; set; }
		private WebApplicationFactory<Program> _application;

		public ClientFixture() {
			_application = new WebApplicationFactory<Program>();
			_client = _application.CreateDefaultClient();
		}

		public void Dispose() {
			_client.Dispose();
			_application.Dispose();
		}
	}
}
