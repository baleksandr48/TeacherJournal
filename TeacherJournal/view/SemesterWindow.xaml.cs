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

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для SemesterWindow.xaml
    /// </summary>
    public partial class SemesterWindow : Window
    {
        MainWindow mainWindow;

        public SemesterWindow(MainWindow window)
        {
            InitializeComponent();
            mainWindow = window;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            String termName = tbxSemesterName.Text;
            DateTime termStartDate = dpSemesterStartDate.SelectedDate.Value.Date;
            DateTime termEndDate = dpSemesterEndDate.SelectedDate.Value.Date;
            int startWithNumerator = chbStartWithNumerator.IsChecked.Value ? 1 : 0;
            
            // записываем семестр в бд, после этого добавляем в коллекцию termList mainwindow-a последний семестр
            if (termName != "" && termStartDate != null && termEndDate != null)
            {
                Term term = new Term(Term.ID_FOR_WRITING, termName, termStartDate, termEndDate, startWithNumerator);
                DBHelper.addTerm(term);
                mainWindow.termList.Add(DBHelper.getLastTerm());
            }

            this.Close();
        }
    }
}
