using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class BadRequestException : DomainException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
