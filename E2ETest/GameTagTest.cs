using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using V8_R8_Hub.Models.Response;

namespace E2ETest {
	public class GameTagTest : IClassFixture<ClientFixture> {
		private readonly ClientFixture _fixture;
		private readonly HttpClient _client;

		public GameTagTest(ClientFixture clientFixture) {
			_fixture = clientFixture;
			_client = _fixture.Client;
		}

		[Fact]
		public async Task GameTag_Create() {
			await ProjectHelper.CreateSampleGame(_client);
			
			await _client.PostJsonContent("/api/GameTag", "nothing");
		}

		[Fact]
		public async Task GameTag_Get_Empty() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>($"/api/Game");

			Assert.NotNull(games);
			
			var game = games.Where(x => x.Guid == gameGuid).Single();
			Assert.NotNull(game);
			Assert.Empty(game.Tags);
		}

		[Fact]
		public async Task GameTag_Get_SingleTag() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			var postResponse = await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "test");
			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>($"/api/Game");

			Assert.NotNull(games);

			Assert.Multiple(
				() => Assert.Equivalent(HttpStatusCode.OK, postResponse.StatusCode),
				() => Assert.Contains(games, x => x.Guid == gameGuid && x.Tags.First() == "test")
			);
		}

		[Fact]
		public async Task GameTag_Get_MultipleTags() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "test");
			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "hallo");
			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "yep");
			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>($"/api/Game");

			Assert.NotNull(games);

			var game = games.Single(x => x.Guid == gameGuid);

			Assert.Multiple(
				() => Assert.Collection(game.Tags,
					tag => Assert.Equal("test", tag),
					tag => Assert.Equal("hallo", tag),
					tag => Assert.Equal("yep", tag)
				)
			);
		}

		[Fact]
		public async Task GameTag_Get_RemoveTag() {
			var gameGuid = await ProjectHelper.CreateSampleGame(_client);

			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "test");
			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "hallo");
			await _client.PostJsonContent($"/api/Game/{gameGuid}/tags", "yep");
			await _client.DeleteAsync($"/api/Game/{gameGuid}/tags/hallo");
	
			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>($"/api/Game");
			
			Assert.NotNull(games);

			var game = games.Where(x => x.Guid == gameGuid).Single();

			Assert.Multiple(
				() => Assert.Collection(game.Tags,
					tag => Assert.Equal("test", tag),
					tag => Assert.Equal("yep", tag)
				)
			);
		}
	}
}
