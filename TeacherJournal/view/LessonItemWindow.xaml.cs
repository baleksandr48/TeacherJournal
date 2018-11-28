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
using TeacherJournal.view;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для LessonItemWindow.xaml
    /// </summary>
    public partial class LessonItemWindow : Window
    {
        private MainWindow mainWindow;
        private Lesson currentLesson;
        private Term currentTerm;
        private List<Group> groups;
        private List<Classroom> classrooms;
        private List<Subject> subjects;

        public LessonItemWindow()
        {
            InitializeComponent();
        }

        public LessonItemWindow(Lesson lesson, Term term, MainWindow mainWindow)
        {
            InitializeComponent();

            this.currentLesson = lesson;
            this.currentTerm = term;
            this.mainWindow = mainWindow;

            try
            {
                this.subjects = new List<Subject>(DBHelper.selectSubject(currentTerm));
                this.classrooms = new List<Classroom>(DBHelper.selectClassroom(currentTerm));
                this.groups = new List<Group>(DBHelper.selectGroups(currentTerm));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbLessonSubject.ItemsSource = subjects;
            cbLessonClassroom.ItemsSource = classrooms;
            if (currentLesson != null)
            {
                foreach (Subject item in cbLessonSubject.Items)
                {
                    if (item.id == currentLesson.subject.id)
                    {
                        cbLessonSubject.SelectedItem = item;
                        break;
                    }
                }
                foreach (Classroom item in cbLessonClassroom.Items)
                {
                    if (item.id == currentLesson.classroom.id)
                    {
                        cbLessonClassroom.SelectedItem = item;
                        break;
                    }
                }
                foreach (Group group in currentLesson.groups)
                {
                    AddNewGroup(group);
                }
                tbLessonTheme.Text = currentLesson.theme;
                tbLessonNumber.Text = currentLesson.numOfLesson.ToString();
                dpLessonDate.SelectedDate = currentLesson.date.Date;
            }
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

        // Ивент нажатия хз зачем он тут вообще нужен, пока что подержу а там посмотрим.
        private void btnAddNewGroup_Click(object sender, RoutedEventArgs e)
        {
            AddNewGroup();
        }

        // Удалить группу.
        private void btnDeleteGroup_Click(object sender, RoutedEventArgs e)
        {
            StackPanel stackPanel = (StackPanel)((Button)sender).Parent;
            GroupVerticalPanel.Children.Remove(stackPanel);
        }

        private void AcceptAndSave(object sender, RoutedEventArgs e)
        {
            if ((dpLessonDate.SelectedDate != null) && (cbLessonSubject.SelectedItem != null) && (cbLessonClassroom.SelectedItem != null)
                && AreGroupsFilled() && (tbLessonNumber.Text != ""))
            {
                int numOfLesson;
                if (int.TryParse(tbLessonNumber.Text, out numOfLesson))
                {
                    Lesson lesson = new Lesson();
                    lesson.numOfLesson = numOfLesson;
                    lesson.subject = cbLessonSubject.SelectedItem as Subject;
                    lesson.classroom = cbLessonClassroom.SelectedItem as Classroom;
                    lesson.theme = tbLessonTheme.Text;
                    lesson.idTerm = currentTerm.id;
                    lesson.date = dpLessonDate.SelectedDate.Value.Date;
                    lesson.groups = new List<Group>();

                    // Проходим по всем комбобоксам групп и добавряем выбранные группы в groups.
                    foreach (StackPanel child in GroupVerticalPanel.Children)
                    {
                        foreach (object _child in child.Children)
                        {
                            if (_child.GetType().Name == "ComboBox")
                            {
                                Group group = ((ComboBox)_child).SelectedItem as Group;
                                lesson.groups.Add(group);
                                break;
                            }
                        }
                    }
                    // Сначала нужно установить тип Занятия.
                    lesson.typeOfLesson = currentLesson.typeOfLesson;
                    var list = this.mainWindow.lessonList;
                    for (int i = 0; i < list.Count; i++)
                    {
                        if (list.ElementAt(i).id == currentLesson.id)
                        {
                            list[i] = lesson;
                        }
                    }
                    // Сохранение редактирования в БД - удаляем старый, добавляем новый.
                    DBHelper.deleteLesson(currentLesson);
                    DBHelper.addLesson(lesson);

                    this.Close();
                }
                else
                {
                    MessageBox.Show("Введіть правильний номер заняття!", "Попередження!");
                }

            }
            else {
                MessageBox.Show("Заповніть всі необхідні поля!", "Попередження");
            }
               
        }

        private bool AreGroupsFilled()
        {
            if (GroupVerticalPanel.Children.Count != 0)
            {
                foreach (StackPanel child in GroupVerticalPanel.Children)
                {
                    foreach (object _child in child.Children)
                    {
                        if (_child.GetType().Name == "ComboBox")
                        {
                            ComboBox cb = (ComboBox)_child;
                            if (cb.SelectedItem == null)
                                return false;
                            break;
                        }
                    }
                }
                return true;
            }
            return false;
        }
    }
}
