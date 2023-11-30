using Dapper;
using Npgsql;
using System.Data;
using V8_R8_Hub.Models.Exceptions;
using V8_R8_Hub.Repositories;

namespace V8_R8_Hub.Services {
	public interface IMetricService {
		public Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid);
	}

	public class MetricService : IMetricService {
		private readonly IMetricRepository _metricRepository;

		public MetricService(IMetricRepository metricRepository) {
			_metricRepository = metricRepository;
		}

		public async Task AddMetric(string metricJsonData, string category, int userId, Guid gameGuid) {
			await _metricRepository.AddMetric(metricJsonData, category, userId, gameGuid);
		}
	}
}
