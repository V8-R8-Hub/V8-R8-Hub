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
	public class GameAssetTest : IClassFixture<ClientFixture> {
		private readonly ClientFixture _fixture;
		private readonly HttpClient _client;

		public GameAssetTest(ClientFixture clientFixture) {
			_fixture = clientFixture;
			_client = _fixture.Client;
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

			await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			var getRequest = await _client.GetAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");

			Assert.Equivalent(HttpStatusCode.OK, getRequest.StatusCode);
			Assert.Equal("SomeAsset", await getRequest.Content.ReadAsStringAsync());
			Assert.Equal("text/plain", getRequest.Content.Headers.ContentType?.MediaType);
		}

		[Fact]
		public async Task GameAsset_Create_WhenGameNotExisting() {
			var gameGuid = Guid.NewGuid();

			var form = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "text/plain"), "assetFiles", "testAsset.txt" }
			};

			await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			var getRequest = await _client.GetAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");

			Assert.Equivalent(HttpStatusCode.NotFound, getRequest.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Delete() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var form = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "text/plain"), "assetFiles", "testAsset.txt" }
			};

			await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			var delete = await _client.DeleteAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");

			var getRequest = await _client.GetAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");
			
			Assert.Equivalent(HttpStatusCode.OK, delete.StatusCode);
			Assert.Equivalent(HttpStatusCode.NotFound, getRequest.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Delete_WithNonExistantAsset() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var delete = await _client.DeleteAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");
			Assert.Equivalent(HttpStatusCode.NotFound, delete.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Delete_WithNonExistantGame() {
			var delete = await _client.DeleteAsync($"/api/Game/{Guid.NewGuid()}/assets/testAsset.txt");
			Assert.Equivalent(HttpStatusCode.NotFound, delete.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Create_WithDuplicateName() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var form = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "text/plain"), "assetFiles", "testAsset.txt" }
			};	

			await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			var secondForm = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "text/plain"), "assetFiles", "testAsset.txt" }
			};

			var secondPost = await _client.PostAsync($"/api/Game/{gameGuid}/assets", secondForm);
			Assert.Equivalent(HttpStatusCode.BadRequest, secondPost.StatusCode);
		}

		[Fact]
		public async Task GameAsset_Create_WithUnsupportedMimeType() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);
			var form = new MultipartFormDataContent {
				{ new StringContent("SomeAsset", null, "application/x-tar"), "assetFiles", "testAsset.txt" }
			};

			var createResponse = await _client.PostAsync($"/api/Game/{gameGuid}/assets", form);

			var getRequest = await _client.GetAsync($"/api/Game/{gameGuid}/assets/testAsset.txt");

			Assert.Equivalent(HttpStatusCode.BadRequest, createResponse.StatusCode);
			Assert.Equivalent(HttpStatusCode.NotFound, getRequest.StatusCode);
		}
	}
}
