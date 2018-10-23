using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    class DayOfWeek
    {
        public long id { get; set; }
        public String name { get; set; }

        public DayOfWeek(long id, String name)
        {
            this.id = id;
            this.name = name;
        }
    }
}
