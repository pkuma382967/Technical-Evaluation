using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchAPI.Models.Domain
{
    public class SearchHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(max)")]
        public string Query { get; set; }

        [Required]
        public DateTime SearchDate { get; set; }
    }
}
