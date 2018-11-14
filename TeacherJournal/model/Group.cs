using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class Group : VocabularyEntity
    {
        public Group() { }

        public Group(long idTerm)
        {
            this.idTerm = idTerm;
            this.id = 0;
        }

        public Group(long id, long idTerm, String name)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.name = name;
        }
    }
}
