using DBUpgrader.Exceptions;
using DBUpgrader.Extensions;
using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBUpgrader.Services {
    internal class UpgradePlannerService : IUpgradePlannerService {
        private readonly IUpgraderRepository _upgraderRepository;
        private readonly IUpgradeTrackerRepository _upgradeTrackerRepository;

        public UpgradePlannerService(IUpgraderRepository upgraderRepository, IUpgradeTrackerRepository upgradeTrackerRepository) {
            _upgraderRepository = upgraderRepository;
            _upgradeTrackerRepository = upgradeTrackerRepository;
        }

        public async Task<string?> GetCurrentUpgradeLevel() {
            var upgraderList = _upgraderRepository.GetUpgraders().ToList();
            var enumerator = upgraderList.GetEnumerator();
            var entries = _upgradeTrackerRepository.GetTrackerEntries();
            await foreach (var entry in entries) {
                enumerator.MoveNext();
                if (enumerator.Current == null) {
                    throw new UnknownUpgradeLevelException(entry.UpgraderName, $"Database has an unknown upgrader {entry.UpgraderName}");
                }

                if (enumerator.Current.Name != entry.UpgraderName) {
                    throw new UnknownUpgradeLevelException(entry.UpgraderName, $"Database has an unknown upgrader {entry.UpgraderName}");
                }
            }
            if (enumerator.Current == null) {
                return null;
            }
            return enumerator.Current.Name;
        }

        public async Task<IEnumerable<UpgraderAction>> PlanUpgradePath(string targetUpgrader) {
            var upgraderList = _upgraderRepository.GetUpgraders().ToList();
            var targetUpgraderIndex = upgraderList.FindIndex(x => x.Name == targetUpgrader);
            if (targetUpgraderIndex == -1) {
                throw new UnknownTargetException(targetUpgrader, "Unknown target upgrader");
            }

            string? currentUpgradeLevel = await GetCurrentUpgradeLevel();
            if (currentUpgradeLevel == null) {
                return upgraderList.Take(targetUpgraderIndex + 1).Select(upgrader => new UpgraderAction {
                    Upgrader = upgrader,
                    UpgraderActionType = UpgraderActionType.Up
                });
            }
            var currentUpgradeIndex = upgraderList.FindIndex(x => x.Name == currentUpgradeLevel);

            if (currentUpgradeIndex == -1) {
                throw new Exception("Expected current upgrade to be found");
            }

            List<UpgraderAction> actions = new List<UpgraderAction>();
            if (currentUpgradeIndex == targetUpgraderIndex) {
                return Enumerable.Empty<UpgraderAction>();
            } else if (targetUpgraderIndex - currentUpgradeIndex < 0) {
                for (int i = currentUpgradeIndex; i > targetUpgraderIndex; i--) {
                    actions.Add(new UpgraderAction {
                        Upgrader = upgraderList[i],
                        UpgraderActionType = UpgraderActionType.Down
                    });
                }
            } else {
                for (int i = currentUpgradeIndex + 1; i <= targetUpgraderIndex; i++) {
                    actions.Add(new UpgraderAction {
                        Upgrader = upgraderList[i],
                        UpgraderActionType = UpgraderActionType.Up
                    });
                }
            }
            return actions;
        }

        public async Task<IEnumerable<UpgraderAction>> PlanUpgradePathToLatest() {
            return await PlanUpgradePath(_upgraderRepository.GetUpgraders().Last().Name);
        }
    }
}
