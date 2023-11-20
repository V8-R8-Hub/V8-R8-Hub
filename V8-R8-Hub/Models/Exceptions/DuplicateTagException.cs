namespace V8_R8_Hub.Models.Exceptions {
	public class DuplicateTagException : Exception {
		public string? GivenTag { get; set; }
		public DuplicateTagException(string? givenTag, string? message) : base(message) {
			GivenTag = givenTag;
		}
	}
}
