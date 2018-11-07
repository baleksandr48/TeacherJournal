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

namespace TeacherJournal
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btnSchedule_Click(object sender, RoutedEventArgs e)
        {
            ScheduleWindow addSchedule = new ScheduleWindow();
            try
            {
                addSchedule.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        private void openVocabulary(object sender, RoutedEventArgs e)
        {
            var btnName = ((Button)sender).Name;
            int vocabularyId = 0;
            switch (btnName)
            {
                case "btnSubject": vocabularyId = VocabularyWindow.SUBJECT; break;
                case "btnClassroom": vocabularyId = VocabularyWindow.CLASSROOM; break;
                case "btnGroup": vocabularyId = VocabularyWindow.GROUP; break;
            }

            VocabularyWindow vocabulary = new VocabularyWindow(vocabularyId);
            try
            {
                vocabulary.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        private void btnAddNewSemester_Click(object sender, RoutedEventArgs e)
        {
            SemesterWindow semester = new SemesterWindow();
            try
            {
                semester.Show();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }
    }
}
