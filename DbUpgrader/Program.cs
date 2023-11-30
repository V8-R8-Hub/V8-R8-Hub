using DBUpgrader;
using DBUpgrader.Exceptions;
using DBUpgrader.Extensions;
using DBUpgrader.Interfaces;
using DBUpgrader.Models;
using DBUpgrader.Repositories;
using DBUpgrader.Services;
using Npgsql;
using Shared.Models;
using System.CommandLine;
using System.Security.Cryptography;

var configRepository = new ConfigRepository();
var connectionFactory = new UpgraderConnectionFactory(configRepository);
var upgraderRepository = new UpgraderRepository(connectionFactory);

static async Task WithDefaultExceptionHandling(Func<Task> wrappedFunction) {
	try {
		await wrappedFunction();
	} catch (DisallowedUpgradeActionException ex) {
		Console.WriteLine($"Upgrade path requires using {ex.OffendingAction.UpgraderActionType} on {ex.OffendingAction.Upgrader.Name} but this is not allowed, use the --allow-down option to allow this");
	} catch (UnknownTargetException ex) {
		Console.WriteLine($"Unknown target upgrader {ex.RequestedUpgrader} requested");
	} catch (UnknownUpgradeLevelException ex) {
		Console.WriteLine($"Database has an unknown upgrader {ex.UnknownUpgraderLevel}");
	}
}

var upgraderTargetArgument = new Argument<string>(name: "upgrader", description: "The upgrader which should be reached")
	.FromAmong(upgraderRepository.GetUpgraders().Select(x => x.Name).ToArray());

var allowDownOption = new Option<bool>(name: "--allow-down", description: "Allows downgraders to be run to reach the specified upgrader");

var rootCommand = new RootCommand();
var upgradeCommand = new Command(name: "upgrade", description: "Upgrades to the specified upgrader");

upgradeCommand.AddArgument(upgraderTargetArgument);
upgradeCommand.AddOption(allowDownOption);

upgradeCommand.SetHandler(async (upgraderTarget, allowDown) => {
	var upgradeTrackerRepository = new UpgradeTrackerRepository(connectionFactory);
	var upgradePlannerService = new UpgradePlannerService(upgraderRepository, upgradeTrackerRepository);
	var upgradeService = new UpgradeService(connectionFactory, upgradePlannerService);
	await WithDefaultExceptionHandling(async () => {
		await upgradeService.UpgradeTo(upgraderTarget, new UpgradeConfig() {
			AllowDown = allowDown
		});
	});
}, upgraderTargetArgument, allowDownOption);

var upgradeLatestCommand = new Command(name: "upgrade-latest", description: "Upgrades to latest upgrader");

upgradeLatestCommand.AddOption(allowDownOption);

upgradeLatestCommand.SetHandler(async (allowDown) => {
	var upgradeTrackerRepository = new UpgradeTrackerRepository(connectionFactory);
	var upgradePlannerService = new UpgradePlannerService(upgraderRepository, upgradeTrackerRepository);
	var upgradeService = new UpgradeService(connectionFactory, upgradePlannerService);
	await WithDefaultExceptionHandling(async () => {
		await upgradeService.UpgradeToLatest(new UpgradeConfig() {
			AllowDown = allowDown
		});
	});
}, allowDownOption);

var listPathCommand = new Command(name: "list-path", description: "Lists the generated path for reaching to the specified upgrader");
listPathCommand.AddArgument(upgraderTargetArgument);

listPathCommand.SetHandler(async (upgraderTarget) => {
	var upgradeTrackerRepository = new UpgradeTrackerRepository(connectionFactory);
	var upgradePlannerService = new UpgradePlannerService(upgraderRepository, upgradeTrackerRepository);
	await WithDefaultExceptionHandling(async () => {
		var upgradePath = await upgradePlannerService.PlanUpgradePath(upgraderTarget);
		foreach (var upgraderAction in upgradePath) {
			Console.WriteLine($"{upgraderAction.Upgrader.Name} {upgraderAction.UpgraderActionType}");
		}
	});
}, upgraderTargetArgument);

var createDevUserCommand = new Command(name: "create-dev-user", description: "Creates the user for dev environment");

