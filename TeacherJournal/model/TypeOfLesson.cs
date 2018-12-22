using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class TypeOfLesson
    {
        public long id { get; set; }
        public String name { get; set; }
        public String strForWord { get; set; }

        public TypeOfLesson(long id, String name, String strForWord)
        {
            this.id = id;
            this.name = name;
            this.strForWord = strForWord;
        }
    }
}
