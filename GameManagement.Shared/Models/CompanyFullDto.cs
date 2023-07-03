namespace GameManagement.Shared.Models
{
    public class CompanyFullDto
    {

        public string? Name { get; set; }
        public string? Country { get; set; }
        public string? Introduction { get; set; }
        public DateTimeOffset EstablishmentTime { get; set; }
        public DateTimeOffset? BankruptTime { get; set; }

    }
}
