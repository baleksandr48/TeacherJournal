using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    class Classroom
    {
        public long id { get; set; }
        public long idTerm { get; set; }
        public String name { get; set; }

        public Classroom(long id, long idTerm, string name)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.name = name;
        }
    }
}
