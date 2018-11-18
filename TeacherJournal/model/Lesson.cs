using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherJournal.model
{

    public class Lesson
    {
        public long id { get; set; }
        public DateTime date { get; set; }
        public int countOfHours { get; set; }
        public int numOfLesson { get; set; }
        public Classroom classroom { get; set; }
        public long idTerm { get; set; }
        public Subject subject { get; set; }
        public List<Group> groups { get; set; }
        public TypeOfLesson typeOfLesson { get; set; }
        public String theme { get; set; }

        public Lesson(long id, DateTime date, int countOfHours, int numOfLesson,
            Classroom classroom, long idTerm, Subject subject, List<Group> groups, TypeOfLesson typeOfLesson, string theme)
        {
            this.id = id;
            this.date = date;
            this.countOfHours = countOfHours;
            this.numOfLesson = numOfLesson;
            this.classroom = classroom;
            this.idTerm = idTerm;
            this.subject = subject;
            this.groups = groups;
            this.typeOfLesson = typeOfLesson;
            this.theme = theme;
        }

        public static List<Lesson> generateLessons(List<Schedule> schedules, Term term)
        {
            List<Lesson> lessons = new List<Lesson>();

            var map = new Dictionary<string, int>();
            map.Add("Monday", 1);
            map.Add("Tuesday", 2);
            map.Add("Wednesday", 3);
            map.Add("Thursday", 4);
            map.Add("Friday", 5);
            map.Add("Saturday", 6);
            map.Add("Sunday", 7);

            // 1 - берем день недели первого дня семестра
            int termFirstDay;
            map.TryGetValue(term.beginDate.DayOfWeek.ToString(), out termFirstDay);

            foreach (Schedule schedule in schedules)
            {
                // 2 - узнаем в какой день недели проводится это занятие
                int daySchedule = (int)schedule.dayOfWeek.id;

                // если занятие по знаменателю, а семестр начинается с числителя - отступаем 1 неделю
                int slip;
                if ((int)schedule.typeOfWeek.id == 3 ||
                    (int)schedule.typeOfWeek.id == term.startFromNumerator ||
                    (int)schedule.typeOfWeek.id == term.startFromNumerator + 2)
                {
                    slip = 0;
                }
                else
                {
                    slip = 7;
                }

                // 4 - прибавляем дни к началу семестра и узнаем дату первого занятия
                DateTime dateLesson;
                if (daySchedule >= termFirstDay)
                {
                    dateLesson = term.beginDate.AddDays(daySchedule - termFirstDay + slip);
                }
                else
                {
                    dateLesson = term.beginDate.AddDays(7 - termFirstDay + daySchedule + slip);
                }
                // 5 - расчитываем все занятия до конца семестра
                while (DateTime.Compare(dateLesson, term.endDate) < 0)
                {
                    lessons.Add(new Lesson(0, dateLesson, 2, schedule.numOfLesson, schedule.classroom,
                        term.id, schedule.subject, schedule.groups, schedule.typeOfLesson, null));
                    if ((int)schedule.typeOfWeek.id == 3)
                    {
                        dateLesson = dateLesson.AddDays(7);
                    }
                    else
                    {
                        dateLesson = dateLesson.AddDays(14);
                    }
                }
            }

            return lessons;
        }

    }

}
