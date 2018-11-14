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
    /// Логика взаимодействия для ScheduleItemWindow.xaml
    /// </summary>
    public partial class ScheduleItemWindow : Window
    {
        private Term currentTerm;

        private List<TypeOfWeek> typeOfWeek;
        private List<model.DayOfWeek> dayOfWeek;
        private int numOfLesson;
        private List<Subject> subject;
        private List<TypeOfLesson> typeOfLesson;
        private List<Classroom> classroom;
        private List<Group> groups;

        private ScheduleWindow window;

        public ScheduleItemWindow()
        {
            InitializeComponent();
        }

        public ScheduleItemWindow(Term term, ScheduleWindow window)
        {
            InitializeComponent();
            this.currentTerm = term;
            this.window = window; 

            try
            {
                typeOfWeek = new List<TypeOfWeek>(DBHelper.selectTypesOfWeek());
                dayOfWeek = new List<model.DayOfWeek>(DBHelper.selectDaysOfWeek());
                typeOfLesson = new List<TypeOfLesson>(DBHelper.selectTypesOfLesson());
                subject = new List<Subject>(DBHelper.selectSubject(currentTerm));
                classroom = new List<Classroom>(DBHelper.selectClassroom(currentTerm));
                groups = new List<Group>(DBHelper.selectGroups(currentTerm));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        private void btnAccept_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnAddNewGroup_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            ComboBox comboBox = new ComboBox();
            comboBox.Width = 130;
            comboBox.ItemsSource = groups;
            comboBox.DisplayMemberPath = "name";
            comboBox.SelectedValuePath = "id";

            Button btn = new Button();
            btn.Content = "Del";
            btn.Margin = new Thickness(5, 0, 0, 0);
            btn.Width = 30;
            btn.Click += new RoutedEventHandler(btnDeleteGroup_Click);

            stackPanel.Children.Add(comboBox);
            stackPanel.Children.Add(btn);

            GroupVerticalPanel.Children.Add(stackPanel);
        }
        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)((Button)sender).Parent;
            GroupVerticalPanel.Children.Remove(stackPanel);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbDayOfWeek.ItemsSource = dayOfWeek;
            cbTypeOfWeek.ItemsSource = typeOfWeek;
            cbTypeOfLesson.ItemsSource = typeOfLesson;
            cbSubject.ItemsSource = subject;
            cbClassroom.ItemsSource = classroom;
        }
    }
}
