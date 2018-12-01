using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Word = Microsoft.Office.Interop.Word;
using System.Data;
using TeacherJournal.model;
using TeacherJournal.database;

namespace TeacherJournal
{
    class Export
    {
        private String[] ColumnsNameOfSchedule = { "Пара", "Номер тижня", "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Суббота" };
        private String[] RowsNumberOfSchedule = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII" };
        private String[] ColumnsNameOfLessons = { "Дата","Шифр потоку (академічної групи)","Назва навчальної дисципліни", "Тема заняття","Вид заняття", "К-ть годин"};
        private String[] TypeOfLesson = { "Лекція", "Практичне заняття", "Лабораторне заняття ", "Семінарське заняття", "Індивідуальне заняття", "Консультація", "Екзамінаційна консультація" };
        private String[] Month = { "Вересень", "Жовтень", "Листопад", "Грудень", "Січень", "Лютий", "Березень", "Квітень", "Травень", "Червень", "Липень", "Серпень", "Вересень", "Жовтень", "Листопад", "Грудень" };
        private String[] nameColumns = { "Місяць", "Читання лекцій", "Проведення практичних занять", "Проведення лабораторних занять", "Проведення семінарських занять", "Проведення індивідуальних занять", "Проведення консультацій протягом семестра", "Проведення екзаменаційних консультацій", "Перевірка контрольних (модульних) робіт, що виконуються під час самостійної роботи", "Рефератів, аналітичних оглядів, перекладів", "Розрахункових, графічних, розрахунково-графічних робіт", "Курсових проектів, робіт", "Проведення заліку", "Проведення семестрових екзаменів", "Керівництво навчальною і виробничою практикою", "Проведення атестації", "Керівництво, консуль - тування, рецензування та проведення захисту дипломних проектів(робіт)", "Керівництво аспірантами, здобувачами та стажуванням викладачів", "Усього" };


        private String zavkaf;
        private String prepod;

