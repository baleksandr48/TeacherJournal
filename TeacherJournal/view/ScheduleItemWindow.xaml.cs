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

        // Переменные - будущие источники данных для комбобоксов.
        private List<TypeOfWeek> typeOfWeek;
        private List<model.DayOfWeek> dayOfWeek;
        private List<Subject> subject;
        private List<TypeOfLesson> typeOfLesson;
        private List<Classroom> classroom;
        private List<Group> groups;

        private ScheduleWindow window;
        // Объект Schedule который передается для редактирование. Он же потом используется для разветвления кода при сохранении результатов работы окна.
        private Schedule currentSchedule;

        public ScheduleItemWindow()
        {
            InitializeComponent();
        }

        public ScheduleItemWindow(Term term, ScheduleWindow window, Schedule schedule = null)
        {
            InitializeComponent();
            this.currentTerm = term;
            this.window = window;
            this.currentSchedule = schedule;

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

        // После завершения инициализации всего окна выполняем привязку данных. Если нам передали Schedule на редактирование - дополнительно устанавливаем значения переданного объекта в контроллы.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbDayOfWeek.ItemsSource = dayOfWeek;
            cbTypeOfWeek.ItemsSource = typeOfWeek;
            cbTypeOfLesson.ItemsSource = typeOfLesson;
            cbSubject.ItemsSource = subject;
            cbClassroom.ItemsSource = classroom;

            if (currentSchedule != null)
            {
                foreach (model.DayOfWeek item in cbDayOfWeek.Items)
                {
                    if (item.id == currentSchedule.dayOfWeek.id)
                    {
                        cbDayOfWeek.SelectedItem = item;
                        break;
                    }
                }
                foreach (TypeOfWeek item in cbTypeOfWeek.Items)
                {
                    if (item.id == currentSchedule.typeOfWeek.id)
                    {
                        cbTypeOfWeek.SelectedItem = item;
                        break;
                    }
                }
                foreach (TypeOfLesson item in cbTypeOfLesson.Items)
                {
                    if (item.id == currentSchedule.typeOfLesson.id)
                    {
                        cbTypeOfLesson.SelectedItem = item;
                        break;
                    }
                }
                foreach (Subject item in cbSubject.Items)
                {
                    if (item.id == currentSchedule.subject.id)
                    {
                        cbSubject.SelectedItem = item;
                        break;
                    }
                }
                foreach (Classroom item in cbClassroom.Items)
                {
                    if (item.id == currentSchedule.classroom.id)
                    {
                        cbClassroom.SelectedItem = item;
                        break;
                    }
                }
                foreach (Group group in currentSchedule.groups)
                {
                    AddNewGroup(group);
                }
                tbNumberOfLesson.Text = currentSchedule.numOfLesson.ToString();
            }
        }

        // Принимаем и сохраняем изменения в объекте Schedule и передаем его в scheduleList.
        private void AcceptAndSave(object sender, RoutedEventArgs e)
        {
            Schedule schedule = new Schedule();
            schedule.typeOfWeek = cbTypeOfWeek.SelectedItem as TypeOfWeek;
            schedule.typeOfLesson = cbTypeOfLesson.SelectedItem as TypeOfLesson;
            schedule.dayOfWeek = cbDayOfWeek.SelectedItem as model.DayOfWeek;
            schedule.numOfLesson = int.Parse(tbNumberOfLesson.Text);
            schedule.subject = cbSubject.SelectedItem as Subject;
            schedule.classroom = cbClassroom.SelectedItem as Classroom;
            schedule.idTerm = currentTerm.id;
            schedule.groups = new List<Group>();

            // Проходим по всем комбобоксам групп и добавряем выбранные группы в groups.
            foreach (StackPanel child in GroupVerticalPanel.Children)
            {
                foreach (object _child in child.Children)
                {
                    if (_child.GetType().Name == "ComboBox")
                    {
                        Group group = ((ComboBox)_child).SelectedItem as Group;
                        schedule.groups.Add(group);
                        break;
                    }
                }
            }

            var list = this.window.scheduleList;

            if (currentSchedule == null)
            {
                list.Add(schedule);
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (list.ElementAt(i).id == currentSchedule.id)
                    {
                        list[i] = schedule;
                    }
                }
            }
            this.Close();
        }

        // Ивент нажатия хз зачем он тут вообще нужен, пока что подержу а там посмотрим.
        private void btnAddNewGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNewGroup();
        }
        // Удалить группу
        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)((Button)sender).Parent;
            GroupVerticalPanel.Children.Remove(stackPanel);
        }

        // Добавляем новую группу, если передаем аргументом группу - выставляем её как SelectedItem.
        private void AddNewGroup(Group group = null)
        {
            StackPanel stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;

            ComboBox cbGroup = new ComboBox();
            cbGroup.Width = 130;
            cbGroup.ItemsSource = groups;
            cbGroup.DisplayMemberPath = "name";
            cbGroup.SelectedValuePath = "id";

            Button btn = new Button();
            btn.Content = "Del";
            btn.Margin = new Thickness(5, 0, 0, 0);
            btn.Width = 30;
            btn.Click += new RoutedEventHandler(btnDeleteGroup_Click);
            if (group != null)
            {
                foreach (Group item in cbGroup.Items)
                {
                    if (item.id == group.id)
                    {
                        cbGroup.SelectedItem = item;
                        break;
                    }
                }
            }

            stackPanel.Children.Add(cbGroup);
            stackPanel.Children.Add(btn);

            GroupVerticalPanel.Children.Add(stackPanel);
        }
    }
}
