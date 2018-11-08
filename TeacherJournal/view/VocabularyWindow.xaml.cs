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

        List<Classroom> classroomList;
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
                    List<Classroom> classroomList = new List<Classroom>(DBHelper.selectClassroom(currentTerm));
                    VocabularyGrid.ItemsSource = classroomList;
                }
            }
        }
    }
}
