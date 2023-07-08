namespace GameManagement.Shared.Models
{
    public class CompanyAddWithBankruptTimeDto : CompanyAddDto
    {
        public DateTime BankruptTime { get; set; }
    }
}
