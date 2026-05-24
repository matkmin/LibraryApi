using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Author { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string ISBN { get; set; } = string.Empty;

        public int PublishedYear { get; set; }

        public int TotalCopies { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();
    }
}
