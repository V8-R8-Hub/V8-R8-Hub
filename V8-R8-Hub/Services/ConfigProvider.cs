namespace V8_R8_Hub.Services {
	public interface IConfigProvider {
		string DatabaseName { get; }
		string DatabaseUser { get; }
		string DatabasePassword { get; }
		string DatabaseHost { get; }
	}

	public class DevConfigProvider : IConfigProvider {
		public string DatabaseName => "v8r8hub";
		public string DatabaseUser => "v8_r8_api_user";
		public string DatabasePassword => "bobby";
		public string DatabaseHost => "localhost";
	}

	public class ProductionConfigProvider : IConfigProvider {
		public string DatabaseName => Environment.GetEnvironmentVariable("DATABASE_NAME")
			?? "v8r8hub";
		public string DatabaseUser => Environment.GetEnvironmentVariable("DATABASE_USER")
			?? "v8_r8_api_user";
		public string DatabasePassword => Environment.GetEnvironmentVariable("DATABASE_PASSWORD")
			?? throw new ArgumentException("DATABASE_PASSWORD environment variable not set");
		public string DatabaseHost => Environment.GetEnvironmentVariable("DATABASE_HOST")
			?? "localhost";
	}
}
