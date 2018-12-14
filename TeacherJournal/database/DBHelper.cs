using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherJournal.model;
using TeacherJournal.view;

namespace TeacherJournal.database
{
    static class DBHelper
    {
        static private String dbName = System.IO.Path.Combine(Environment.CurrentDirectory, "../../database/journal.db");
        static private SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName));
        static private String pragmaKeyON = "PRAGMA foreign_keys=ON;";
        
        //-----Term-----
        //добавить 1 семестр
        public static void addTerm(Term term)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("INSERT INTO Term (name, beginDate, endDate, startFromNumerator) VALUES ('{0}', '{1}', '{2}', {3});",
                        term.name, calculateDays(term.beginDate), calculateDays(term.endDate), term.startFromNumerator);
                    command.ExecuteNonQuery();
                    
                }
                connection.Close();
            }
        }
        //добавить список семестров
        public static void addTerms(List<Term> terms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Term term in terms)
                        {
                            command.CommandText = String.Format("INSERT INTO Term (name, beginDate, endDate, startFromNumerator) VALUES ('{0}', '{1}', '{2}', {3});",
                                term.name, calculateDays(term.beginDate), calculateDays(term.endDate), term.startFromNumerator);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        //обновляет 1 семестр
        public static void updateTerm(Term term)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("UPDATE Term SET name = {0}, beginDate = {1}, endDate = {2}, startFromNumerator = {3} WHERE id = {4};", term.name, calculateDays(term.beginDate), 
                        calculateDays(term.endDate), term.startFromNumerator, term.id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        //обновляет список семестров
        public static void updateTerms(List<Term> terms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        command.Transaction = transaction;
                        foreach (Term term in terms)
                        {
                            command.CommandText = String.Format("UPDATE Term SET name = '{0}', beginDate = '{1}', endDate = '{2}', " +
                        "startFromNumerator = '{3}' WHERE id = '{4}');", term.name, calculateDays(term.beginDate),
                        calculateDays(term.endDate), term.startFromNumerator, term.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        //удаляет 1 семестр
        public static void deleteTerm(Term term)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE FROM Term WHERE id = {0};", term.id);
                    command.ExecuteNonQuery();
                } 
                connection.Close();
            }
        }
        //удаляет список семестров
        public static void deleteTerms(List<Term> terms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Term term in terms)
                        {
                            command.CommandText = String.Format("DELETE FROM Term WHERE id = {0};", term.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        //достаем список семестров
        public static List<Term> selectTerms()
        {
            List<Term> terms = new List<Term>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM Term;");

                    SQLiteDataReader reader = command.ExecuteReader();
                    foreach (DbDataRecord record in reader)
                    {
                        terms.Add(new Term(record.GetInt64(0),
                            record.GetString(1),
                            intToDateTime((int)record.GetInt64(2)),
                            intToDateTime((int)record.GetInt64(3)),
                            (int)record.GetInt64(4)));
                    }
                }
                connection.Close();
            }

            return terms;
        }

        public static Term getLastTerm()
        {
            List<Term> terms = selectTerms();
            Term lastTerm = terms.ElementAt(terms.Count - 1);
            return lastTerm;
        }

        public static void ClearTerms()
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE From Term");
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        //-----Group-----
        public static void addGroup(Group group)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("INSERT INTO _Group (name, idTerm) VALUES ('{0}', '{1}');", group.name, group.idTerm);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void addGroups(List<Group> groups)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach(Group group in groups)
                        {
                            command.CommandText = String.Format("INSERT INTO _Group (name, idTerm) VALUES ('{0}', '{1}');", group.name, group.idTerm);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void updateGroup(Group group)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("UPDATE _Group SET name = '{0}' WHERE id = {1};", group.name, group.id);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void updateGroups(List<Group> groups)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Group group in groups)
                        {
                            command.CommandText = String.Format("UPDATE _Group SET name = '{0}' WHERE id = {1};", group.name, group.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void deleteGroup(Group group)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE FROM _Group WHERE id = {0};", group.id);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void deleteGroups(List<Group> groups)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Group group in groups)
                        {
                            command.CommandText = String.Format("DELETE FROM _Group WHERE id = {0};", group.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        
        public static List<Group> selectGroups(Term term)
        {
            List<Group> groups = new List<Group>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM _Group WHERE idTerm = {0};", term.id);
                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        groups.Add(new Group(record.GetInt64(0),
                            record.GetInt64(1),
                            record.GetString(2)));
                    }
                }
                connection.Close();
            }
            return groups;
        }

        //-----Subject-----
        public static void addSubject(Subject subject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("INSERT INTO Subject (name, idTerm) VALUES ('{0}', '{1}');", subject.name, subject.idTerm);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void addSubjects(List<Subject> subjects)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();
                        

                        foreach (Subject subject in subjects)
                        {
                            command.CommandText = String.Format("INSERT INTO Subject (name, idTerm) VALUES ('{0}', '{1}');", subject.name, subject.idTerm);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void updateSubject(Subject subject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("UPDATE Subject SET name = '{0}' WHERE id = {1};", subject.name, subject.id);
                    command.ExecuteNonQuery();
                }
                   
                connection.Close();
            }
        }

        public static void updateSubjects(List<Subject> subjects)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Subject subject in subjects)
                        {
                            command.CommandText = String.Format("UPDATE Subject SET name = '{0}' WHERE id = {1};", subject.name, subject.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void deleteSubject(Subject subject)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE FROM Subject WHERE id = {0};", subject.id);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void deleteSubjects(List<Subject> subjects)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Subject subject in subjects)
                        {
                            command.CommandText = String.Format("DELETE FROM Subject WHERE id = {0};", subject.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        
        public static List<Subject> selectSubject(Term term)
        {
            List<Subject> subjects = new List<Subject>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM Subject WHERE idTerm = {0};", term.id);
                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        subjects.Add(new Subject(record.GetInt64(0),
                            record.GetInt64(1),
                            record.GetString(2)));
                    }
                }
                connection.Close();
            }
            return subjects;
        }

        //-----Classroom-----
        public static void addClassroom(Classroom classroom)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("INSERT INTO Classroom (name, idTerm) VALUES ('{0}', '{1}');", classroom.name, classroom.idTerm);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void addClassrooms(List<Classroom> classrooms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Classroom classroom in classrooms)
                        {
                            command.CommandText = String.Format("INSERT INTO Classroom (name, idTerm) VALUES ('{0}', '{1}');", classroom.name, classroom.idTerm);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void updateClassroom(Classroom classroom)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("UPDATE Classroom SET name = '{0}' WHERE id = {1};", classroom.name, classroom.id);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void updateClassrooms(List<Classroom> classrooms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Classroom classroom in classrooms)
                        {
                            command.CommandText = String.Format("UPDATE Classroom SET name = '{0}' WHERE id = {1};", classroom.name, classroom.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void deleteClassroom(Classroom classroom)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE FROM Classroom WHERE id = {0};", classroom.id);
                    command.ExecuteNonQuery();
                }
                    
                connection.Close();
            }
        }

        public static void deleteClassrooms(List<Classroom> classrooms)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Classroom classroom in classrooms)
                        {
                            command.CommandText = String.Format("DELETE FROM Classroom WHERE id = {0};", classroom.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }
        
        public static List<Classroom> selectClassroom(Term term)
        {
            List<Classroom> classrooms = new List<Classroom>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM Classroom WHERE idTerm = {0};", term.id);
                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        classrooms.Add(new Classroom(record.GetInt64(0),
                            record.GetInt64(1),
                            record.GetString(2)));
                    }
                }
                connection.Close();
            }
            return classrooms;
        }

        /*public static List<Classroom> selectAllClassrooms()
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
        }*/

        //-----Schedule-----
        public static void addSchedule(Schedule schedule)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        for (int i = 0; i < schedule.groups.Count(); i++)
                        {
                            command.CommandText = String.Format("INSERT INTO Schedule (idTerm, idTypeOfWeek, idDayOfWeek, numOfLesson, idSubject, idTypeOfLesson, idClassroom, idGroup) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
                                schedule.idTerm, schedule.typeOfWeek.id, schedule.dayOfWeek.id, schedule.numOfLesson, schedule.subject.id, schedule.typeOfLesson.id, schedule.classroom.id, schedule.groups[i].id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void addSchedules(List<Schedule> schedules)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Schedule schedule in schedules)
                        {
                            for (int i = 0; i < schedule.groups.Count(); i++)
                            {
                                command.CommandText = String.Format("INSERT INTO Schedule (idTerm, idTypeOfWeek, idDayOfWeek, numOfLesson, idSubject, idTypeOfLesson, idClassroom, idGroup) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7});",
                                    schedule.idTerm, schedule.typeOfWeek.id, schedule.dayOfWeek.id, schedule.numOfLesson, schedule.subject.id, schedule.typeOfLesson.id, schedule.classroom.id, schedule.groups[i].id);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static List<Schedule> selectSchedules(Term term)
        {
            List<Schedule> schedules = new List<Schedule>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT Schedule.id, Schedule.idTerm, TypeOfWeek.id, TypeOfWeek.name, DayOfWeek.id, DayOfWeek.name, " +
                "Schedule.numOfLesson, Subject.id, Subject.name, TypeOfLesson.id, TypeOfLesson.name, Classroom.id, Classroom.name, _Group.id, _Group.name " +
                "FROM (((((Schedule INNER JOIN TypeOfWeek ON Schedule.idTypeOfWeek = TypeOfWeek.id) " +
                "INNER JOIN DayOfWeek ON Schedule.idDayOfWeek = DayOfWeek.id) " +
                "INNER JOIN Subject ON Schedule.idSubject = Subject.id) " +
                "INNER JOIN TypeOfLesson ON Schedule.idTypeOfLesson = TypeOfLesson.id) " +
                "INNER JOIN Classroom ON Schedule.idClassroom = Classroom.id) " +
                "INNER JOIN _Group ON Schedule.idGroup = _Group.id " +
                "WHERE Schedule.idTerm = {0};", term.id);
                    SQLiteDataReader reader = command.ExecuteReader();

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
                                s.typeOfWeek.id == typeOfWeek.id &&
                                s.dayOfWeek.id == dayOfWeek.id &&
                                s.numOfLesson == numOfLesson &&
                                s.subject.id == subject.id &&
                                s.typeOfLesson.id == typeOfLesson.id &&
                                s.classroom.id == classroom.id)
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
                }
                connection.Close();
            }
            return schedules;
        }

        public static void ClearSchedule(Term term)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();
                        
                        command.CommandText = String.Format("DELETE FROM Schedule WHERE idTerm={0}", term.id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        //-----Lesson-----

        public static void addLesson(Lesson lesson)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        for (int i = 0; i < lesson.groups.Count(); i++)
                        {
                            command.CommandText = String.Format("INSERT INTO Lesson (idTerm, _date, countOfHours, numOfLesson, idSubject, " +
                            "idTypeOfLesson, idClassroom, idGroup, theme) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}');",
                            lesson.idTerm, calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                            lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void addLessons(List<Lesson> lessons)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Lesson lesson in lessons)
                        {
                            for (int i = 0; i < lesson.groups.Count(); i++)
                            {
                                command.CommandText = String.Format("INSERT INTO Lesson (idTerm, _date, countOfHours, numOfLesson, idSubject, " +
                                    "idTypeOfLesson, idClassroom, idGroup, theme) VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, '{8}');",
                                    lesson.idTerm, calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                                    lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void updateLesson(Lesson lesson)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        for (int i = 0; i < lesson.groups.Count(); i++)
                        {
                            command.CommandText = String.Format("UPDATE Lesson SET _date = {0}, countOfHours={1}, numOfLesson={2}, idSubject={3}, " +
                                "idTypeOfLesson={4}, idClassroom={5}, idGroup={6}, theme='{7}' WHERE id = {8};",
                                calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                                lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme, lesson.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void updateLessons(List<Lesson> lessons)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Lesson lesson in lessons)
                        {
                            for (int i = 0; i < lesson.groups.Count(); i++)
                            {
                                command.CommandText = String.Format("UPDATE Lesson SET _date = {0}, countOfHours={1}, numOfLesson={2}, idSubject={3}, " +
                                    "idTypeOfLesson={4}, idClassroom={5}, idGroup={6}, theme='{7}' WHERE id = {8};",
                                    calculateDays(lesson.date), lesson.countOfHours, lesson.numOfLesson, lesson.subject.id,
                                    lesson.typeOfLesson.id, lesson.classroom.id, lesson.groups[i].id, lesson.theme, lesson.id);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static void deleteLesson(Lesson lesson)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("DELETE FROM Lesson WHERE idTerm = {0} AND _date = {1} " +
                        "AND countOfHours = {2} AND numOfLesson = {3} AND idClassroom = {4} " +
                        "AND idSubject = {5} AND idTypeOfLesson = {6};", lesson.idTerm, calculateDays(lesson.date), 
                        lesson.countOfHours, lesson.numOfLesson, lesson.classroom.id, lesson.subject.id, lesson.typeOfLesson.id);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        public static void deleteLessons(List<Lesson> lessons)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        foreach (Lesson lesson in lessons)
                        {
                            command.CommandText = String.Format("DELETE FROM Lesson WHERE idTerm = {0} AND _date = {1} " +
                            "AND countOfHours = {2} AND numOfLesson = {3} AND idClassroom = {4} " +
                            "AND idSubject = {5} AND idTypeOfLesson = {6};", lesson.idTerm, calculateDays(lesson.date),
                            lesson.countOfHours, lesson.numOfLesson, lesson.classroom.id, lesson.subject.id, lesson.typeOfLesson.id);
                            command.ExecuteNonQuery();
                        }
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        public static List<Lesson> selectLessons(Term term, DateTime startDate, DateTime endDate)
        {
            List<Lesson> lessons = new List<Lesson>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT Lesson.id, Lesson._date, Lesson.countOfHours, Lesson.numOfLesson, " +
                "Subject.id, Subject.name, TypeOfLesson.id, TypeOfLesson.name, Classroom.id, Classroom.name, _Group.id, _Group.name, Lesson.theme " +
                "FROM (((Lesson INNER JOIN Subject ON Lesson.idSubject = Subject.id) " +
                "INNER JOIN TypeOfLesson ON Lesson.idTypeOfLesson = TypeOfLesson.id) " +
                "INNER JOIN Classroom ON Lesson.idClassroom = Classroom.id) " +
                "INNER JOIN _Group ON Lesson.idGroup = _Group.id " +
                "WHERE Lesson.idTerm = {0} AND Lesson._date >= {1} AND Lesson._date <= {2} ORDER BY Lesson._date;", term.id, calculateDays(startDate), calculateDays(endDate));

                    SQLiteDataReader reader = command.ExecuteReader();

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
                                l.subject.id == subject.id &&
                                l.typeOfLesson.id == typeOfLesson.id &&
                                l.classroom.id == classroom.id &&
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
                }
                connection.Close();
            }
            return lessons;
        }

        public static void ClearLesson(Term term)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteTransaction transaction = connection.BeginTransaction())
                {
                    using (SQLiteCommand command = connection.CreateCommand())
                    {
                        command.Transaction = transaction;
                        command.CommandText = pragmaKeyON;
                        command.ExecuteNonQuery();

                        command.CommandText = String.Format("DELETE FROM Lesson WHERE idTerm={0}", term.id);
                        command.ExecuteNonQuery();
                    }
                    transaction.Commit();
                }
                connection.Close();
            }
        }

        //-----------DayOfWeek----------
        public static List<model.DayOfWeek> selectDaysOfWeek()
        {
            List<model.DayOfWeek> daysOfWeek = new List<model.DayOfWeek>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM DayOfWeek");

                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        daysOfWeek.Add(new model.DayOfWeek(record.GetInt64(0),
                            record.GetString(1)));
                    }
                }
                connection.Close();
            }
            return daysOfWeek;
        }
        //-----------TypeOfWeek----------
        public static List<TypeOfWeek> selectTypesOfWeek()
        {
            List<TypeOfWeek> typesOfWeek = new List<TypeOfWeek>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM TypeOfWeek");

                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        typesOfWeek.Add(new TypeOfWeek(record.GetInt64(0),
                            record.GetString(1)));
                    }
                }
                connection.Close();
            }
            return typesOfWeek;
        }
        //-----------TypeOfLesson----------
        public static List<TypeOfLesson> selectTypesOfLesson()
        {
            List<TypeOfLesson> typesOfLesson = new List<TypeOfLesson>();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM TypeOfLesson");

                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        typesOfLesson.Add(new TypeOfLesson(record.GetInt64(0),
                            record.GetString(1)));
                    }
                }
                connection.Close();
            }
            return typesOfLesson;
        }



        //Teacher
        public static Teacher getTeacher()
        {
            Teacher teacher = new model.Teacher();
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = String.Format("SELECT * FROM Teacher");

                    SQLiteDataReader reader = command.ExecuteReader();

                    foreach (DbDataRecord record in reader)
                    {
                        teacher = new Teacher(record.GetString(1), record.GetString(2), record.GetString(3),
                            record.GetString(4), record.GetString(5), record.GetString(6), record.GetString(7));
                    }
                }
                connection.Close();
            }
            return teacher;
        }

        public static void updateTeacher(Teacher teacher)
        {
            using (SQLiteConnection connection = new SQLiteConnection(String.Format("Data Source={0};", dbName)))
            {
                connection.Open();
                using (SQLiteCommand command = connection.CreateCommand())
                {
                    command.CommandText = pragmaKeyON;
                    command.ExecuteNonQuery();

                    command.CommandText = String.Format("UPDATE Teacher SET institute = '{0}', faculty = '{1}', department = '{2}', " +
                        "fullName = '{3}', headFullName = '{4}', academicRank = '{5}', post = '{6}';", teacher.institute, teacher.faculty, teacher.department,
                        teacher.fullName, teacher.headFullName, teacher.academicRank, teacher.post);
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
        

        //считает количество дней с 01.01.1970 до заданной даты
        public static int calculateDays(DateTime date)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);// 1970/1/1 00:00:00 
            TimeSpan result = date.Subtract(dt);
            return Convert.ToInt32(result.TotalDays);
        }

        //преводчит количество дней с 01.01.1970 до заданной даты в DateTime
        public static DateTime intToDateTime(int days)
        {
            DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);// 1970/1/1 00:00:00 
            return dt.AddDays(days);
        }
        
        public static void AddVocabularyItem(VocabularyEntity obj)
        {
            if (obj.GetType().Name == "Subject")
            {
                addSubject(obj as Subject);
            }
            else if (obj.GetType().Name == "Group")
            {
                addGroup(obj as Group);
            }
            else if (obj.GetType().Name == "Classroom")
            {
                addClassroom(obj as Classroom);
            }
        }

        public static void UpdateVocabularyItem(VocabularyEntity obj)
        {
            if (obj.GetType().Name == "Subject")
            {
                updateSubject(obj as Subject);
            }
            else if (obj.GetType().Name == "Group")
            {
                updateGroup(obj as Group);
            }
            else if (obj.GetType().Name == "Classroom")
            {
                updateClassroom(obj as Classroom);
            }
        }

        public static void DeleteVocabularyItem(VocabularyEntity obj)
        {
            if (obj.GetType().Name == "Subject")
            {
                deleteSubject(obj as Subject);
            }
            else if (obj.GetType().Name == "Group")
            {
                deleteGroup(obj as Group);
            }
            else if (obj.GetType().Name == "Classroom")
            {
                deleteClassroom(obj as Classroom);
            }
        }

        public static Schedule AddRandomSchedule(Term term)
        {
           // List<Term> term = new List<Term>(DBHelper.selectTerms());
            List<Group> groups = new List<Group>(DBHelper.selectGroups(term));
            List<TypeOfWeek> typeOfWeek = new List<TypeOfWeek>(DBHelper.selectTypesOfWeek());
            List<model.DayOfWeek> dayOfWeek = new List<model.DayOfWeek>(DBHelper.selectDaysOfWeek());
            List<TypeOfLesson> typeOfLesson = new List<TypeOfLesson>(DBHelper.selectTypesOfLesson());
            List<Subject> subject = new List<Subject>(DBHelper.selectSubject(term));
            List<Classroom> classroom = new List<Classroom>(DBHelper.selectClassroom(term));

            if (groups.Count != 0)
            {
                Random rnd = new Random();
                int rndSize = rnd.Next(1, groups.Count + 1); // ранд. значение размера списка групп
                List<Group> groupsTemp = new List<Group>();
                for (int i = 0; i <= rndSize - 1; i++)
                {
                    groupsTemp.Add(EnumerableExtension.PickRandom(groups)); // добавляем ранд. айтем с groups в groupsTemp
                }

                groupsTemp = groupsTemp.GroupBy(x => x.id).Select(x => x.First()).ToList(); // убираем дубликаты

                Schedule schedule = new Schedule(1, term.id, // ранд. семестр 
                                                    EnumerableExtension.PickRandom(typeOfWeek), // ...
                                                    EnumerableExtension.PickRandom(dayOfWeek),
                                                    rnd.Next(1, 6),
                                                    EnumerableExtension.PickRandom(subject),
                                                    EnumerableExtension.PickRandom(typeOfLesson),
                                                    EnumerableExtension.PickRandom(classroom), groupsTemp);

                try
                {
                    //DBHelper.addSchedule(schedule);
                    return schedule;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception cought", ex);
                }
            }
            return null;
        }
    }
}
