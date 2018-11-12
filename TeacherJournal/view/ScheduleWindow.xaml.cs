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

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private Term currentTerm;

        public ScheduleWindow()
        {
            InitializeComponent();
        }

        public ScheduleWindow(Term term)
        {
            InitializeComponent();
            this.currentTerm = term;
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
    }
}
