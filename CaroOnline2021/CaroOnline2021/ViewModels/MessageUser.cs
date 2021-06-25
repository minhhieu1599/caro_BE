using System;

namespace CaroOnline2021.ViewModels
{
    public class MessageUser
    {
        public Guid UserId { get; set; }
        public string Message { get; set; }
        public Guid RoomId { get; set; }
    }
}
