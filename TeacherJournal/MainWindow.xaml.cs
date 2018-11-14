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

namespace TeacherJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Term> termList;
        private ObservableCollection<Lesson> lessonList;

        Term currentTerm;

        public MainWindow()
        {
            InitializeComponent();
            DBSetuper.setup();

            // привязываем коллекцию семестров к cbSemesterList
            termList = new ObservableCollection<Term>(DBHelper.selectTerms());
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
         //   Term currentTerm = (Term)cbSemesterList.SelectedItem;
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
                openAddTermWindow();
            }
        }

        /// <summary>
        /// Метод который срабатывает при нажатии на кнопки: аудитории, группы, предметы. 
        /// Узнаем что это за кнопка, и передаем соответствующую константу окну VocabularyWindow для построения соответствующего интерфейса.
        /// </summary>
        private void openVocabulary(object sender, RoutedEventArgs e)
        {
            var btnName = ((Button)sender).Name;
            int vocabularyId = 0;
          //  Term currentTerm = (Term)cbSemesterList.SelectedItem;

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
                openAddTermWindow();
            }
        }

        private void openAddTermWindow()
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

        // Открываем окно для добавления нового семестра
        private void btnAddNewSemester_Click(object sender, RoutedEventArgs e)
        {
            openAddTermWindow();
        }

        // При выборе семестра возвращаем выбранный айтем и приводим тип к Term. Узнаем id. 
        private void cbSemesterList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Term term = (Term)cbSemesterList.SelectedItem;
            currentTerm = term;
            if (currentTerm != null)
            {
                updateDatePickers();
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbSemesterList.ItemsSource = termList;
            lessonList = new ObservableCollection<Lesson>(DBHelper.selectLessons(currentTerm, dpStartDate.SelectedDate.Value.Date, dpEndDate.SelectedDate.Value.Date));
            lessonsGrid.ItemsSource = lessonList;

            //---- СОРТИРОВКА ПО ДАТЕ
            //create a collection view for the datasoruce binded with grid
            ICollectionView dataView = CollectionViewSource.GetDefaultView(lessonsGrid.ItemsSource);
            //clear the existing sort order
            dataView.SortDescriptions.Clear();
            //create a new sort order for the sorting that is done lastly
            dataView.SortDescriptions.Add(new SortDescription("date", System.ComponentModel.ListSortDirection.Ascending));
            //refresh the view which in turn refresh the grid
            dataView.Refresh();
            //---- СОРТИРОВКА ПО ДАТЕ
        }

        private void updateDatePickers()
        {
            dpStartDate.SelectedDate = currentTerm.beginDate;
            dpEndDate.SelectedDate = currentTerm.endDate;
        }
    }  
}
