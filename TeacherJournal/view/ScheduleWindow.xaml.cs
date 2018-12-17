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
using System.ComponentModel;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private Term currentTerm;
        public ObservableCollection<Schedule> scheduleList;
        // Форма с progressbar.
        LoadingForm loadingForm;

        public ScheduleWindow()
        {
            InitializeComponent();
        }

        public ScheduleWindow(Term term)
        {
            InitializeComponent();
            this.currentTerm = term;

            scheduleList = new ObservableCollection<Schedule>();
        }

        // После завершения инициализации всего окна выполняем привязку данных.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<Schedule> temp = DBHelper.selectSchedules(currentTerm);
            temp = temp.OrderBy(s => s.dayOfWeek).ThenBy(s => s.numOfLesson).ToList();
            
            scheduleList = new ObservableCollection<Schedule>(temp);
            ScheduleGrid.ItemsSource = scheduleList;
        }

        // Используем данные с таблицы чтобы заполнить таблицу Lesson в бд.
        private void btnFillSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ви підтверджуєте прийняття розкладу?\nУ разі, якщо існує розклад на цей семестр, " +
                "він буде перезаписаний новими даними.", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (Schedule.isSchedulesCorrect(scheduleList.ToList()))
                {
                    BackgroundWorker bg = new BackgroundWorker();
                    bg.DoWork += new DoWorkEventHandler(bg_DoWork);
                    bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
                    // Запуск worker.
                    bg.RunWorkerAsync();
                    // Отображаем loading form.
                    loadingForm = new LoadingForm("Заповнення журналу занять");
                    loadingForm.ShowDialog();
                }
                else
                {
                    MessageBox.Show("Ви невірно заповнили розклад!" +
                        "\nВикладач не може знаходитися в двух місцях одночасно.", "Похибка");
                }
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            // Очистка расписания и занятий. Создание новых данных по занятиям и расписанию.
            try
            {
                DBHelper.ClearSchedule(currentTerm);
                DBHelper.ClearLesson(currentTerm);
                DBHelper.addSchedules(scheduleList.ToList());
                DBHelper.addLessons(Lesson.generateLessons(scheduleList.ToList(), currentTerm));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Закрыть loading form и это окно.
            loadingForm.WorkEnded = true;
            loadingForm.Close();
            this.DialogResult = true;
        }

        // Добавляем объект Schedule.
        private void btnAddNewScheduleRow_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItemWindow addScheduleItem = new ScheduleItemWindow(currentTerm, this);
            try
            {
                addScheduleItem.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        // Удаляем объект Schedule со scheduleList.
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

        // Редактируем объект Schedule.
        private void EditeRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Schedule obj = ((FrameworkElement)sender).DataContext as Schedule;
                ScheduleItemWindow window = new ScheduleItemWindow(currentTerm, this, obj);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        // Двойное нажатие ЛЕВОЙ кнопки на строку. Нужно редактировать объект Schedule в строке. 
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow row = sender as DataGridRow;
                Schedule obj = ((FrameworkElement)sender).DataContext as Schedule;
                ScheduleItemWindow window = new ScheduleItemWindow(currentTerm, this, obj);
                window.ShowDialog();
            }
            e.Handled = true;
        }
        
    }
}
