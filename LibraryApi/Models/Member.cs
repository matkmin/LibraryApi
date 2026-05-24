using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Member
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string SsoSubject { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; }

        public ICollection<Loan> Loans { get; set; } = new List<Loan>();

    }
}
