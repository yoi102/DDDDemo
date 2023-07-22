namespace GameManagement.Shared.DtoParameters
{
    public class GameDtoParameters
    {
        private const int MaxPageSize = 15;
        public string? Q { get; set; }
        public int PageNumber { get; set; } = 1;
        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }

        public string OrderBy { get; set; } = "Name";
    }
}