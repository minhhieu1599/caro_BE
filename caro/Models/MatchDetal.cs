using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace caro.Models
{
    [Table("MatchDetals")]
    public class MatchDetal
    {
        [Key]
        public Guid ID { get; set; }

        public int Order { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int SortNumber { get; set; }
        public Guid UserId { get; set; }

        public Match Match { get; set; }
        public Guid MatchID { get; set; }
    }
}
