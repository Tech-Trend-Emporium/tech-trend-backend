using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Dto.Review
{
    public class CreateReviewRequest
    {
        public float Rating { get; set; }
        public string Comment { get; set; }
        public int ProductId { get; set; }
        public string Username { get; set; }
    }
}
