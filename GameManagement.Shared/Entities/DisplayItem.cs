using System.ComponentModel.DataAnnotations;

namespace GameManagement.Shared.Entities
{
    public class DisplayItem : EntityBase
    {

        public string FileSHA256Hash { get; set; } = string.Empty;

        public string FileName { get;  set; } = string.Empty;


        public long FileSizeInBytes { get;  set; }

        [Url]
        [Required]
        public string RemoteUrl { get; set; } = string.Empty;


        //[Url]
        //[Required]
        //public string BackupUrl { get; set; } = string.Empty;


        public Guid GameId { get; set; }
        public Game Game { get; set; } = new Game();

    }
}