using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Exceptions
{
    public class PendingApprovalException : Exception
    {
        public object Payload { get; }

        public PendingApprovalException(string message, object payload) : base(message)
        {
            Payload = payload;
        }
    }
}
