using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application._Event.Model.Request
{
    public class EventConfirmationModel
    {
        [Range(1, 60)]
        public int BookTimeInMinutes { get; set; }
        [Range(1, 1000)]
        public int DaysForUpdate { get; set; }
    }
}
