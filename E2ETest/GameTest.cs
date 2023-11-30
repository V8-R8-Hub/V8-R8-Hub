using Microsoft.AspNetCore.Mvc.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using V8_R8_Hub.Models.Response;

namespace E2ETest {
	public class GameTest : IClassFixture<ClientFixture> {
		private ClientFixture _fixture;
		private HttpClient _client { get; set; }

		public GameTest(ClientFixture clientFixture) {
			_fixture = clientFixture;
			_client = _fixture._client;
		}

		[Fact]
		public async Task Game_Create_Smoke() {
			await ProjectHelper.CreateSampleGame(_client);
		}

		[Fact]
		public async Task Game_GetExistingGame() {
			var guid = await ProjectHelper.CreateSampleGame(_client);

			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>("/api/Game");

			Assert.NotNull(games);

			var game = games.Where(x => x.Guid == guid).Single();
			Assert.NotNull(game);

			Assert.Equal(ProjectHelper.SampleGameName, game.Name);
			Assert.Equal(ProjectHelper.SampleGameDescription, game.Description);
		}

		[Fact]
		public async Task Game_Get_Files() {
			var guid = await ProjectHelper.CreateSampleGame(_client);

			var games = await _client.GetFromJsonAsync<List<GameBriefResponse>>("/api/Game");

			Assert.NotNull(games);

			var game = games.Where(x => x.Guid == guid).Single();
			Assert.NotNull(game);

			var gameFile = await _client.GetAsync(game.GameBlobUrl);
			Assert.Equivalent(HttpStatusCode.OK, gameFile.StatusCode);
			Assert.Equal("text/html", gameFile.Content.Headers.ContentType?.MediaType);
			Assert.Equal(ProjectHelper.SampleGameHtmlContent, await gameFile.Content.ReadAsStringAsync());

			var thumbnailFile = await _client.GetAsync(game.ThumbnailUrl);
			Assert.Equivalent(HttpStatusCode.OK, thumbnailFile.StatusCode);
			Assert.Equal("image/jpeg", thumbnailFile.Content.Headers.ContentType?.MediaType);
			Assert.Equal(ProjectHelper.SampleGameThumbnailFile, await thumbnailFile.Content.ReadAsStringAsync());
		}
	}
}
