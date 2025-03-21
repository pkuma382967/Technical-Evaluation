using System.ComponentModel.DataAnnotations;

namespace SearchAPI.Models.Domain
{
    public class SearchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Query { get; set; }

        [Required]
        public DateTime SearchDate { get; set; }
    }
}
