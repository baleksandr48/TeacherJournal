using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using TeacherJournal.database;
using TeacherJournal.model;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для VocabularyWindow.xaml
    /// </summary>
    public partial class VocabularyWindow : Window
    {
        // константы для определения отображения окна словаря
        public const int SUBJECT = 1001;
        public const int GROUP = 1002;
        public const int CLASSROOM = 1003;

        // словарь Словарных типов( лол )
        Dictionary<int, String> vocabularyTypes = new Dictionary<int, string>() {
            { SUBJECT, "Словар предметів"},
            { GROUP, "Словар груп"},
            { CLASSROOM, "Словар аудиторій"}
        };

        int currentType;

        public ObservableCollection<Classroom> classroomList;
        List<Group> groupList;
        List<Subject> subjectList;

        public VocabularyWindow()
        {
            InitializeComponent();
        }
        public VocabularyWindow(int vocabularyType, Term currentTerm)
        {
            InitializeComponent();

            if (vocabularyTypes.ContainsKey(vocabularyType))
            {
                currentType = vocabularyType;
                tbVocabularyName.Text = vocabularyTypes[currentType];

                if (currentType == SUBJECT)
                {
                    // если словарь предметов
                    subjectList = new List<Subject>(DBHelper.selectSubject(currentTerm));
                    VocabularyGrid.ItemsSource = subjectList;
                }
                else if (currentType == GROUP)
                {
                    // если словарь групп
                    groupList = new List<Group>(DBHelper.selectGroups(currentTerm));
                    VocabularyGrid.ItemsSource = groupList;
                }
                else if (currentType == CLASSROOM)
                {
                    // если словарь аудиторий
                    classroomList = new ObservableCollection<Classroom>(DBHelper.selectClassroom(currentTerm));
                    VocabularyGrid.ItemsSource = classroomList;
                }
            }
        }

        /// <summary>
        /// Обработка закрытия формы через кнопку Сохранить и закрыть
        /// Берем нашу коллекцию и передаем её в соответсвующий метод DBHelper-a
        /// </summary>
        private void btnAcceptClose_Click(object sender, RoutedEventArgs e)
        {
            if (currentType == SUBJECT)
            {
                // если словарь предметов
            }
            else if (currentType == GROUP)
            {
                // если словарь групп
                Console.WriteLine(groupList.Count);
            }
            else if (currentType == CLASSROOM)
            {
                // если словарь аудиторий
                Console.WriteLine(classroomList.Count);
            }
        }

        // Удаляем строку в таблице удаляя объект из коллекции
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try {
                if (currentType == SUBJECT)
                {
                    // если словарь предметов
                    Subject obj = ((FrameworkElement)sender).DataContext as Subject;
                    Console.WriteLine("Name:" + obj.name);
                }
                else if (currentType == GROUP)
                {
                    // если словарь групп
                    Group obj = ((FrameworkElement)sender).DataContext as Group;
                    Console.WriteLine("Name:" + obj.name);
                }
                else if (currentType == CLASSROOM)
                {
                    // если словарь аудиторий
                    Classroom obj = ((FrameworkElement)sender).DataContext as Classroom;
                    Console.WriteLine("Name:" + obj.name);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }
    }
}
