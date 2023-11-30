using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using V8_R8_Hub.Models.Response;

namespace E2ETest {
	public class FrontendSmokeTest : IClassFixture<ClientFixture> {
		private readonly ClientFixture _fixture;
		private readonly HttpClient _client;

		public FrontendSmokeTest(ClientFixture clientFixture) {
			_fixture = clientFixture;
			_client = _fixture.Client;
		}

		[Fact]
		public async Task Index_Get_SmokeTest() {
			var response = await _client.GetAsync("/");

			Assert.True(response.StatusCode == HttpStatusCode.OK);
		}
	}
}