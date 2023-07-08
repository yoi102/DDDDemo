using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.DtoParameters
{
    public class TagDtoParameters
    {
        private const int MaxPageSize = 20;
        [Required]
        public string Tag { get; set; } = string.Empty;

        public int PageNumber { get; set; } = 1;
        private int _pageSize = 5;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }







    }
}
