namespace V8_R8_Hub.Models.Exceptions {
	public class DisallowedMimeTypeException : Exception {
		public string GivenMimeType { get; set; }
		public DisallowedMimeTypeException(string givenMimeType, string? message) : base(message) {
			GivenMimeType = givenMimeType;
		}
	}
}
