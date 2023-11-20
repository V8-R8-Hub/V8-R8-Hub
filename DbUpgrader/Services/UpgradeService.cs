using DBUpgrader.Exceptions;
using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Services
{
	internal class UpgradeService : IUpgradeService
	{
		private readonly IConnectionFactory _connectionFactory;
		private readonly IUpgradePlannerService _upgradePlannerService;

		public UpgradeService(
			IConnectionFactory connectionFactory, 
			IUpgradePlannerService upgradePlannerService
		) {
			_connectionFactory = connectionFactory;
			_upgradePlannerService = upgradePlannerService;
		}

		public async Task UpgradeTo(string targetUpgrader, UpgradeConfig config) {
			var upgradePath = await _upgradePlannerService.PlanUpgradePath(targetUpgrader);
			await RunUpgradePath(upgradePath, config);
		}

		public async Task UpgradeToLatest(UpgradeConfig config) {
			var upgradePath = await _upgradePlannerService.PlanUpgradePathToLatest();
			await RunUpgradePath(upgradePath, config);
		}

		private async Task RunUpgradePath(IEnumerable<UpgraderAction> upgradePath, UpgradeConfig config) {
			if (!config.AllowDown) {
				var downgradeAction = upgradePath.FirstOrDefault(x => x.UpgraderActionType == UpgraderActionType.Down);
				if (downgradeAction != null) {
					throw new DisallowedUpgradeActionException(downgradeAction, "Upgrade path contains downgrades, but downgrades are not allowed");
				}
			}

			var connection = await _connectionFactory.GetConnection();
			using var transaction = await connection.BeginTransactionAsync();
			try {
				foreach (var upgraderAction in upgradePath) {
					if (upgraderAction.UpgraderActionType == UpgraderActionType.Up) {
						Console.WriteLine($"{upgraderAction.Upgrader.Name} UP");
						await upgraderAction.Upgrader.Up(connection);
					} else if (upgraderAction.UpgraderActionType == UpgraderActionType.Down) {
						Console.WriteLine($"{upgraderAction.Upgrader.Name} DOWN");
						await upgraderAction.Upgrader.Down(connection);
					}
				}
			} catch (Exception) {
				await transaction.RollbackAsync();
				throw;
			}

			await transaction.CommitAsync();
		}
	}
}
