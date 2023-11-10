namespace V8_R8_Hub.Models.Request {
	public record AddGameSessionRequest {
		public required DateTimeOffset Begin { get; set; }
		public required DateTimeOffset End { get; set; }
	}
}
