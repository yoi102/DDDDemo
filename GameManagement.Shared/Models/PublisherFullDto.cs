namespace GameManagement.Shared.Models
{
    public class PublisherFullDto
    {

        public string Name { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public DateTimeOffset EstablishmentTime { get; set; }
        public DateTimeOffset? BankruptTime { get; set; }

    }
}