        private int[] sumMonth = new int[2];
        // Количество столбцов.
        private int tableColumns;
        // Общее количество строк в таблице.
        private int tableRows;
        // Предустановленная закладка конца файла.
        private object oEndOfDoc = "\\endofdoc";
        // Range для создания таблицы.
        private Word.Range rangeEndOfFile;
        // Параграф для для создания пустой строки после таблицы.
        private Word.Paragraph objParagraph;
        // Параграф с текстом документа
        //private Word.Paragraph[] wordparagraphs;
        private object objRangePara;
        // Создание документа.
        private Word.Application WordApp;
        private Word.Document adoc;
        // Количество пар, всегда не больше 8.
        private int maxLessons = 8;
        private int DaysOfWeek = 6;
        public Export()
        {
            WordApp = new Word.Application();
            adoc = WordApp.Documents.Add();
            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            prepod = "Каплієнко Т.І.";
            zavkaf = "Субботін С.О.";

            Word.Application wrdApplication = new Word.Application();
            adoc.PageSetup.TopMargin = wrdApplication.InchesToPoints(0.5f);
            adoc.PageSetup.BottomMargin = wrdApplication.InchesToPoints(0.5f);
            adoc.PageSetup.LeftMargin = wrdApplication.InchesToPoints(0.5f);
            adoc.PageSetup.RightMargin = wrdApplication.InchesToPoints(0.5f);
            //wordparagraphs = new Word.Paragraph[10];
        }
        private void SetStyleOfDoc(Word.Table table)
        {
            //Настройка стиля
            table.Range.Font.Size = 10;
            table.Range.Font.Name = "Times New Roman";
            table.Range.ParagraphFormat.SpaceAfter = 0;
            table.Range.ParagraphFormat.SpaceBefore = 0;

            //Создает контур
            table.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            table.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
        }
        private void InsertText(String Text,int bold, int size, Word.WdUnderline underline, Word.WdParagraphAlignment alignment, bool Enter)
        {
            Word.Paragraph Para1;
            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Para1 = adoc.Content.Paragraphs.Add(ref objRangePara);
            Para1.Range.Text = Text;
            Para1.Range.Bold = bold;
            Para1.Range.Font.Size = size;
            Para1.Range.Font.Underline = underline;
            Para1.Alignment = alignment;

            if(Enter)
            {
                rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                Word.Table tableLessons = adoc.Tables.Add(rangeEndOfFile, 1, 1);
            }
        }
        private void InsertFieldWithInitials(String HeadofDepartment, String Teacher,String Orientation)
        {

            Word.Paragraph Para1;
            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Para1 = adoc.Content.Paragraphs.Add(ref objRangePara);
            if (Orientation == "Album")
            {
                Para1.Range.Text = "Завідувач кафедри                                                                                                                                 ___" + HeadofDepartment + "___________";
            }
            else
            {
                Para1.Range.Text = "Завідувач кафедри                                                                 ___" + HeadofDepartment + "___________";
            }
            
            Para1.Range.Bold = 0;
            Para1.Range.Font.Size = 14;
            Para1.Range.Font.Underline = 0;
            Para1.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Word.Table tableLessons = adoc.Tables.Add(rangeEndOfFile, 1, 1);

            Word.Paragraph Para2;
            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Para2 = adoc.Content.Paragraphs.Add(ref objRangePara);

            if (Orientation == "Album")
            {
                Para2.Range.Text = "Викладач                                                                                                                                                ___" + Teacher + "___________";
            }
            else Para2.Range.Text = "Викладач                                                                                ___" + Teacher + "___________";
            Para2.Range.Bold = 0;
            Para2.Range.Font.Size = 14;
            Para2.Range.Font.Underline = 0;
            Para2.Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;
        }
        private void fillCellOfReport(List<Lesson> lessons, Word.Table table, int month, String typeOfLesson, int column, int cell)
        {
            int countOfHours = 0;
            foreach (Lesson lsn1 in lessons)
            {
                if (Convert.ToInt32(lsn1.date.Month) == month)
                {
                    if (lsn1.typeOfLesson.name == typeOfLesson)
                    {
                        countOfHours += lsn1.countOfHours;
                        table.Columns[column].Cells[cell].Range.Delete();
                        table.Columns[column].Cells[cell].Range.InsertAfter(Convert.ToString(countOfHours));
                    }
                }
            }
        }
        private void fillRowsOfReport(List<Lesson> lessons, Word.Table table, int month, String nameMonth, int rows)
        {
           
            table.Columns[1].Cells[rows].Range.InsertAfter(nameMonth);
            for (int i = 0; i < TypeOfLesson.Length; i++)
            {
                fillCellOfReport(lessons, table, month, TypeOfLesson[i], i+2, rows);
            }
            amountHoursInMonth(lessons, table, month, 19, rows);

            if(month>=9 && month<=12)
            {
                sumMonth[0]+= amountHoursInMonth(lessons, table, month, 19, rows);
            }
            if(month >= 1 && month <= 8)
            {
                sumMonth[1] += amountHoursInMonth(lessons, table, month, 19, rows);
            }
            
        }
        private int amountHoursInMonth(List<Lesson> lessons, Word.Table table, int month, int column, int cell)
        {
            int countOfHours = 0;
            foreach (Lesson lsn1 in lessons)
            {
                if (Convert.ToInt32(lsn1.date.Month) == month)
                {
                        countOfHours += lsn1.countOfHours;
                        table.Columns[column].Cells[cell].Range.Delete();
                        table.Columns[column].Cells[cell].Range.InsertAfter(Convert.ToString(countOfHours));
                }
            }
            return countOfHours;
        }
        private int amountOfHoursInTerm(List<Lesson> lessons, Word.Table table, String typeOfLesson, int column, int cell)
        {
            int countOfHours = 0;
            foreach (Lesson lsn1 in lessons)
            {
                if (lsn1.typeOfLesson.name == typeOfLesson)
                {
                    countOfHours += lsn1.countOfHours;
                    table.Columns[column].Cells[cell].Range.Delete();
                    table.Columns[column].Cells[cell].Range.InsertAfter(Convert.ToString(countOfHours));
                }
            }
            return countOfHours;
        }
        private void SetStyleOfTable(Word.Table table, int column, int cell, int bold, int size, Word.WdParagraphAlignment alignH)
        {
            //table.Columns[column].Cells[cell].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            //table.Columns[column].Cells[cell].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            //table.Columns[column].Cells[cell].Range.Font.Size = 11;
            table.Cell(cell,column).Range.ParagraphFormat.Alignment = alignH;
            table.Cell(cell, column).VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
            table.Cell(cell, column).Range.Font.Size = size;
            table.Cell(cell, column).Range.Font.Bold = bold;
        }
        private void CreateLessonsTable(Term term)
        {
            //Лекции
            List<Lesson> lessons = DBHelper.selectLessons(term, term.beginDate, term.endDate);
            tableRows = lessons.Count + 1;
            tableColumns = 6;
            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Word.Table tableLessons = adoc.Tables.Add(rangeEndOfFile, tableRows, tableColumns);

            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            objParagraph = adoc.Content.Paragraphs.Add(ref objRangePara);
            objParagraph.Range.Font.Underline = 0;
            objParagraph.Range.Text = Environment.NewLine;

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

            SetStyleOfDoc(tableLessons);
            // Создаём шапку.
            for (int i = 0; i < tableColumns; i++)
            {
                tableLessons.Rows[1].Cells[i + 1].Range.InsertAfter(ColumnsNameOfLessons[i]);
                SetStyleOfTable(tableLessons, i + 1, 1,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }

            // Заполнение колонки с датой.
            for (int i = 0; i < lessons.Count; i++)
            {
                tableLessons.Rows[i + 2].Cells[1].Range.InsertAfter(Convert.ToString(lessons[i].date.Day));
                tableLessons.Rows[i + 2].Cells[1].Range.InsertAfter(".");
                tableLessons.Rows[i + 2].Cells[1].Range.InsertAfter(Convert.ToString(lessons[i].date.Month));
                SetStyleOfTable(tableLessons, 1, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }

            // Заполнение колонки с номером групп
            for (int i = 0; i < lessons.Count; i++)
            {
                for (int k = 0; k < lessons[i].groups.Count; k++)
                {
                    tableLessons.Rows[i + 2].Cells[2].Range.InsertAfter(lessons[i].groups[k].name);
                    if (k + 1 < lessons[i].groups.Count)
                    {
                        tableLessons.Rows[i + 2].Cells[2].Range.InsertAfter(",");
                    }
                }
                SetStyleOfTable(tableLessons, 2, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }

            // Заполнение колонки с названием дисциплины.
            for (int i = 0; i < lessons.Count; i++)
            {
                tableLessons.Rows[i + 2].Cells[3].Range.InsertAfter(lessons[i].subject.name);
                SetStyleOfTable(tableLessons, 3, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }

            // Заполнение колонки с темой.
            for (int i = 0; i < lessons.Count; i++)
            {
                tableLessons.Rows[i + 2].Cells[4].Range.InsertAfter(lessons[i].theme);
                SetStyleOfTable(tableLessons, 4, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }
            // Заполнение колонки вид занятия.
            for (int i = 0; i < lessons.Count; i++)
            {
                tableLessons.Rows[i + 2].Cells[5].Range.InsertAfter(lessons[i].typeOfLesson.name);
                SetStyleOfTable(tableLessons, 5, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);
            }
            // Заполнение колонки к-во часов.
            for (int i = 0; i < lessons.Count; i++)
            {
                tableLessons.Rows[i + 2].Cells[6].Range.InsertAfter(Convert.ToString(lessons[i].countOfHours));
                SetStyleOfTable(tableLessons, 6, i + 2,0,11, Word.WdParagraphAlignment.wdAlignParagraphCenter);

            }
        }
        private void CreateScheduleTable(Term term,bool Break)
        {
            tableColumns = 8;
            List<Schedule> schedules = DBHelper.selectSchedules(term);
            
            //Количество недель в семестре
            int numOfWeeks = (DBHelper.calculateDays(term.endDate) - DBHelper.calculateDays(term.beginDate)) / 7;
            //Счетчик который показывает в какие дние есть занятия а в какие нет True = есть False = нет
            bool[] DaysIsntNull = new bool[maxLessons];

            foreach (Schedule schedule in schedules)
            {
                for (int i = 0; i < maxLessons; i++)
                {
                    for (int k = 1; k < (maxLessons + 1); k++)
                    {
                        if (schedule.numOfLesson == i + k)
                        {
                            DaysIsntNull[i + k - 1] = true;
                        }
                    }
                }
            }

            Schedule[,] scheduleMatrix = new Schedule[maxLessons, DaysOfWeek];

            foreach (Schedule obj in schedules)
            {
                for (int i = 0; i < maxLessons; i++)
                {
                    for (int k = 0; k < DaysOfWeek; k++)
                    {
                        if (obj.numOfLesson == i + 1 && obj.dayOfWeek.id == k + 1)
                        {
                            scheduleMatrix[i, k] = obj;
                        }
                    }
                }
            }
            
            int maxRows = 0;//Количество строк генерируемой части таблицы

            for (int i = 0; i < maxLessons; i++)
            {
                if (DaysIsntNull[i] == true)
                {
                    maxRows += numOfWeeks;
                }
                else
                {
                    maxRows += 2;
                }
            }

            tableRows = maxRows + 1;
            //Создание таблицы
            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            Word.Table tableSchedule = adoc.Tables.Add(rangeEndOfFile, tableRows, tableColumns);

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            if (Break)
            {
                Word.Paragraph Para4;
                objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                Para4 = adoc.Content.Paragraphs.Add(ref objRangePara);
                Para4.Range.Text = "Примітка. У кожний рядок заноситься вид занять або роботи, шифр потоку або групи, номер аудиторії.";
                Para4.Range.Font.Size = 12;
                Para4.Range.Font.Name = "Times New Roman";
                rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                rangeEndOfFile.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
            }
            else
            {
                objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                objParagraph = adoc.Content.Paragraphs.Add(ref objRangePara);
                objParagraph.Range.Text = Environment.NewLine;
            }

            

            SetStyleOfDoc(tableSchedule);

            //Сливание клеток
            for (int i = 0; i < maxLessons; i++)
            {
                if (DaysIsntNull[i])
                {
                    tableSchedule.Columns[1].Cells[i + 2].Merge(tableSchedule.Columns[1].Cells[i + (numOfWeeks + 1)]);
                }
                else
                {
                    tableSchedule.Columns[1].Cells[i + 2].Merge(tableSchedule.Columns[1].Cells[i + 3]);
                }
            }
            //Основная часть заполнение таблицы
            for (int i = 0, posDays = 3; i < DaysOfWeek; i++, posDays++)
            {
                int begin = 2;
                for (int k = 0; k < maxLessons; k++)
                {
                    if (DaysIsntNull[k])
                    {
                        if (scheduleMatrix[k, i] != null)
                        {
                            if (scheduleMatrix[k, i].dayOfWeek.id == i + 1)
                            {
                                tableSchedule.Columns[posDays].Cells[begin].Merge(tableSchedule.Columns[posDays].Cells[begin + numOfWeeks - 1]);
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k, i].subject.name);
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(", ");
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k, i].typeOfLesson.name);
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(", ");

                                for (int n = 0; n < scheduleMatrix[k, i].groups.Count; n++)
                                {
                                    tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k, i].groups[n].name);
                                    tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(", ");
                                }
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter("ауд. ");

                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k, i].classroom.name);
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(", ");
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k, i].typeOfWeek.name);
                                tableSchedule.Columns[posDays].Cells[begin].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                                tableSchedule.Columns[posDays].Cells[begin].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                                tableSchedule.Columns[posDays].Cells[begin].Range.Font.Size = 11;
                                begin += 1;

                            }
                        }
                        else
                        {
                            begin += numOfWeeks;
                        }
                    }
                    else
                    {
                        begin += 2;
                    }
                }
            }


            //Шапка таблицы
            for (int i = 0; i < tableColumns; i++)
            {
                tableSchedule.Cell(1, 1 + i).Range.InsertAfter(ColumnsNameOfSchedule[i]);
                tableSchedule.Columns[1 + i].Cells[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                tableSchedule.Columns[1 + i].Cells[1].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tableSchedule.Columns[1 + i].Cells[1].Range.Font.Bold = 1;
            }

            //Заполнение клеток
            for (int i = 0; i < tableSchedule.Columns[1].Cells.Count - 1; i++)
            {
                tableSchedule.Columns[1].Cells[2 + i].Range.InsertAfter(RowsNumberOfSchedule[i]);
                tableSchedule.Columns[1].Cells[2 + i].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                tableSchedule.Columns[1].Cells[2 + i].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tableSchedule.Columns[1].Cells[2 + i].Range.Font.Bold = 1;
                tableSchedule.Columns[1].Cells[2 + i].Range.Font.Size = 14;
                //tableSchedule.Rows[i + 1].Cells[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            }

            //Нумеруем 2 колонку
            for (int i = 0, position = 1; i < maxLessons; i++)
            {

                if (DaysIsntNull[i] == true)
                {
                    for (int k = 0; k < numOfWeeks; k++, position++)
                    {
                        if (position <= maxRows)
                        {
                            tableSchedule.Columns[2].Cells[position + 1].Range.InsertAfter(Convert.ToString(k + 1));
                            tableSchedule.Columns[2].Cells[position + 1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            tableSchedule.Columns[2].Cells[position + 1].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            tableSchedule.Columns[2].Cells[position + 1].Range.Font.Size = 10;
                        }
                    }
                }
                else
                {
                    for (int k = 0; k < 2; k++, position++)
                    {
                        if (position <= maxRows)
                        {
                            tableSchedule.Columns[2].Cells[position + 1].Range.InsertAfter(Convert.ToString(k + 1));
                            tableSchedule.Columns[2].Cells[position + 1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                            tableSchedule.Columns[2].Cells[position + 1].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                            tableSchedule.Columns[2].Cells[position + 1].Range.Font.Size = 10;
                        }
                    }
                }
            }
        }
        private void Titulka(String institut, String fakultet, String DateBegin, String DateEnd, String kafedra, String FIO,String zvanya, String posada)
        {
            Word.Paragraphs wordparagraphs = adoc.Paragraphs;
            
            adoc.Paragraphs.Add();
            wordparagraphs[1].Range.Text = "Форма № 12";
            wordparagraphs[1].Range.Bold = 0;
            wordparagraphs[1].Range.Font.Size = 8;
            wordparagraphs[1].Range.Font.Name = "Times New Roman";
            wordparagraphs[1].Range.Font.Underline = 0;
            wordparagraphs[1].Alignment = Word.WdParagraphAlignment.wdAlignParagraphRight;
            adoc.Paragraphs.Add();

            adoc.Paragraphs.Add();
            wordparagraphs[2].Range.Text = "ЗАПОРІЗЬКИЙ НАЦІОНАЛЬНИЙ ТЕХНІЧНИЙ УНІВЕРСИТЕТ";
            wordparagraphs[2].Range.Bold = 1;
            wordparagraphs[2].Range.Font.Size = 14;
            wordparagraphs[2].Range.Font.Name = "Times New Roman";
            wordparagraphs[2].Range.Font.Underline = 0;
            wordparagraphs[2].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            adoc.Paragraphs.Add();
            wordparagraphs[3].Range.Text = "Інститут, факультет: "+institut+","+fakultet;
            wordparagraphs[3].Range.Bold = 1;
            wordparagraphs[3].Range.Font.Size = (float)12.5;
            wordparagraphs[3].Range.Font.Name = "Times New Roman";
            wordparagraphs[3].Range.Font.Underline = 0;
            wordparagraphs[3].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();

            adoc.Paragraphs.Add();
            wordparagraphs[9].Range.Text = "ЖУРНАЛ ОБЛІКУ РОБОТИ ВИКЛАДАЧА";
            wordparagraphs[9].Range.Bold = 0;
            wordparagraphs[9].Range.Font.Name = "Times New Roman";
            wordparagraphs[9].Range.Font.Size = 34;
            wordparagraphs[9].Range.Font.Underline = 0;
            wordparagraphs[9].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            adoc.Paragraphs.Add();
            wordparagraphs[10].Range.Text = "на "+ DateBegin +" - "+ DateEnd +" навчальний рік";
            wordparagraphs[10].Range.Bold = 0;
            wordparagraphs[10].Range.Font.Name = "Times New Roman";
            wordparagraphs[10].Range.Font.Size = (float)24.5;
            wordparagraphs[10].Range.Font.Underline = 0;
            wordparagraphs[10].Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();
            adoc.Paragraphs.Add();

            adoc.Paragraphs.Add();
            wordparagraphs[16].Range.Text = "1. Кафедра__________________________" + kafedra+"";
            wordparagraphs[16].Range.Bold = 0;
            wordparagraphs[16].Range.Font.Name = "Times New Roman";
            wordparagraphs[16].Range.Font.Size = 18;
            wordparagraphs[16].Range.Font.Underline = 0;
            wordparagraphs[16].Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            adoc.Paragraphs.Add();
            wordparagraphs[17].Range.Text = "2. Прізвище, ім'я, по батькові__________" + FIO;
            wordparagraphs[17].Range.Bold = 0;
            wordparagraphs[17].Range.Font.Name = "Times New Roman";
            wordparagraphs[17].Range.Font.Size = 18;
            wordparagraphs[17].Range.Font.Underline = 0;
            wordparagraphs[17].Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            adoc.Paragraphs.Add();
            wordparagraphs[18].Range.Text = "3. Вчене звання, науковий ступінь______" + zvanya;
            wordparagraphs[18].Range.Bold = 0;
            wordparagraphs[18].Range.Font.Name = "Times New Roman";
            wordparagraphs[18].Range.Font.Size = 18;
            wordparagraphs[18].Range.Font.Underline = 0;
            wordparagraphs[18].Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;

            adoc.Paragraphs.Add();
            wordparagraphs[19].Range.Text = "4. Посада___________________________" + posada;
            wordparagraphs[19].Range.Bold = 0;
            wordparagraphs[19].Range.Font.Name = "Times New Roman";
            wordparagraphs[19].Range.Font.Size = 18;
            wordparagraphs[19].Range.Font.Underline = 0;
            wordparagraphs[19].Alignment = Word.WdParagraphAlignment.wdAlignParagraphLeft;


            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rangeEndOfFile.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);
        }
        private void CreateReportTable(Term term1, Term term2)
        {

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rangeEndOfFile.PageSetup.Orientation = Word.WdOrientation.wdOrientLandscape;

            Object defaultTableBehavior = Word.WdDefaultTableBehavior.wdWord9TableBehavior;
            Object autoFitBehavior = Word.WdAutoFitBehavior.wdAutoFitWindow;

            List<Lesson> lessons1Term = DBHelper.selectLessons(term1, term1.beginDate, term1.endDate);
            List<Lesson> lessons2Term = DBHelper.selectLessons(term2, term2.beginDate, term2.endDate);
            tableRows =  18;
            tableColumns = 19;

            Word.Table tableOfReport = adoc.Tables.Add(rangeEndOfFile, tableRows, tableColumns, ref defaultTableBehavior, ref autoFitBehavior);

            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            objParagraph = adoc.Content.Paragraphs.Add(ref objRangePara);
            objParagraph.Range.Text = Environment.NewLine;

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            SetStyleOfDoc(tableOfReport);


            fillRowsOfReport(lessons1Term, tableOfReport, 1, "Січень", 8);
            fillRowsOfReport(lessons2Term, tableOfReport, 2, "Лютий", 10);
            fillRowsOfReport(lessons2Term, tableOfReport, 3, "Березень", 11);
            fillRowsOfReport(lessons2Term, tableOfReport, 4, "Квітень", 12);
            fillRowsOfReport(lessons2Term, tableOfReport, 5, "Травень", 13);
            fillRowsOfReport(lessons2Term, tableOfReport, 6, "Червень", 14);
            fillRowsOfReport(lessons2Term, tableOfReport, 7, "Липень", 15);
            fillRowsOfReport(lessons2Term, tableOfReport, 8, "Серпень", 16);
            fillRowsOfReport(lessons1Term, tableOfReport, 9, "Вересень", 4);
            fillRowsOfReport(lessons1Term, tableOfReport, 10, "Жовтень", 5);
            fillRowsOfReport(lessons1Term, tableOfReport, 11, "Листопад", 6);
            fillRowsOfReport(lessons1Term, tableOfReport, 12, "Грудень", 7);

            int[] sum = new int[TypeOfLesson.Length];
            //tableOfReport.Columns.AutoFit();

           
            for (int i = 0; i < TypeOfLesson.Length; i++)
            {
                amountOfHoursInTerm(lessons1Term, tableOfReport, TypeOfLesson[i], i + 2, 9);
                amountOfHoursInTerm(lessons2Term, tableOfReport, TypeOfLesson[i], i + 2, 17);
                tableOfReport.Columns[i + 2].Cells[18].Range.InsertAfter(Convert.ToString(amountOfHoursInTerm(lessons1Term, tableOfReport, TypeOfLesson[i], i + 2, 9) + amountOfHoursInTerm(lessons2Term, tableOfReport, TypeOfLesson[i], i + 2, 17)));

            }
            for(int i=1;i<=3;i++)
                for(int k=1;k<=tableColumns;k++)
                {
                    if (i == 1 || k==1 && i==1|| k==2 && i==1 || k==3 && i == 1 || i==2 && k==10 || i == 2 && k == 11 || i == 2 && k == 12)
                    {
                        SetStyleOfTable(tableOfReport, k, i, 1, 10, Word.WdParagraphAlignment.wdAlignParagraphCenter);
                    }
                    else
                    {
                        SetStyleOfTable(tableOfReport, k, i, 1, 10, Word.WdParagraphAlignment.wdAlignParagraphLeft);
                    }
                }
            //tableOfReport.Columns[1].Cells[9].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
            tableOfReport.Columns[1].Cells[9].Range.InsertAfter("Разом за семестр I");
            tableOfReport.Columns[19].Cells[9].Range.InsertAfter(Convert.ToString(sumMonth[0]));
            tableOfReport.Columns[1].Cells[17].Range.InsertAfter("Разом за семестр II");
            tableOfReport.Columns[1].Cells[18].Range.InsertAfter("Всього за навчальний рік");
            tableOfReport.Columns[19].Cells[17].Range.InsertAfter(Convert.ToString(sumMonth[1]));
            tableOfReport.Columns[19].Cells[18].Range.InsertAfter(Convert.ToString(sumMonth[0] + sumMonth[1]));
            for(int i=0;i<19;i++)
            {
                tableOfReport.Rows[3].Cells[i + 1].Range.InsertAfter(nameColumns[i]);
                tableOfReport.Rows[3].Cells[i+1].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
            }
            for(int i=13;i<=19;i++)
            {
                tableOfReport.Columns[i].Cells[3].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
                tableOfReport.Columns[i].Cells[2].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
                tableOfReport.Columns[i].Cells[3].Merge(tableOfReport.Columns[i].Cells[2]);
            }

            tableOfReport.Columns[10].Cells[2].Range.InsertAfter("Керівництво і приймання індивідуальних завдань:");
            

            for (int i = 2; i < 10; i++)
            {
                tableOfReport.Columns[i].Cells[3].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
                tableOfReport.Columns[i].Cells[2].Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
                tableOfReport.Columns[i].Cells[3].Merge(tableOfReport.Columns[i].Cells[2]);
            }
            tableOfReport.Cell(1,1).Range.Orientation = Word.WdTextOrientation.wdTextOrientationUpward;
            tableOfReport.Columns[2].Cells[1].Range.InsertAfter("Кількість годин");
            tableOfReport.Columns[1].Cells[1].Merge(tableOfReport.Columns[1].Cells[3]);
            tableOfReport.Columns[10].Cells[2].Merge(tableOfReport.Columns[12].Cells[2]);
            tableOfReport.Cell(1,2).Merge(tableOfReport.Cell(1,19));


        }
        
        public void ConvertToWord(Term term1, Term term2)
        {
            Titulka("інформатики та радіоелектроніки", "комп’ютерних наук і технологій",Convert.ToString(term1.beginDate.Year), Convert.ToString(term2.beginDate.Year), "програмних засобів", "Каплієнко Тетяна Ігорівна", "к.т.н.", "доцент");

            InsertText("Розклад занять і графік роботи в приміщеннях вищого навчального закладу на " + Convert.ToString(term1.beginDate.Year) + "-" + Convert.ToString(term2.beginDate.Year) + " навчальний рік",1,14, Word.WdUnderline.wdUnderlineSingle, Word.WdParagraphAlignment.wdAlignParagraphCenter,true);

            InsertText(term1.name, 1, 12, 0, Word.WdParagraphAlignment.wdAlignParagraphLeft, false);

            CreateScheduleTable(term1, false);

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rangeEndOfFile.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            InsertText(term2.name, 1, 12, 0, Word.WdParagraphAlignment.wdAlignParagraphLeft, false);

            CreateScheduleTable(term2, true);

            InsertText("1.Облік виконання навчальної роботи викладача", 1, Convert.ToInt32(14.5), 0, Word.WdParagraphAlignment.wdAlignParagraphLeft, false);
           
            CreateLessonsTable(term1);
        
            CreateLessonsTable(term2);

            InsertFieldWithInitials(zavkaf, prepod, "Book");

            rangeEndOfFile = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            rangeEndOfFile.InsertBreak(Word.WdBreakType.wdSectionBreakNextPage);

            CreateReportTable(term1, term2);
            InsertFieldWithInitials(zavkaf, prepod, "Album");
            //Сохранение документа
            object filename = @"E:\4curs\AV\TeacherJournal-master\Export\TeacherJournal.doc"; // Здесь указать путь до рабочего стола
            adoc.SaveAs(ref filename);
            WordApp.Visible = true;
        }
    }
}
