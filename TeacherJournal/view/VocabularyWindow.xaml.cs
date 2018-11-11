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

        private int currentType;
        private Term currentTerm;

        public ObservableCollection<VocabularyEntity> list;

        public VocabularyWindow()
        {
            InitializeComponent();
        }

        public VocabularyWindow(int vocabularyType, Term currentTerm)
        {
            InitializeComponent();
            this.currentType = vocabularyType;
            this.currentTerm = currentTerm;
            tbVocabularyName.Text = vocabularyTypes[currentType];
        }

        /// <summary>
        /// Обработка закрытия формы через кнопку Сохранить и закрыть
        /// Берем нашу коллекцию и передаем её в соответсвующий метод DBHelper-a
        /// </summary>
        private void btnAcceptClose_Click(object sender, RoutedEventArgs e)
        {
            Console.WriteLine(list.Count);
        }

        // Удаляем строку в таблице удаляя объект из коллекции
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                VocabularyEntity obj = ((FrameworkElement)sender).DataContext as VocabularyEntity;
                Console.WriteLine("Name:" + obj.name);
                list.Remove(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        /// <summary>
        /// Обработчик загрузки окна
        /// </summary>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (vocabularyTypes.ContainsKey(currentType))
            {
                if (currentType == SUBJECT)
                {
                    // если словарь предметов
                    list = new ObservableCollection<VocabularyEntity>(DBHelper.selectSubject(currentTerm));
                }
                else if (currentType == GROUP)
                {
                    // если словарь групп
                    list = new ObservableCollection<VocabularyEntity>(DBHelper.selectGroups(currentTerm));
                }
                else if (currentType == CLASSROOM)
                {
                    // если словарь аудиторий
                    list = new ObservableCollection<VocabularyEntity>(DBHelper.selectClassroom(currentTerm));
                }
                VocabularyGrid.ItemsSource = list;
            }
        }
    }
}
