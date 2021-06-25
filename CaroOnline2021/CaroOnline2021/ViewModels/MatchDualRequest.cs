using System.Collections.Generic;
using CaroOnline2021.Models;

namespace CaroOnline2021.ViewModels
{
    public class MatchDualRequest
    {
        public string RoomId { get; set; }
        public string FirstUserId { get; set; }
        public string SecondUserId { get; set; }
        public List<User> Users { get; set; }
        public List<MatchDetail> MatchDetails { get; set; }
    }
}
