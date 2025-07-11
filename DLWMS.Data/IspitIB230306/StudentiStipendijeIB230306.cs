using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DLWMS.Data.IspitIB230306
{
    public class StudentiStipendijeIB230306
    {
        public int Id { get; set; }
        public int StipendijaGodinaId { get; set; }
        public StipendijeGodineIB230306 StipendijaGodina{ get; set; }
        public int StudentId { get; set; }
        public Student Student { get; set; }
        public override string ToString()
        {
            return Id.ToString();
        }
    }
}
