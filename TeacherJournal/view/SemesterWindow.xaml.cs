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
        private Term currentTerm;

        public SemesterWindow()
        {
            InitializeComponent();
        }

        public SemesterWindow(MainWindow window, Term term = null)
        {
            InitializeComponent();
            this.mainWindow = window;
            this.currentTerm = term;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if ((dpSemesterStartDate.SelectedDate != null) && (dpSemesterEndDate.SelectedDate != null) && (tbxSemesterName.Text != ""))
            {
                String termName = tbxSemesterName.Text;
                DateTime termStartDate = dpSemesterStartDate.SelectedDate.Value.Date;
                DateTime termEndDate = dpSemesterEndDate.SelectedDate.Value.Date;
                int startWithNumerator = chbStartWithNumerator.IsChecked.Value ? 1 : 0;

                // Записываем семестр в бд, после этого добавляем в коллекцию termList mainwindow-a последний семестр.
                if (MessageBox.Show("Ви підтверджуєте створення нового семестру?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    try
                    {
                        currentTerm = new Term(Term.ID_FOR_WRITING, termName, termStartDate, termEndDate, startWithNumerator);
                        DBHelper.addTerm(currentTerm);
                        mainWindow.termList.Add(currentTerm);
                        this.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0} Exception cought", ex);
                    }
                }
            }
            else
            {
                MessageBox.Show("Заповніть всі необхідні поля!", "Попередження");
            }
        }

        private void dpPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Validators.DatePickerValidation(sender, e);
        }
    }
}
