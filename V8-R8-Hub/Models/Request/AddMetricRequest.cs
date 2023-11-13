namespace V8_R8_Hub.Models.Request {
	public record AddMetricRequest {
		public required string MetricJsonData { get; set; }
		public required string MetricCategory { get; set; }
	}
}
