using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class Subject : VocabularyEntity
    {
        public Subject() { }

        public Subject(long idTerm)
        {
            this.idTerm = idTerm;
        }

        public Subject(long id, long idTerm, string name)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.name = name;
        }
    }
}
