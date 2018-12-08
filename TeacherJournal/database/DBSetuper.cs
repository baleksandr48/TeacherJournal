using Microsoft.Win32;
using System;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using System.Windows;


namespace TeacherJournal.database
{
    static class DBSetuper
    {
        static private String dbName;
        static private SQLiteConnection connection;

        static public void setup()
        {
            // Файл БД лежит в текущей директории.
            dbName = Path.Combine(Environment.CurrentDirectory, "../../database/journal.db");

            // Если БД существует, то действий не требуется.
            if (File.Exists(dbName))
                return;

            /* Даем выбор пользователю - или создать новый файл, или указать существующий. 
            MessageBoxResult result = CustomMessageBox.ShowYesNo("Не знайдено файл бази даних journal.db." +
                                                                "\nВкажіть шлях до файлу journal.db або створіть новий файл.",
                                                                "Попередження",
                                                                "Вказати шлях",
                                                                "Створити файл");
            // Если создать файл.
            if (result == MessageBoxResult.No)
            {
                createDB();
            }
            // Если указать путь к базе данных.
            else if (result == MessageBoxResult.Yes)
            {
                OpenFileDialog fileDialog = new OpenFileDialog();
                fileDialog.DefaultExt = "db";
                fileDialog.AddExtension = true;

                // Если указали файл, то копируем его в папку database.
                if (fileDialog.ShowDialog() == true)
                {
                    var fileName = fileDialog.FileName;
                    File.Copy(fileName, Path.Combine(Environment.CurrentDirectory, "../../database/journal.db"));
                    return;
                }
                // Иначе - останавливаем выполнение программы.
                else
                {
                    Environment.Exit(0);
                    return;
                }

            }
            else return;
            */

            connection = new SQLiteConnection(String.Format("Data Source={0};", dbName));
            connection.Open();

            createTables();
            fillVocabularies();
            connection.Close();
        }

        //Создаем файл базы данных
        static private void createDB()
        {
            SQLiteConnection.CreateFile(dbName);
        }

        //Создаем все таблицы
        static private void createTables()
        {
            String term = "CREATE TABLE Term (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "name TEXT NOT NULL, " +
                "beginDate INTEGER NOT NULL, " +
                "endDate INTEGER NOT NULL," +
                "startFromNumerator INTEGER NOT NULL);";
            String group = "CREATE TABLE _Group (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "idTerm INTEGER NOT NULL," +
                "name TEXT NOT NULL," +
                "FOREIGN KEY(idTerm) REFERENCES Term(id) ON DELETE CASCADE);";
            String subject = "CREATE TABLE Subject (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "idTerm INTEGER NOT NULL," +
                "name TEXT NOT NULL," +
                "FOREIGN KEY(idTerm) REFERENCES Term(id) ON DELETE CASCADE);";
            String classroom = "CREATE TABLE Classroom (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "idTerm INTEGER NOT NULL," +
                "name TEXT NOT NULL," +
                "FOREIGN KEY(idTerm) REFERENCES Term(id) ON DELETE CASCADE);";
            String dayOfWeek = "CREATE TABLE DayOfWeek (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "name TEXT NOT NULL);";
            String typeOfLesson = "CREATE TABLE TypeOfLesson (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "name TEXT NOT NULL);";
            String typeOfWeek = "CREATE TABLE TypeOfWeek (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "name TEXT NOT NULL);";
            String schedule = "CREATE TABLE Schedule (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "idTerm INTEGER NOT NULL," +
                "idTypeOfWeek INTEGER NOT NULL," +
                "idDayOfWeek INTEGER NOT NULL," +
                "numOfLesson INTEGER NOT NULL," +
                "idSubject INTEGER NOT NULL," +
                "idTypeOfLesson INTEGER NOT NULL," +
                "idClassroom INTEGER NOT NULL," +
                "idGroup INTEGER NOT NULL," +
                "FOREIGN KEY(idDayOfWeek) REFERENCES DayOfWeek(id)," +
                "FOREIGN KEY(idTypeOfWeek) REFERENCES TypeOfWeek(id)," +
                "FOREIGN KEY(idTerm) REFERENCES Term(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idTypeOfLesson) REFERENCES TypeOfLesson(id)," +
                "FOREIGN KEY(idSubject) REFERENCES Subject(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idGroup) REFERENCES _Group(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idClassroom) REFERENCES Classroom(id) ON DELETE CASCADE);";
            String lesson = "CREATE TABLE Lesson (id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "_date INTEGER NOT NULL," +
                "countOfHours INTEGER NOT NULL," +
                "numOfLesson INTEGER NOT NULL," +
                "idClassroom INTEGER NOT NULL," +
                "idTerm INTEGER NOT NULL," +
                "idSubject INTEGER NOT NULL," +
                "idGroup INTEGER NOT NULL," +
                "idTypeOfLesson INTEGER NOT NULL," +
                "theme TEXT," +
                "FOREIGN KEY(idTerm) REFERENCES Term(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idClassroom) REFERENCES Classroom(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idSubject) REFERENCES Subject(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idGroup) REFERENCES _Group(id) ON DELETE CASCADE," +
                "FOREIGN KEY(idTypeOfLesson) REFERENCES TypeOfLesson(id));";
            String teacher = "CREATE TABLE Teacher(id INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE," +
                "institute TEXT," +
                "faculty TEXT," +
                "department TEXT," +
                "fullName TEXT," +
                "academicRank TEXT," +
                "post TEXT);";

            new SQLiteCommand(group, connection).ExecuteNonQuery();
            new SQLiteCommand(subject, connection).ExecuteNonQuery();
            new SQLiteCommand(term, connection).ExecuteNonQuery();
            new SQLiteCommand(classroom, connection).ExecuteNonQuery();
            new SQLiteCommand(dayOfWeek, connection).ExecuteNonQuery();
            new SQLiteCommand(typeOfLesson, connection).ExecuteNonQuery();
            new SQLiteCommand(typeOfWeek, connection).ExecuteNonQuery();
            new SQLiteCommand(schedule, connection).ExecuteNonQuery();
            new SQLiteCommand(lesson, connection).ExecuteNonQuery();
            new SQLiteCommand(teacher, connection).ExecuteNonQuery();
        }

        static private void fillVocabularies()
        {
            //Заполняем словарь дней недели
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Понеділок')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Вівторок')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Середа')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Четверг')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('П’ятниця')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Субота')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO DayOfWeek (name) VALUES ('Неділя')", connection).ExecuteNonQuery();

            //Заполняем словарь типов недель
            new SQLiteCommand("INSERT INTO TypeOfWeek (name) VALUES ('Чисельник')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfWeek (name) VALUES ('Знаменник')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfWeek (name) VALUES ('Щотижня')", connection).ExecuteNonQuery();

            //Заполняем словарь видов занятий
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Лекція')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Лабораторне заняття ')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Практичне заняття')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Семінарське заняття')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Індивідуальне заняття')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Консультація')", connection).ExecuteNonQuery();
            new SQLiteCommand("INSERT INTO TypeOfLesson (name) VALUES ('Екзамінаційна консультація')", connection).ExecuteNonQuery();

            //Создаем запись с информацией преподавателя
            new SQLiteCommand("INSERT INTO Teacher (institute, faculty, department, fullName, academicRank, post) VALUES (' ', ' ', ' ', ' ', ' ', ' ')", connection).ExecuteNonQuery();
        }
    }
}
