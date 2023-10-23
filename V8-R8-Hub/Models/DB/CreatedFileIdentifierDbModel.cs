namespace V8_R8_Hub.Models.DB
{
    public record CreatedFileIdentifierDbModel
    {
        public required int Id { get; set; }
        public required Guid PublicId { get; set; }
    }
}
