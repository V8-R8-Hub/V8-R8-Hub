namespace V8_R8_Hub.Models.Exceptions {
	public class UnknownGameException : Exception {
		public Guid GivenGuid { get; set; } 
		public UnknownGameException(Guid givenGuid, string? message) : base(message) {
			GivenGuid = givenGuid;
		}
	}
}
