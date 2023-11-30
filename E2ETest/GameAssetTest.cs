using Microsoft.AspNetCore.Mvc.Testing;
using Swashbuckle.SwaggerUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using V8_R8_Hub.Models.Response;

namespace E2ETest {
	public class GameAssetTest {
		private HttpClient _client { get; set; }
		private WebApplicationFactory<Program> _application;

		public GameAssetTest() {
			_application = new WebApplicationFactory<Program>();
			_client = _application.CreateDefaultClient();
		}

		[Fact]
		public async Task GameAsset_Get_AssetNotExisting() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var games = await _client.GetAsync($"/api/Game/{gameGuid}/assets/Not existing.png");
			Assert.Equivalent(HttpStatusCode.NotFound, games.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Get_GameNotExisting() {
			var games = await _client.GetAsync($"/api/Game/{Guid.NewGuid()}/assets/Not existing.png");
			Assert.Equivalent(HttpStatusCode.NotFound, games.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Create() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var form = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "text/plain"), "assetFiles", "testAsset.txt" }
			};

			var createRequest = await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			Assert.Equivalent(HttpStatusCode.OK, createRequest.StatusCode);

			var getRequest = await _client.GetAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");

			Assert.Equivalent(HttpStatusCode.OK, getRequest.StatusCode);
			Assert.Equal("SomeAsset", await getRequest.Content.ReadAsStringAsync());
			Assert.Equal("text/plain", getRequest.Content.Headers.ContentType?.MediaType);
		}
	}
}
