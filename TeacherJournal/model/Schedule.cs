using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{
    public class Schedule
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

        public Schedule() { }
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
        // Расписание считается неправильным если в один момент времени преподу надо находиться в двух местах.
        public static bool isSchedulesCorrect(List<Schedule> schedules)
        {
            for (int i = 0; i < schedules.Count; i++)
            {
                Schedule s1 = schedules[i];
                for (int j = i + 1; j < schedules.Count; j++)
                {
                    Schedule s2 = schedules[j];

                    if (s1.numOfLesson == s2.numOfLesson &&
                        s1.dayOfWeek.id == s2.dayOfWeek.id &&
                        (s1.typeOfWeek.id == s2.typeOfWeek.id ||
                        s1.typeOfWeek.id == 3 ||
                        s2.typeOfWeek.id == 3))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
