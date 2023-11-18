using NSubstitute;
using NSubstitute.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using V8_R8_Hub.Repositories;
using V8_R8_Hub.Services;

namespace UnitTest {
	public class GameRepositoryTest {
		private IDbConnection _dbConnection;
		private IConfigProvider _configProvider;
		private GameRepository _gameRepository;

		public GameRepositoryTest() {
			_configProvider = Substitute.For<IConfigProvider>();
			_configProvider.Configure().DatabaseName.Returns("v8_r8_test");
			_gameRepository = new GameRepository()
		}

		[Fact]
		public async Task 
	}
}
