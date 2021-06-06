using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace caro.Models
{
    [Table("UserRooms") ]
    public class UserRoom
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Room Room { get; set; }
        public Guid RoomId { get; set; } 
    }
}
