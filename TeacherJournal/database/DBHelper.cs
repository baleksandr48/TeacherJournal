using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherJournal.model;
using TeacherJournal.view;

namespace TeacherJournal.database
{
    static class DBHelper
    {
        static private String dbName = System.IO.Path.Combine(Environment.CurrentDirectory, "journal.db");
        static private SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName));

        //-----Term-----
        public static void addTerm(Term term)
        {
            connection.Open();
            execute("INSERT INTO Term (name, beginDate, endDate, startFromNumerator) VALUES ('{0}', '{1}', '{2}', {3});",
                    term.name, calculateDays(term.beginDate), calculateDays(term.endDate), term.startFromNumerator);
            connection.Close();
        }

        public static void updateTerm(Term term)
        {
            connection.Open();
            execute("UPDATE Term SET name = '{0}' WHERE id = '{1}');",
                    term.name, term.id);
            connection.Close();
        }

        public static void deleteTerm(Term term)
        {
            connection.Open();
            execute("DELETE FROM Term WHERE id = {0};", term.id);
            connection.Close();
        }

        public static List<Term> selectTerms()
        {
            connection.Open();
            List<Term> terms = new List<Term>();
            SQLiteDataReader reader = execute("SELECT * FROM Term;");

            foreach (DbDataRecord record in reader)
            {
                terms.Add(new Term(record.GetInt64(0),
                    record.GetString(1),
                    intToDateTime((int)record.GetInt64(2)),
                    intToDateTime((int)record.GetInt64(3)),
                    (int)record.GetInt64(4)));
            }
            connection.Close();

            return terms;
        }

        public static Term getLastTerm()
        {
            List<Term> terms = selectTerms();
            Term lastTerm = terms.ElementAt(terms.Count - 1);
            return lastTerm;
        }

        //-----Group-----
        public static void addGroup(Group group)
        {
            connection.Open();
            execute("INSERT INTO _Group (name, idTerm) VALUES ('{0}', '{1}');",
                    group.name, group.idTerm);
            connection.Close();
        }

        public static void updateGroup(Group group)
        {
            connection.Open();
            execute("UPDATE _Group SET name = '{0}' WHERE id = {1};",
                    group.name, group.id);
            connection.Close();
        }

        public static void deleteGroup(Group group)
        {
            connection.Open();
            execute("DELETE FROM _Group WHERE id = {0};", group.id);
            connection.Close();
        }

        public static void clearGroup(Term term)
        {
            connection.Open();
            execute("DELETE FROM _Group WHERE idTerm = {0};", term.id);
            connection.Close();
        }

        public static List<Group> selectGroups(Term term)
        {
            connection.Open();
            List<Group> groups = new List<Group>();
            SQLiteDataReader reader = execute("SELECT * FROM _Group WHERE idTerm = {0};", term.id);

            foreach (DbDataRecord record in reader)
            {
                groups.Add(new Group(record.GetInt64(0),
                    record.GetInt64(1),
                    record.GetString(2)));
            }
            connection.Close();

            return groups;
        }

        //-----Subject-----
        public static void addSubject(Subject subject)
        {
            connection.Open();
            execute("INSERT INTO Subject (name, idTerm) VALUES ('{0}', '{1}');",
                    subject.name, subject.idTerm);
            connection.Close();
        }

        public static void updateSubject(Subject subject)
        {
            connection.Open();
            execute("UPDATE Subject SET name = '{0}' WHERE id = {1};",
                    subject.name, subject.id);
            connection.Close();
        }

        public static void deleteSubject(Subject subject)
        {
            connection.Open();
            execute("DELETE FROM Subject WHERE id = {0};", subject.id);
            connection.Close();
        }

        public static void clearSubject(Term term)
        {
            connection.Open();
            execute("DELETE FROM Subject WHERE idTerm = {0};", term.id);
            connection.Close();
        }

        public static List<Subject> selectSubject(Term term)
        {
            connection.Open();
            List<Subject> subjects = new List<Subject>();
            SQLiteDataReader reader = execute("SELECT * FROM Subject WHERE idTerm = {0};", term.id);

            foreach (DbDataRecord record in reader)
            {
                subjects.Add(new Subject(record.GetInt64(0),
                    record.GetInt64(1),
                    record.GetString(2)));
            }
            connection.Close();

            return subjects;
        }

        //-----Classroom-----
        public static void addClassroom(Classroom classroom)
        {
            connection.Open();
            execute("INSERT INTO Classroom (name, idTerm) VALUES ('{0}', '{1}');", classroom.name, classroom.idTerm);
            connection.Close();
        }

        public static void updateClassroom(Classroom classroom)
        {
            connection.Open();
            execute("UPDATE Classroom SET name = '{0}' WHERE id = {1};", classroom.name, classroom.id);
            connection.Close();
        }

        public static void deleteClassroom(Classroom classroom)
        {
            connection.Open();
            execute("DELETE FROM Classroom WHERE id = {0};", classroom.id);
            connection.Close();
        }

        public static void clearClassroom(Term term)
        {
            connection.Open();
            execute("DELETE FROM Classroom WHERE idTerm = {0};", term.id);
            connection.Close();
        }

        public static List<Classroom> selectClassroom(Term term)
        {
            connection.Open();
            List<Classroom> classrooms = new List<Classroom>();
            SQLiteDataReader reader = execute("SELECT * FROM Classroom WHERE idTerm = {0};", term.id);

            foreach (DbDataRecord record in reader)
            {
                classrooms.Add(new Classroom(record.GetInt64(0),
                    record.GetInt64(1),
                    record.GetString(2)));
            }
            connection.Close();

            return classrooms;
        }

        public static List<Classroom> selectAllClassrooms()
        {
            connection.Open();
            List<Classroom> classrooms = new List<Classroom>();
            SQLiteDataReader reader = execute("SELECT * FROM Classroom");

            foreach (DbDataRecord record in reader)
            {
                classrooms.Add(new Classroom(record.GetInt64(0),
                    record.GetInt64(1),
                    record.GetString(2)));
            }
            connection.Close();

            return classrooms;
        }

        //-----Schedule-----
        public static void addShedule(Schedule schedule)
        {
            connection.Open();
            for (int i = 0; i < schedule.groups.Count(); i++)
            {
                execute("INSERT INTO Schedule (idTerm, idTypeOfWeek, idDayOfWeek, numOfLesson, idSubject, idTypeOfLesson, idClassroom, idGroup) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
                    schedule.idTerm, schedule.typeOfWeek.id, schedule.dayOfWeek.id, schedule.numOfLesson, schedule.subject.id, schedule.typeOfLesson.id, schedule.classroom.id, schedule.groups[i].id);
            }
            connection.Close();
        }

        public static void addShedules(List<Schedule> schedules)
        {
            connection.Open();
            foreach (Schedule schedule in schedules)
            {
                foreach (Group group in schedule.groups)
                {
                    execute("INSERT INTO Schedule (idTerm, idTypeOfWeek, idDayOfWeek, numOfLesson, idSubject, idTypeOfLesson, idClassroom, idGroup) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
                    schedule.idTerm, schedule.typeOfWeek.id, schedule.dayOfWeek.id, schedule.numOfLesson, schedule.subject.id, schedule.typeOfLesson.id, schedule.classroom.id, group.id);
                }
            }
            connection.Close();
        }

        public static List<Schedule> selectSchedules(Term term)
        {
            connection.Open();
            List<Schedule> schedules = new List<Schedule>();
            SQLiteDataReader reader = execute("SELECT Schedule.id, Schedule.idTerm, TypeOfWeek.id, TypeOfWeek.name, DayOfWeek.id, DayOfWeek.name, " +
                "Schedule.numOfLesson, Subject.id, Subject.name, TypeOfLesson.id, TypeOfLesson.name, Classroom.id, Classroom.name, _Group.id, _Group.name " +
                "FROM (((((Schedule INNER JOIN TypeOfWeek ON Schedule.idTypeOfWeek = TypeOfWeek.id) " +
                "INNER JOIN DayOfWeek ON Schedule.idDayOfWeek = DayOfWeek.id) " +
                "INNER JOIN Subject ON Schedule.idSubject = Subject.id) " +
                "INNER JOIN TypeOfLesson ON Schedule.idTypeOfLesson = TypeOfLesson.id) " +
                "INNER JOIN Classroom ON Schedule.idClassroom = Classroom.id) " +
                "INNER JOIN _Group ON Schedule.idGroup = _Group.id " +
                "WHERE Schedule.idTerm = {0};", term.id);
            foreach (DbDataRecord record in reader)
            {
                long id = record.GetInt64(0);
                long idTerm = record.GetInt64(1);
                TypeOfWeek typeOfWeek = new TypeOfWeek(record.GetInt64(2), record.GetString(3));
                model.DayOfWeek dayOfWeek = new model.DayOfWeek(record.GetInt64(4), record.GetString(5));
                int numOfLesson = (int)record.GetInt64(6);
                Subject subject = new Subject(record.GetInt64(7), record.GetInt64(2), record.GetString(8));
                TypeOfLesson typeOfLesson = new TypeOfLesson(record.GetInt64(9), record.GetString(10));
                Classroom classroom = new Classroom(record.GetInt64(11), record.GetInt64(2), record.GetString(12));
                Group group = new Group(record.GetInt64(13), record.GetInt64(2), record.GetString(14));

                if (schedules.Count() == 0)
                {
                    List<Group> groups = new List<Group>();
                    groups.Add(group);
                    schedules.Add(new Schedule(id, idTerm, typeOfWeek, dayOfWeek, numOfLesson, subject, typeOfLesson, classroom, groups));
                }
                else
                {
                    Schedule s = schedules[schedules.Count() - 1];
                    if (s.idTerm == idTerm &&
                        s.typeOfWeek == typeOfWeek &&
                        s.dayOfWeek == dayOfWeek &&
                        s.numOfLesson == numOfLesson &&
                        s.subject == subject &&
                        s.typeOfLesson == typeOfLesson &&
                        s.classroom == classroom)
                    {
                        schedules[schedules.Count() - 1].groups.Add(group);
                    }
                    else
                    {
                        List<Group> groups = new List<Group>();
                        groups.Add(group);
                        schedules.Add(new Schedule(id, idTerm, typeOfWeek, dayOfWeek, numOfLesson, subject, typeOfLesson, classroom, groups));
                    }
                }
            }
            connection.Close();

            return schedules;
        }

        //-----Lesson-----

        public static void addLesson(Lesson lesson)
        {
            connection.Open();
            for (int i = 0; i < lesson.groups.Count(); i++)
            {
                execute("INSERT INTO Lesson (idTerm, _date, countOfHours, numOfLesson, idSubject, " +
                    "idTypeOfLesson, idClassroom, idGroup, theme) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}');",
                    lesson.idTerm, calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                    lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme);
            }
            connection.Close();
        }

        public static void addLessons(List<Lesson> lessons)
        {
            connection.Open();
            foreach (Lesson lesson in lessons)
            {
                for (int i = 0; i < lesson.groups.Count(); i++)
                {
                    execute("INSERT INTO Lesson (idTerm, _date, countOfHours, numOfLesson, idSubject, " +
                        "idTypeOfLesson, idClassroom, idGroup, theme) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}');",
                        lesson.idTerm, calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                        lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme);
                }
            }
            connection.Close();
        }

        public static void updateLesson(Lesson lesson)
        {
            connection.Open();
            for (int i = 0; i < lesson.groups.Count(); i++)
            {
                execute("UPDATE Lesson SET _date = {0}, countOfHours={1}, numOfLesson={2}, idSubject={3}, " +
                "idTypeOfLesson={4}, idClassroom={5}, idGroup={6}, theme='{7}' WHERE id = {8};",
                calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                        lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme, lesson.id);
            }
            connection.Close();
        }

        public static void deleteLesson(Lesson lesson)
        {
            connection.Open();
            execute("DELETE FROM Lesson WHERE id = {0};", lesson.id);
            connection.Close();
        }

        public static List<Lesson> selectLessons(Term term, DateTime startDate, DateTime endDate)
        {
            connection.Open();
            List<Lesson> lessons = new List<Lesson>();
            SQLiteDataReader reader = execute("SELECT Lesson.id, Lesson._date, Lesson.countOfHours, Lesson.numOfLesson, " +
                "Subject.id, Subject.name, TypeOfLesson.id, TypeOfLesson.name, Classroom.id, Classroom.name, _Group.id, _Group.name, Lesson.theme " +
                "FROM (((Lesson INNER JOIN Subject ON Lesson.idSubject = Subject.id) " +
                "INNER JOIN TypeOfLesson ON Lesson.idTypeOfLesson = TypeOfLesson.id) " +
                "INNER JOIN Classroom ON Lesson.idClassroom = Classroom.id) " +
                "INNER JOIN _Group ON Lesson.idGroup = _Group.id " +
                "WHERE Lesson.idTerm = {0} AND Lesson._date >= {1} AND Lesson._date <= {2};", term.id, calculateDays(startDate), calculateDays(endDate));

            foreach (DbDataRecord record in reader)
            {
                long id = record.GetInt64(0);
                long idTerm = term.id;
                DateTime date = intToDateTime((int)record.GetInt64(1));
                int countOfHours = (int)record.GetInt64(2);
                int numOfLesson = (int)record.GetInt64(3);
                Subject subject = new Subject(record.GetInt64(4), idTerm, record.GetString(5));
                TypeOfLesson typeOfLesson = new TypeOfLesson(record.GetInt64(6), record.GetString(7));
                Classroom classroom = new Classroom(record.GetInt64(8), idTerm, record.GetString(9));
                Group group = new Group(record.GetInt64(10), idTerm, record.GetString(11));
                String theme = record.GetString(12);

                if (lessons.Count() == 0)
                {
                    List<Group> groups = new List<Group>();
                    groups.Add(group);
                    lessons.Add(new Lesson(id, date, countOfHours, numOfLesson, classroom, idTerm, subject, groups, typeOfLesson, theme));
                }
                else
                {
                    Lesson l = lessons[lessons.Count() - 1];
                    if (l.idTerm == idTerm &&
                        l.date == date &&
                        l.countOfHours == countOfHours &&
                        l.numOfLesson == numOfLesson &&
                        l.subject == subject &&
                        l.typeOfLesson == typeOfLesson &&
                        l.classroom == classroom &&
                        l.theme == theme)
                    {
                        lessons[lessons.Count() - 1].groups.Add(group);
                    }
                    else
                    {
                        List<Group> groups = new List<Group>();
                        groups.Add(group);
                        lessons.Add(new Lesson(id, date, countOfHours, numOfLesson, classroom, idTerm, subject, groups, typeOfLesson, theme));
                    }

                }
            }
            connection.Close();
            return lessons;
        }
        //-----------DayOfWeek----------
        public static List<model.DayOfWeek> selectDaysOfWeek()
        {
            connection.Open();
            List<model.DayOfWeek> daysOfWeek = new List<model.DayOfWeek>();
            SQLiteDataReader reader = execute("SELECT * FROM DayOfWeek");

            foreach (DbDataRecord record in reader)
            {
                daysOfWeek.Add(new model.DayOfWeek(record.GetInt64(0),
                    record.GetString(1)));
            }
            connection.Close();

            return daysOfWeek;
        }
        //-----------TypeOfWeek----------
        public static List<TypeOfWeek> selectTypesOfWeek()
        {
            connection.Open();
            List<TypeOfWeek> typesOfWeek = new List<TypeOfWeek>();
            SQLiteDataReader reader = execute("SELECT * FROM TypeOfWeek");

            foreach (DbDataRecord record in reader)
            {
                typesOfWeek.Add(new TypeOfWeek(record.GetInt64(0),
                    record.GetString(1)));
            }
            connection.Close();

            return typesOfWeek;
        }
        //-----------TypeOfLesson----------
        public static List<TypeOfLesson> selectTypesOfLesson()
        {
            connection.Open();
            List<TypeOfLesson> typesOfLesson = new List<TypeOfLesson>();
            SQLiteDataReader reader = execute("SELECT * FROM TypeOfLesson");

            foreach (DbDataRecord record in reader)
            {
                typesOfLesson.Add(new TypeOfLesson(record.GetInt64(0),
                    record.GetString(1)));
            }
            connection.Close();

            return typesOfLesson;
        }

        //-----------
        private static SQLiteDataReader execute(String cmd, params object[] parameters)
        {
            String pragmaKey = "PRAGMA foreign_keys=ON;";
            SQLiteDataReader reader = new SQLiteCommand(pragmaKey + String.Format(cmd, parameters), connection).ExecuteReader();
            return reader;
        }

        public static int calculateDays(DateTime date)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);// 1970/1/1 00:00:00 
            TimeSpan result = date.Subtract(dt);
            return Convert.ToInt32(result.TotalDays);
        }

        public static DateTime intToDateTime(int days)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);// 1970/1/1 00:00:00 
            return dt.AddDays(days);
        }

    }
}
