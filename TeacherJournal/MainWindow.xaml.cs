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
        // Переменная хранит текущую начальную дату.
        private DateTime startDateValue;
        // Конечную дату ---- нужны из-за того, что обработчик dp_SelectedDateChanged местами вызывается дважды подряд.
        private DateTime endDateValue; 

        Term currentTerm;
        
        public MainWindow()
        {
            InitializeComponent();
            DBSetuper.setup();

            // Привязываем коллекцию семестров к cbSemesterList windowLoaded.
            termList = new ObservableCollection<Term>(DBHelper.selectTerms());
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbSemesterList.ItemsSource = termList;
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
                    vocabulary.ShowDialog();
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
            SemesterWindow semester = new SemesterWindow();
            try
            {
                if (semester.ShowDialog() == true)
                {
                    Term newTerm = DBHelper.getLastTerm();
                    termList.Add(newTerm);
                    cbSemesterList.SelectedItem = newTerm;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        // Открытие окна для добавления нового семестра.
        private void btnAddNewSemester_Click(object sender, RoutedEventArgs e)
        {
            OpenAddTermWindow();
        }

        // Открытие окна расписания.
        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                ScheduleWindow Schedule = new ScheduleWindow(currentTerm);
                try
                {
                    if (Schedule.ShowDialog() == true)
                    {
                        UpdateLessonList();
                    }                
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception cought", ex);
                }
            }
            else
            {
                OpenAddTermWindow();
            }
        }

        // При выборе семестра возвращаем выбранный айтем и приводим тип к Term. Узнаем id. 
        private void cbSemesterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Term term = (Term)cbSemesterList.SelectedItem;
            if(term != null)
            { 
                currentTerm = term;

                // Если даты выбранного семестра совпадают с датами датапикеров - вызываем обновление списка вручную.
                if ((startDateValue == currentTerm.beginDate) && (endDateValue == currentTerm.endDate))
                {
                    UpdateLessonList();
                }
                // Если нет - задаем датапикерам дату начала и конца выбранного семестра.
                else
                {
                    dpStartDate.SelectedDate = currentTerm.beginDate;
                    dpEndDate.SelectedDate = currentTerm.endDate;
                }             
            }
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
                // Задаем датапикерам дату сегодня.
                dpStartDate.SelectedDate = DateTime.Today;
                dpEndDate.SelectedDate = DateTime.Today;
            }
        }

        // Обновляем список занятий с помощью повторного запроса в бд и, если, результат больше нуля - обновляем ItemSource нашего lessonGrid.
        public  void UpdateLessonList()
        {
            if (currentTerm != null)
            {
                startDateValue = dpStartDate.SelectedDate.Value.Date;
                endDateValue = dpEndDate.SelectedDate.Value.Date;
                lessonList = new ObservableCollection<Lesson>();
                if ((lessonList != null))
                {
                    //сортируем список и делаем егои сточником данных для таблицы
                    List<Lesson> tempLessons = DBHelper.selectLessons(currentTerm, startDateValue, endDateValue);
                    lessonList = new ObservableCollection<Lesson>(tempLessons.OrderBy(l => l.date).ThenBy(l => l.numOfLesson).ToList());
                    lessonsGrid.ItemsSource = lessonList;
                }
                else {
                    lessonsGrid.ItemsSource = null;
                }
            }
        }

        // Сортировка lessonGrid по указанному названию столбца и в указанном направлении.
        /*private void SortLessonGridByPropertyName(string propertyName, ListSortDirection direction)
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
        }*/

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

        // Нажатие на кнопку редактирования конкретного занятия. Вызываем окно и передаем объект подлежащий редактированию.
        private void EditRow_Click(object sender, RoutedEventArgs e)
        {
            Lesson obj = ((FrameworkElement)sender).DataContext as Lesson;
            OpenLessonItemWindow(obj);
        }

        // Двойное нажатие ЛЕВОЙ кнопки на строку. Нужно редактировать объект Lesson в строке. 
        private void Row_DoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DataGridRow row = sender as DataGridRow;
                Lesson obj = ((FrameworkElement)sender).DataContext as Lesson;
                OpenLessonItemWindow(obj);
            }
            e.Handled = true;
        }

        // Открытие окна для редактирования Lesson.
        private void OpenLessonItemWindow(Lesson ls)
        {
            LessonItemWindow window = new LessonItemWindow(ls, currentTerm, this);
            try
            {
                window.ShowDialog();
                UpdateLessonList();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        // Событие изменения даты на датапикерах. В связи с тем, что может вызываться дважды для каждого датапикера, вводим проверку с startDateValue и endDateValue.
        // Вызываем обновление списка занятий.
        private void dp_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DatePicker datePicker = (DatePicker)sender;
            String senderName = datePicker.Name;
            DateTime dateValue = datePicker.SelectedDate.Value.Date;
            
            if(senderName == "dpStartDate")
            {
                if (startDateValue != dateValue)
                {
                    startDateValue = dateValue;
                    if (endDateValue >= startDateValue)
                    {
                        UpdateLessonList();
                    }
                } 
            }
            else if (senderName == "dpEndDate")
            {
                if (endDateValue != dateValue)
                {
                    endDateValue = dateValue;
                    if (endDateValue >= startDateValue)
                    {
                        UpdateLessonList();
                    }
                }
            }

        }

        // Событие ввода данных в датапикер. Вводить можно только числовые значения и ".".
        private void dpPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Validators.DatePickerValidation(sender, e);
        }

        // Очистка всея бд по нажатия на соответствующий пункт меню.
        private void ClearTerms_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ви впевненні, що хочете очистити файл бд?", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    DBHelper.ClearTerms();
                    termList.Clear();
                    lessonList.Clear();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("{0} Exception cought", ex);
                }
            }
        }

        // Открыть информацию о преподавателе
        private void OpenTeacherInfo_Click(object sender, RoutedEventArgs e)
        {
            TeacherInfoWindow window = new TeacherInfoWindow();
            window.ShowDialog();
        }

        // Нажатие кнопки редактировать семестр.
        private void btnEditSemester_Click(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                SemesterWindow window = new SemesterWindow(currentTerm);
                if (window.ShowDialog() == true)
                {
                    // Делаем небольшой костыль, чтобы таблица не обновлялась дважды.
                    endDateValue = DateTime.MinValue;
                    cbSemesterList.SelectedItem = null;
                    cbSemesterList.SelectedItem = currentTerm;
                    /* dpStartDate.SelectedDate = currentTerm.beginDate;
                     dpEndDate.SelectedDate = currentTerm.endDate;*/
                }
            }
        }

        // Обработка нажатия кнопки в Док.
        private void btnToDoc_Click(object sender, RoutedEventArgs e)
        {
            ExportWindow exportWindow = new ExportWindow();
            exportWindow.ShowDialog();
        }
    
        // Добавление занятия.
        public void btnAddLesson(object sender, RoutedEventArgs e)
        {
            if (currentTerm != null)
            {
                OpenLessonItemWindow(null);    
            }
            else
            {
                OpenAddTermWindow();
            }
        }

    }  
}
