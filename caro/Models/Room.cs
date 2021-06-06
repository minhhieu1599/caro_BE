using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace caro.Models
{
    [Table("Rooms")]
    public class Room
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        //inactive - 0active - 1 inprocessing -2
        public int status { get; set; }

        public List<Message> Messages { get; set; }
    }
}
