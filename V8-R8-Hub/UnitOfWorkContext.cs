using System.Data;
using V8_R8_Hub.Services;

namespace V8_R8_Hub {
    public interface IUnitOfWorkContext : IDisposable {
        Task Begin();
        Task Commit();
        Task Rollback();
    }

    public class UnitOfWorkContext : IUnitOfWorkContext {
        private IDbTransaction? _transaction;
        private readonly IDbConnection _db;
        public UnitOfWorkContext(IDbConnector connector) {
            _db = connector.GetDbConnection();
        }

        public async Task Begin() {
            _db.Open();
            _transaction = _db.BeginTransaction();
        }

        public async Task Commit() {
            _transaction?.Commit();
            _transaction = null;
        }

        public void Dispose() {
            _transaction?.Dispose();
        }

        public async Task Rollback() {
            _transaction?.Rollback();
            _transaction = null;
        }
    }
}
