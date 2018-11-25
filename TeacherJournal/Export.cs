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

        private String[] nameColumns = { "Пара", "Номер тижня", "Понеділок", "Вівторок", "Середа", "Четвер", "П'ятниця", "Суббота" };
        private String[] number = { "I", "II", "III", "IV", "V", "VI", "VII", "VIII" };

        public Export()
        {

        }
        public void ConvertToWordSchedule(int numTerms)
        {
            object start1 = 0;
            object end1 = 0;

            //Количество пар, всегда не больше 8
            int maxLessons = 8;
            //Лекции
            Term term = DBHelper.selectTerms()[numTerms];
            List<Schedule> schedules = DBHelper.selectSchedules(term);
            List<Lesson> lessons = DBHelper.selectLessons(term, term.beginDate, term.endDate);
        
            //Количество недель в семестре
            int numOfWeeks = (DBHelper.calculateDays(term.endDate) - DBHelper.calculateDays(term.beginDate)) / 7;
            //Счетчик который показывает в какие дние есть занятия а в какие нет True = есть False = нет
            bool[] DaysIsntNull = new bool[maxLessons];
    
            foreach (Schedule schedule in schedules)
            {     
                for(int i=0; i<maxLessons;i++)
                {
                    for (int k = 1; k < (maxLessons+1); k++)
                    {
                        if (schedule.numOfLesson == i + k)
                        {
                            DaysIsntNull[i+k-1] = true;
                        }
                    }
                }
            }

            Schedule[,] scheduleMatrix = new Schedule[8, 5];

            foreach (Schedule obj in schedules)
            {
                for (int i = 0; i < 8; i++)
                {
                    for (int k = 0; k < 5; k++)
                    {
                        if (obj.numOfLesson == i + 1 && obj.dayOfWeek.id==k+1)
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

            int tableColumns = 7;//Количество столбцов
            int tableRows = (maxRows) + 1;//Общее количество строк в таблице

            //Создание документа
            Word.Application WordApp = new Word.Application();
            Word.Document adoc = WordApp.Documents.Add();
                
            //Создание таблицы
            Word.Range tableScheduleLocation = adoc.Range(ref start1, ref end1);
            Word.Table tableSchedule  = adoc.Tables.Add(tableScheduleLocation, tableRows, tableColumns);
            //Word.Table tableSchedule = tableScheduleLocation.Tables[1];

            object oEndOfDoc = "\\endofdoc";
            Word.Range objRange;
            Word.Table objTable;
            Word.Paragraph objParagraph;
            object objRangePara;

            objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            objParagraph = adoc.Content.Paragraphs.Add(ref objRangePara);
            objParagraph.Range.Text = Environment.NewLine;

            objRange = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
            objTable = adoc.Tables.Add(objRange, 5, 2);

            //Настройка стиля
            tableSchedule.Range.Font.Size = 10;
            tableSchedule.Range.Font.Name = "Times New Roman";
            tableSchedule.Range.ParagraphFormat.SpaceAfter = 4;
            tableSchedule.Range.ParagraphFormat.SpaceBefore = 4;

            //Создает контур
            tableSchedule.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            tableSchedule.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
        

            //Сливание клеток
            for (int i = 0; i < maxLessons;i++ )
            {
                if (DaysIsntNull[i])
                {
                    tableSchedule.Columns[1].Cells[i + 2].Merge(tableSchedule.Columns[1].Cells[i + (numOfWeeks+1)]);
                }
                else
                {
                    tableSchedule.Columns[1].Cells[i + 2].Merge(tableSchedule.Columns[1].Cells[i + 3]);
                }
            }
            //Основная часть заполнение таблицы
            for (int i = 0, posDays = 3; i < 5; i++, posDays++)
            {
                int begin = 2;
                for (int k = 0; k < 8; k++)
                {
                    if (DaysIsntNull[k])
                    {
                        if (scheduleMatrix[k, i] != null)
                        {
                            if (scheduleMatrix[k, i].dayOfWeek.id == i + 1)
                            {
                                tableSchedule.Columns[posDays].Cells[begin].Merge(tableSchedule.Columns[posDays].Cells[begin + numOfWeeks-1]);
                                tableSchedule.Columns[posDays].Cells[begin].Range.InsertAfter(scheduleMatrix[k,i].subject.name);
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
                tableSchedule.Cell(1, 1 + i).Range.InsertAfter(nameColumns[i]);
                tableSchedule.Columns[1 + i].Cells[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                tableSchedule.Columns[1 + i].Cells[1].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tableSchedule.Columns[1 + i].Cells[1].Range.Font.Bold = 1;
            }
                
            //Заполнение клеток
            for (int i = 0; i < tableSchedule.Columns[1].Cells.Count - 1; i++)
            {
                tableSchedule.Columns[1].Cells[2 + i].Range.InsertAfter(number[i]);
                tableSchedule.Columns[1].Cells[2 + i].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                tableSchedule.Columns[1].Cells[2 + i].VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;
                tableSchedule.Columns[1].Cells[2 + i].Range.Font.Bold = 1;
                tableSchedule.Columns[1].Cells[2 + i].Range.Font.Size = 14;
                //tableSchedule.Rows[i + 1].Cells[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
            }
        
            //Нумеруем 2 колонку
            for(int i=0, position=1; i<maxLessons; i++)
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
            //Сохранение документа
            object filename = @"E:\4curs\AV\TeacherJournal-master\Export\TeacherJournalSchedule.doc"; // Здесь указать путь до рабочего стола
        adoc.SaveAs(ref filename);
        WordApp.Visible = true;
        }
        public void ConvertToWordLessons2(int numTerms)
        {
            Word.Application WordApp = new Word.Application();
            Word.Document adoc = WordApp.Documents.Add();

            object oEndOfDoc = "\\endofdoc";
            Word.Range objRange;
            Word.Table objTable;
            Word.Paragraph objParagraph;
            object objRangePara;
            for (int nIndex = 0; nIndex < 3; nIndex++)
            {
                objRange = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;

                objTable = adoc.Tables.Add(objRange, 5, 2);
                objRangePara = adoc.Bookmarks.get_Item(ref oEndOfDoc).Range;
                objParagraph = adoc.Content.Paragraphs.Add(ref objRangePara);
                objParagraph.Range.Text = Environment.NewLine;

                objTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                objTable.Borders.InsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
            }
            //Сохранение документа
            object filename = @"E:\4curs\AV\TeacherJournal-master\Export\TeacherJournalLessons.doc"; // Здесь указать путь до рабочего стола
            adoc.SaveAs(ref filename);
            WordApp.Visible = true;
        }

    }

    
}
