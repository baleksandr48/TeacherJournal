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
    public class VisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null && (long)value > -1)
            {
                return Visibility.Visible;
            }
            else
            {
                return Visibility.Hidden;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// Конвентор для отображения List<Group> в виде String в таблице ScheduleWindow
    /// </summary>
    public class ListToStringConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {     
            if (value != null)
            {
                List<Group> list = new List<Group>((List<Group>)value);
                List<string> nameList = list.Select(o => o.name).ToList(); // используем LINQ чтобы сотворить лист строк имен групп из листа групп

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