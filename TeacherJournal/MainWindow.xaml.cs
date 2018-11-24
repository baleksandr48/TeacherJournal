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
using System.Windows.Navigation;
using System.Windows.Shapes;
using TeacherJournal.view;
using TeacherJournal.database;
using TeacherJournal.model;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using WPFCustomMessageBox;

namespace TeacherJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // Список семестров. Используем его так же из окна SemesterWindow.
        public ObservableCollection<Term> termList;
        // Список для хранения занятий.
        public ObservableCollection<Lesson> lessonList;
        // Сеременная хранит текущую начальную дату.
        private DateTime startDateValue;
        // Сонечную дату ---- нужны из-за того, что обработчик dp_SelectedDateChanged местами вызывается дважды подряд.
        private DateTime endDateValue; 

        Term currentTerm;
        
        public MainWindow()
        {
            InitializeComponent();
            DBSetuper.setup();

            // Привязываем коллекцию семестров к cbSemesterList.
            termList = new ObservableCollection<Term>(DBHelper.selectTerms());
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                ScheduleWindow Schedule = new ScheduleWindow(currentTerm);
                try
                {
                    Schedule.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception cought", ex);
                }
            }
            else {
                OpenAddTermWindow();
            }
        }

        /// <summary>
        /// Метод который срабатывает при нажатии на кнопки: аудитории, группы, предметы. 
        /// Узнаем что это за кнопка, и передаем соответствующую константу окну VocabularyWindow для построения соответствующего интерфейса.
        /// </summary>
        private void OpenVocabulary(object sender, RoutedEventArgs e)
        {
            var btnName = ((Button)sender).Name;
            int vocabularyId = 0;

            if (currentTerm != null)
            {
                switch (btnName)
                {
                    case "btnSubject": vocabularyId = VocabularyWindow.SUBJECT; break;
                    case "btnClassroom": vocabularyId = VocabularyWindow.CLASSROOM; break;
                    case "btnGroup": vocabularyId = VocabularyWindow.GROUP; break;
                }

                VocabularyWindow vocabulary = new VocabularyWindow(vocabularyId, currentTerm);
                try
                {
                    vocabulary.Show();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception cought", ex);
                }
            }
            else {
                OpenAddTermWindow();
            }
        }

        private void OpenAddTermWindow()
        {
            SemesterWindow semester = new SemesterWindow(this);
            try
            {
                semester.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        // Открываем окно для добавления нового семестра.
        private void btnAddNewSemester_Click(object sender, RoutedEventArgs e)
        {
            OpenAddTermWindow();
        }

        // При выборе семестра возвращаем выбранный айтем и приводим тип к Term. Узнаем id. 
        private void cbSemesterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Term term = (Term)cbSemesterList.SelectedItem;
            currentTerm = term;
            if (currentTerm != null)
            {
                // Задаем датапикерам дату начала и конца выбранного семестра.
                dpStartDate.SelectedDate = currentTerm.beginDate;
                dpEndDate.SelectedDate = currentTerm.endDate;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbSemesterList.ItemsSource = termList;    
        }

        // Отобразить весь семестр в главной таблице посредством установления дат в даты начала и окончания семестра.
        private void ShowAllTerm(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                // Задаем датапикерам дату начала и конца выбранного семестра.
                dpStartDate.SelectedDate = currentTerm.beginDate;
                dpEndDate.SelectedDate = currentTerm.endDate;
            }
        }

        private void ShowToday(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                // Задаем датапикерам дату начала и конца выбранного семестра.
                dpStartDate.SelectedDate = DateTime.Today;
                dpEndDate.SelectedDate = DateTime.Today;
            }
        }
        

        // Обновляем список занятий с помощью повторного запроса в бд и, если, результат больше нуля - обновляем ItemSource нашего lessonGrid.
        private void UpdateLessonList()
        {
            if (currentTerm != null)
            {
                lessonList = new ObservableCollection<Lesson>(DBHelper.selectLessons(currentTerm, dpStartDate.SelectedDate.Value.Date, dpEndDate.SelectedDate.Value.Date));
                if ((lessonList != null) && (lessonList.Count != 0))
                {
                    lessonsGrid.ItemsSource = lessonList;
                    SortLessonGridByPropertyName("date", ListSortDirection.Ascending);
                }
                else {
                    lessonsGrid.ItemsSource = null;
                }
            }
        }

        // Сортировка lessonGrid по указанному названию столбца и в указанном направлении.
        private void SortLessonGridByPropertyName(string propertyName, ListSortDirection direction)
        {
            if (propertyName != "")
            {
                //create a collection view for the datasoruce binded with grid
                ICollectionView dataView = CollectionViewSource.GetDefaultView(lessonsGrid.ItemsSource);
                //clear the existing sort order
                dataView.SortDescriptions.Clear();
                //create a new sort order for the sorting that is done lastly
                dataView.SortDescriptions.Add(new SortDescription(propertyName, direction));
                //refresh the view which in turn refresh the grid
                dataView.Refresh();
            }
        }

        // Удаляем строку с таблицы и с бд.
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ви впевненні, що хочете видалити цей запис?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    Lesson obj = ((FrameworkElement)sender).DataContext as Lesson;
                    lessonList.Remove(obj);
                    DBHelper.deleteLesson(obj);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception caught", ex);
                }
            }  
        }

        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            Lesson obj = ((FrameworkElement)sender).DataContext as Lesson;
            LessonItemWindow window = new LessonItemWindow(obj, currentTerm, this);
            try
            {
                window.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }

        }

        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            String senderName = datePicker.Name;
            DateTime dateValue = datePicker.SelectedDate.Value.Date;

            if (senderName == "dpStartDate")
            {
                if (startDateValue != dateValue) { startDateValue = dateValue; if (endDateValue >= startDateValue && startDateValue != null) { UpdateLessonList(); } } 
            }
            else if (senderName == "dpEndDate")
            {
                if (endDateValue != dateValue) { endDateValue = dateValue; if (endDateValue >= startDateValue && startDateValue != null) { UpdateLessonList(); } }
            }
        }

        private void dpPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Validators.DatePickerValidation(sender, e);
        }
    }  
}
