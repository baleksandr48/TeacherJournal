using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    class Schedule
    {
        public long id { get; set; }
        public long idTerm { get; set; }
        public TypeOfWeek typeOfWeek { get; set; }
        public DayOfWeek dayOfWeek { get; set; }
        public int numOfLesson { get; set; }
        public Subject subject { get; set; }
        public TypeOfLesson typeOfLesson { get; set; }
        public Classroom classroom { get; set; }
        public List<Group> groups { get; set; }

        public Schedule(long id, long idTerm, TypeOfWeek typeOfWeek, DayOfWeek dayOfWeek, int numOfLesson, Subject subject, TypeOfLesson typeOfLesson, Classroom classroom, List<Group> groups)
        {
            this.id = id;
            this.idTerm = idTerm;
            this.typeOfWeek = typeOfWeek;
            this.dayOfWeek = dayOfWeek;
            this.numOfLesson = numOfLesson;
            this.subject = subject;
            this.typeOfLesson = typeOfLesson;
            this.classroom = classroom;
            this.groups = groups;
        }
    }
}