createDevUserCommand.SetHandler(async () => {
	var db = await connectionFactory.GetConnection();
	await db.ExecuteQuery("""
		CREATE ROLE v8_r8_api_user WITH
			LOGIN
			PASSWORD 'bobby'
			IN ROLE v8_r8_hub_api_group;
		""");
});

var hostArgument = new Argument<string>(name: "host", description: "Database host");
var databaseNameArgument = new Argument<string>(name: "name", description: "Database name");
var databaseUserOption = new Option<string?>(name: "--user", description: "Database user username", getDefaultValue: () => null);
databaseUserOption.AddAlias("-u");
var portOption = new Option<ushort>(name: "--port", description: "Database port", getDefaultValue: () => 5432);
databaseUserOption.AddAlias("-p");

var bootstrapCommand = new Command(name: "bootstrap", description: "Initializes the database with upgrader user and upgrade log table");
bootstrapCommand.AddArgument(hostArgument);
bootstrapCommand.AddArgument(databaseNameArgument);
bootstrapCommand.AddOption(databaseUserOption);
bootstrapCommand.AddOption(portOption);

bootstrapCommand.SetHandler(
	async (host, databaseName, username, port) => {
		var configRepository = new ConfigRepository();

		if (username != null) {
			Console.Write($"Enter password for {username}: ");
			string password = Console.ReadLine() ?? "";
			var adminConnectionFactory = new AdminConnectionFactory(new ConnectionStringModel() {
				Host = host,
				Username = username,
				Password = password,
				Port = port
			});
			var adminConnection = await adminConnectionFactory.GetConnection();
			var commandBuilder = new NpgsqlCommandBuilder();

			var randomBytes = RandomNumberGenerator.GetBytes(8);
			var generatedPassword = Convert.ToHexString(randomBytes);

			var command = new NpgsqlCommand("SELECT 1 FROM pg_roles WHERE rolname='hub_upgrader'", adminConnection);
			var result = await command.ExecuteScalarAsync();
			if (!(result ?? 0).Equals(1)) {
				await adminConnection.ExecuteQuery($"""
					CREATE ROLE hub_upgrader WITH
						PASSWORD '{generatedPassword}'
						LOGIN
						CREATEDB
						CREATEROLE
				""");
				await configRepository.SetConfig(new PersistentDataBaseConfig() {
					DatabaseName = databaseName,
					Password = generatedPassword,
					Host = host,
					Port = port,
					Username = "hub_upgrader"
				});
			}

			command = new NpgsqlCommand("SELECT 1 FROM pg_catalog.pg_database WHERE datname = @databaseName;", adminConnection);
			command.Parameters.AddWithValue("databaseName", databaseName);
			result = await command.ExecuteScalarAsync();
			if (!(result ?? 0).Equals(1)) {
				await adminConnection.ExecuteQuery($@"
					CREATE DATABASE {commandBuilder.QuoteIdentifier(databaseName)} 
						OWNER hub_upgrader
				");
			}
			await adminConnection.CloseAsync();
		}

		var upgraderConnectionFactory = new UpgraderConnectionFactory(configRepository);

		var upgraderConnection = await upgraderConnectionFactory.GetConnection();

		await using (var transaction = await upgraderConnection.BeginTransactionAsync()) {
			await upgraderConnection.ExecuteQuery("""
				CREATE TYPE upgrade_action_type AS ENUM ('Up', 'Down');
			""");

			await upgraderConnection.ExecuteQuery("""
				CREATE TABLE upgrade_log (
					id SERIAL PRIMARY KEY,
					upgrader_name text NOT NULL,
					action_type upgrade_action_type NOT NULL,
					success boolean NOT NULL,
					message text
				);
			""");

			await upgraderConnection.ExecuteQuery("""
				CREATE TABLE upgrade_tracker (
					id SERIAL PRIMARY KEY,
					upgrader_name text UNIQUE NOT NULL
				);
			""");

			await transaction.CommitAsync();
		}

	},
	hostArgument,
	databaseNameArgument,
	databaseUserOption,
	portOption
);


rootCommand.Add(upgradeCommand);
rootCommand.Add(upgradeLatestCommand);
rootCommand.Add(listPathCommand);
rootCommand.Add(bootstrapCommand);
rootCommand.Add(createDevUserCommand);

await rootCommand.InvokeAsync(args);
