using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherJournal.model;

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
            execute("INSERT INTO Term (name, beginDate, endDate) VALUES ('{0}', '{1}', '{2}');",
                    term.name, term.beginDate.ToString("dd.MM.yyyy"), term.endDate.ToString("dd.MM.yyyy"));
            connection.Close();
        }

        public static void updateTerm(Term term)
        {
            connection.Open();
            execute("UPDATE Term SET name = '{0}', beginDate = '{1}', endDate = '{2}' WHERE id = '{3}');",
                    term.name, term.beginDate.ToString("dd.MM.yyyy"), term.endDate.ToString("dd.MM.yyyy"), term.id);
            connection.Close();
        }

        public static void deleteTerm(Term term)
        {
            connection.Open();
            execute("DELETE FROM Term WHERE id = {0};;", term.id);
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
                    record.GetString(2),
                    record.GetString(3)));
            }
            connection.Close();

            return terms;
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

        public static void updateSubject(Classroom classroom)
        {
            connection.Open();
            execute("UPDATE Classroom SET name = '{0}' WHERE id = {1};", classroom.name, classroom.id);
            connection.Close();
        }

        public static void deleteSubject(Classroom classroom)
        {
            connection.Open();
            execute("DELETE FROM Classroom WHERE id = {0};", classroom.id);
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
        //-----Schedule-----
        public static void addShedule(Schedule schedule)
        {
            connection.Open();
            for (int i = 0; i < schedule.groups.Count(); i++)
            {
                execute("INSERT INTO Schedule (idTerm, idTypeOfWeek, idDayOfWeek, numOfLesson, idSubject, idTypeOfLesson, idClassroom, idGroup) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8});",
                    schedule.idTerm, schedule.typeOfWeek.id, schedule.dayOfWeek.id, schedule.numOfLesson, schedule.subject.id, schedule.typeOfLesson.id, schedule.classroom.id, schedule.groups[i].id);
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
                "WHERE idTerm = {0};", term.id);

            /*foreach (DbDataRecord record in reader)
            {
                if(schedules.Count() == 0)
                {
                    schedules.Add(new Schedule(record.GetInt64(0),
                        record.GetInt64(1),
                        new TypeOfWeek(record.GetInt64(2), record.GetString(3)),
                        new DayOfWeek(record.GetInt64(4), record.GetString(5)),
                        record.GetInt32(6),
                        new Subject(record.GetInt64(7), record.GetInt64(2), record.GetString(8)),
                        new TypeOfLesson(record.GetInt64(9), record.GetString(10)),
                        new Classroom(record.GetInt64(11), record.GetInt64(2), record.GetString(12)),
                        new List<Group>new Group(record.GetInt64(13), record.GetInt64(2), record.GetString(14))));
                        record.GetInt32(4), record.GetInt64(5), record.GetInt64(6), record.GetInt64(7), record.GetInt64(8)));
                }
                classrooms.Add(new Classroom(record.GetInt64(0),
                    record.GetInt64(1),
                    record.GetString(2)));
            }
            connection.Close();*/

            return schedules;
        }
        //-----------
        private static SQLiteDataReader execute(String cmd, params object[] parameters)
        {
            String pragmaKey = "PRAGMA foreign_keys=ON;";
            SQLiteDataReader reader = new SQLiteCommand(pragmaKey + String.Format(cmd, parameters), connection).ExecuteReader();
            return reader;
        }
    }
}
