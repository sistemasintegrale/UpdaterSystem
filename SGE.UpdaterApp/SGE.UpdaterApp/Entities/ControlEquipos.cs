using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGE.UpdaterApp.Entities
{
    public class ControlEquipos : ControlVersiones
    {
        public int ceq_icod_equipo { get; set; }

        public string ceq_vnombre_equipo { get; set; } = string.Empty;  

        public int cvr_icod_version { get; set; }

        public DateTime? ceq_sfecha_actualizacion { get; set; }

        public string cep_vubicacion_sistema { get; set; } = string.Empty;
    }
}
