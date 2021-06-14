using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CaroOnline2021.ViewModels
{
    public class MessagerUser
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public string Message { get; set; }
    }
}
