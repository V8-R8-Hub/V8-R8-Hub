using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E2ETest {
	public class ClientFixture : IDisposable {
		public HttpClient Client { get; private set; }
		private readonly WebApplicationFactory<Program> _application;

		public ClientFixture() {
			_application = new WebApplicationFactory<Program>();
			Client = _application.CreateDefaultClient();
		}

		public void Dispose() {
			Client.Dispose();
			_application.Dispose();
			GC.SuppressFinalize(this);
		}
	}
}
