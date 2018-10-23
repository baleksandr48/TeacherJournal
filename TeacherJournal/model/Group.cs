using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    class Group
    {
        public long id { get; set; }
        public long idTerm { get; set; }
        public String name { get; set; }

        public Group(long id, long idTerm, String name)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.name = name;
        }
    }
}
