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
        private Term currentTerm;
        private bool NeedToUpdateLessons;

        public SemesterWindow()
        {
            InitializeComponent();
        }

        public SemesterWindow(Term term = null)
        {
            InitializeComponent();
            this.currentTerm = term;
            this.NeedToUpdateLessons = false;
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            if ((dpSemesterStartDate.SelectedDate != null) && (dpSemesterEndDate.SelectedDate != null) && (tbxSemesterName.Text != ""))
            {
                String termName = tbxSemesterName.Text;
                DateTime termStartDate = dpSemesterStartDate.SelectedDate.Value.Date;
                DateTime termEndDate = dpSemesterEndDate.SelectedDate.Value.Date;
                int startWithNumerator = chbStartWithNumerator.IsChecked.Value ? 1 : 0;

                // Записываем семестр в бд.

                // Сделать перезапись данных, если в существующем семестре изменили дату начала или окончания. !!!!!!!
                              
                if (currentTerm == null)
                {
                    if (MessageBox.Show("Ви підтверджуєте створення нового семестру?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    {
                        currentTerm = new Term(Term.ID_FOR_WRITING, termName, termStartDate, termEndDate, startWithNumerator);
                        try
                        {
                            DBHelper.addTerm(currentTerm);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("{0} Exception cought", ex);
                        }
                        this.DialogResult = true;
                    }
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("Перезаписати дані занять?", "Підтвердження", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        NeedToUpdateLessons = true;
                    }
                    else if (result == MessageBoxResult.No)
                    {
                        NeedToUpdateLessons = false;
                    }
                    else return;

                    currentTerm.name = termName;
                    currentTerm.beginDate = termStartDate;
                    currentTerm.endDate = termEndDate;
                    currentTerm.startFromNumerator = startWithNumerator;
                    try
                    {
                        DBHelper.updateTerm(currentTerm);

                        if (NeedToUpdateLessons)
                        {
                            List<Schedule> scheduleList = DBHelper.selectSchedules(currentTerm);
                            DBHelper.ClearLesson(currentTerm);
                            DBHelper.addLessons(Lesson.generateLessons(scheduleList, currentTerm));
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("{0} Exception cought", ex);
                    }
                    this.DialogResult = true;
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                tbxSemesterName.Text = currentTerm.name;
                dpSemesterStartDate.SelectedDate = currentTerm.beginDate;
                dpSemesterEndDate.SelectedDate = currentTerm.endDate;
                chbStartWithNumerator.IsChecked = currentTerm.startFromNumerator == 1 ? true : false;
            }
        }
    }
}
