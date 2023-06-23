using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LogReg.Models;

    public class Guests
    {
        [Key]
        public int GuestsId { get; set; }

        public int UserId { get; set; }
        public User? User { get; set; }

        public int WeddingId { get; set; }
        public Wedding? Wedding { get; set; }
    }

