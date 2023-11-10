using DbUpgrader.Upgraders;
using DBUpgrader.Interfaces;

namespace DBUpgrader.Repositories {
	internal class UpgraderRepository : IUpgraderRepository {
		private List<Upgrader> _upgraders;
		public UpgraderRepository(IConnectionFactory connectionFactory) {
			_upgraders = new List<Upgrader>() {
				new InitUpgrader(connectionFactory),
				new AddWebGroup(connectionFactory),
				new AddSessionTable(connectionFactory),
				new AddPublicFileTable(connectionFactory),
				new AddGameTable(connectionFactory),
				new AddWebGroupPermissions(connectionFactory),
				new AddGameAssetsTable(connectionFactory),
				new AddUserTable(connectionFactory),
				new AddAuthKeyTable(connectionFactory),
				new AddMetricTable(connectionFactory),
				new AddUserGameSessionTable(connectionFactory),
			};
		}

		public IEnumerable<Upgrader> GetUpgraders() {
			return _upgraders;
		}
	}
}
