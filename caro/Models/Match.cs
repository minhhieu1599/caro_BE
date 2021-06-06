using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace caro.Models
{
    [Table("Matches")]
    public class Match
    {
        [Key]
        public Guid Id { get; set; }
        public Guid RoomId { get; set; }
        public Guid FirstUserId { get; set; }
        public Guid SecondUserId { get; set; }
        public Guid CreatedRoomUserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public int FirstScore { get; set; }
        public int SecondScore { get; set; }
        public Guid WinnerId { get; set; }
        public List<MatchDetal> MatchDetals { get; set; }

    }
}
