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

        List<int> vocabularyTypes = new List<int>() { SUBJECT, GROUP, CLASSROOM };
        int currentType;

        List<Classroom> classroomList;

        public VocabularyWindow()
        {
            InitializeComponent();
        }
        public VocabularyWindow(int vocabularyType, Term currentTerm)
        {
            InitializeComponent();
            if (vocabularyTypes.Contains(vocabularyType))
            {
                currentType = vocabularyType;
                textBlock.Text = currentType.ToString();

                classroomList = new List<Classroom>(DBHelper.selectClassroom(currentTerm));
                VocabularyGrid.ItemsSource = classroomList;
            
            }
        }
    }
}
