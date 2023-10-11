using DbUpgrader.Upgraders;
using DBUpgrader.Interfaces;

namespace DBUpgrader.Repositories {
	internal class UpgraderRepository : IUpgraderRepository
    {
        private List<Upgrader> _upgraders;
        public UpgraderRepository(IConnectionFactory connectionFactory)
        {
            _upgraders = new List<Upgrader>() {
                new InitUpgrader(connectionFactory),
                new AddSessionTable(connectionFactory)
            };
        }

        public IEnumerable<Upgrader> GetUpgraders() {
            return _upgraders;
        }
    }
}
