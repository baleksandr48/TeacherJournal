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

namespace TeacherJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ObservableCollection<Term> termList;

        public MainWindow()
        {
            InitializeComponent();
            DBSetuper.setup();

            // привязываем коллекцию семестров к cbSemesterList
            termList = new ObservableCollection<Term>(DBHelper.selectTerms());
            cbSemesterList.ItemsSource = termList;
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            Term currentTerm = (Term)cbSemesterList.SelectedItem;
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
            Term currentTerm = (Term)cbSemesterList.SelectedItem;

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
            Console.WriteLine("Term id: " + term.id);
        }
    }  
}
