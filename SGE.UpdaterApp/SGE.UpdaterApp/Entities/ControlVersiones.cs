using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGE.UpdaterApp.Entities
{
    public class ControlVersiones
    {
        public int cvr_icod_version { get; set; }
        public string cvr_vversion { get; set; } = string.Empty;
        public string? NombrePvt { get; set; } 
        public DateTime? cvr_sfecha_version { get; set; }
        public string cvr_vurl { get; set; } = string.Empty; 
    }
}
