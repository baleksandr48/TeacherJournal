using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using TeacherJournal.model;

namespace TeacherJournal
{
    // Конвертор для отображения List<Group> в виде строки.
    public class ListToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {     
            if (value != null)
            {
                List<Group> list = new List<Group>((List<Group>)value);
                // Используем LINQ чтобы создать список из строк имен групп из списка групп.
                List<string> nameList = list.Select(o => o.name).ToList(); 

                return String.Join(", ", nameList.ToArray());
            }
            return "";
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}