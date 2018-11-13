using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using TeacherJournal.model;
using TeacherJournal.database;
using System.Collections.ObjectModel;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private Term currentTerm;
        private ObservableCollection<Schedule> scheduleList;

        public ScheduleWindow()
        {
            InitializeComponent();
        }

        public ScheduleWindow(Term term)
        {
            InitializeComponent();
            this.currentTerm = term;

            scheduleList = new ObservableCollection<Schedule>(DBHelper.selectSchedules(currentTerm));
        }

        private void btnFillSchedule_Click(object sender, RoutedEventArgs e)
        {
            /// Use date from table to fill schedule
            this.Close();
        }

        private void btnAddNewScheduleRow_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItemWindow addScheduleItem = new ScheduleItemWindow(currentTerm);
            try
            {
                addScheduleItem.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduleGrid.ItemsSource = scheduleList;
        }

        // Удаляем объект Schedule со scheduleList
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Schedule obj = ((FrameworkElement)sender).DataContext as Schedule;
                scheduleList.Remove(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }
        // Редактируем объект Schedule
        private void EditeRow_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RandScheduleItem_Click(object sender, RoutedEventArgs e)
        {
            scheduleList.Add(DBHelper.AddRandomSchedule(currentTerm));
        }
    }
}
