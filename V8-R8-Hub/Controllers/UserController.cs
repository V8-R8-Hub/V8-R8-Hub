using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using V8_R8_Hub.Middleware;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Models.Request;
using V8_R8_Hub.Services;

namespace V8_R8_Hub.Controllers {
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase {
		private readonly IMetricService _metricService;
		private readonly ILogger<UserController> _logger;
		private readonly IGameSessionService _gameSessionService;

		public UserController(IMetricService metricService, ILogger<UserController> logger, IGameSessionService gameSessionService) {
			_metricService = metricService;
			_logger = logger;
			_gameSessionService = gameSessionService;
		}

		/// <summary>
		/// Add game metric for a game with a specific category
		/// </summary>
		/// <response code="200">Success the metric was added</response>
		/// <response code="404">The game guid does not correspond with a game</response>
		/// <response code="400">The metric json data was not valid</response>
		[HttpPost("metrics/{gameGuid:guid}", Name = "AddMetric")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(string), 404)]
		[ProducesResponseType(typeof(string), 400)]
		public async Task<IActionResult> AddMetric(Guid gameGuid, [FromBody] AddMetricRequest request) {
			try {
				await _metricService.AddMetric(request.MetricJsonData, request.MetricCategory, HttpContext.GetUserId(), gameGuid);
			} catch (UnknownGameException ex) {
				_logger.LogInformation("Tried to add metric to non existant game {GivenGuid}", ex.GivenGuid);
				return NotFound("Game not found");
			} catch (InvalidJsonException) {
				_logger.LogInformation("Client sent invalid json metric");
				return BadRequest("Json metric was not valid");
			}
			return Ok();
		}

		/// <summary>
		/// Adds a play session for a specific game
		/// </summary>
		/// <response code="200">Game session was added</response>
		/// <response code="404">Game does not exist</response>
		/// <response code="400">Some parameter is invalid</response>
		/// <returns></returns>
		[HttpPost("game-sessions/{gameGuid:guid}", Name = "AddGameSession")]
		[ProducesResponseType(200)]
		[ProducesResponseType(typeof(string), 404)]
		[ProducesResponseType(typeof(string), 400)]
		public async Task<IActionResult> AddGameSession(Guid gameGuid, [FromBody] AddGameSessionRequest request) {
			try {
				await _gameSessionService.AddGameSession(HttpContext.GetUserId(), gameGuid, request.Begin, request.End);
			} catch (UnknownGameException ex) {
				_logger.LogInformation("Tried to add session for non existant game {GivenGuid}", ex.GivenGuid);
				return NotFound("Game not found");
			} catch (ArgumentException ex) {
				if (ex.ParamName == "start") {
					_logger.LogInformation("Client sent invalid game session {Message}", ex.Message);
					return BadRequest("Start date has to be before stop date");
				}
				throw ex;
			}
			return Ok();
		}
	}
}
