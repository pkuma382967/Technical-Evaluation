using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SearchAPI.Models.Domain
{
    public class Product
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Date the product was added or released
        public DateTime Date { get; set; }

        // Popularity score (e.g., number of purchases or views)
        public int Popularity { get; set; }

        // Relevance score for search (e.g., calculated by some algorithm)
        [Range(0, 1)]
        public float Relevance { get; set; }
    }
}
