using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class TypeOfWeek
    {
        public long id { get; set; }
        public String name { get; set; }

        public TypeOfWeek(long id, String name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
