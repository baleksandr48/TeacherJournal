using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class Classroom : VocabularyEntity
    {
        public Classroom() { }

        public Classroom(long idTerm)
        {
            this.idTerm = idTerm;
        }

        public Classroom(long id, long idTerm, string name)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.name = name;
        }
        
    }
}
