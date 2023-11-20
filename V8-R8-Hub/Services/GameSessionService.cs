using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Repositories;

namespace V8_R8_Hub.Services {
	public interface IGameSessionService {
		Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop);
	}
	public class GameSessionService : IGameSessionService {
		private readonly IGameSessionRepository _gameSessionRepository;
		private readonly IUnitOfWorkContext _uow;

		public GameSessionService(IGameSessionRepository gameSessionRepository, IUnitOfWorkContext uow) {
			_gameSessionRepository = gameSessionRepository;
			_uow = uow;
		}

		public async Task AddGameSession(int userId, Guid gameGuid, DateTimeOffset start, DateTimeOffset stop) {
			await _uow.Begin();
			if (start > stop) {
				throw new ArgumentException("Start date must be before stop date", nameof(start));
			}
			await _gameSessionRepository.AddGameSession(userId, gameGuid, start, stop);
			await _uow.Commit();
		}
	}
}
