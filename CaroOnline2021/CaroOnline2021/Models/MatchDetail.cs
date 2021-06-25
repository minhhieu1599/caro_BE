using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CaroOnline2021.Models
{
    [Table("MatchDetails")]
    public class MatchDetail
    {
        [Key]
        public Guid Id { get; set; }
  
        public int Order { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int SortNumber { get; set; }
        public Guid UserId { get; set; }


        public Match Match { get; set; }
        public Guid MatchId { get; set; }
    }
}
