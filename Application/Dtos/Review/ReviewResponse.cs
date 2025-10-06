using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Review
{
    public class ReviewResponse
    {
        public int Id { get; set; }
        public string? Comment { get; set; }
        public float Rating { get; set; }
        public string Username { get; set; } = null!;
        public int ProductId { get; set; }
    }
}
