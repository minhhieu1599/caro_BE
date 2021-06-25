using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaroOnline2021.Models
{
    [Table("UserRooms")]
    public class UserRoom
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }
        public Room Room { get; set; }
        public Guid RoomId { get; set; }

        public DateTime CreatedDate { get; set; }

    }
}
