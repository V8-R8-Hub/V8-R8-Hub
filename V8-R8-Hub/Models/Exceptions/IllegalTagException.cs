namespace V8_R8_Hub.Models.Exceptions {
	public class IllegalTagException : Exception {
		public string GivenTag { get; set; }
		public IllegalTagException(string givenTag, string? message) : base(message) {
			GivenTag = givenTag;
		}
	}
}
