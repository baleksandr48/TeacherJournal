using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class DayOfWeek : IComparable
    {
        public long id { get; set; }
        public String name { get; set; }

        public DayOfWeek(long id, String name)
        {
            this.id = id;
            this.name = name;
        }
        int IComparable.CompareTo(object obj)
        {
            DayOfWeek d = (DayOfWeek) obj;
            if(this.id > d.id)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
