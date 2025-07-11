using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLWMS.Data.IspitIB230306
{
    public class StipendijeGodineIB230306
    {
        public int Id { get; set; }
        public int StipendijaId { get; set; }
        public StipendijeIB230306 Stipendija { get; set; }
        public int Godina { get; set; }
        public int Iznos { get; set; }
        public bool Status { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }

    }
}
