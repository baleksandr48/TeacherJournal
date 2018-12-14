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
using TeacherJournal.model;
using TeacherJournal.database;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для TeacherInfoWindow.xaml
    /// </summary>
    public partial class TeacherInfoWindow : Window
    {
        private Teacher _teacher;

        public TeacherInfoWindow()
        {
            InitializeComponent();
            try
            {
                _teacher = DBHelper.getTeacher();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception {0} caught", ex);
            }
        }

        public TeacherInfoWindow(Teacher teacher)
        {
            InitializeComponent();
            _teacher = teacher;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tbAcademicRank.Text = _teacher.academicRank;
            tbDepartment.Text = _teacher.department;
            tbFaculty.Text = _teacher.faculty;
            tbHeadFullName.Text = _teacher.headFullName;
            tbInstitute.Text = _teacher.institute;
            tbPost.Text = _teacher.post;
            tbTeacherFullName.Text = _teacher.fullName;
        }

        private void btAcceptAndClose_Click(object sender, RoutedEventArgs e)
        {
            if ((tbAcademicRank.Text != "") && (tbDepartment.Text != "") && (tbFaculty.Text != "") && (tbHeadFullName.Text != "")
                && (tbInstitute.Text != "") && (tbPost.Text != "") && (tbTeacherFullName.Text != ""))
            {
                Teacher teacher = new Teacher();
                teacher.academicRank = tbAcademicRank.Text;
                teacher.department = tbAcademicRank.Text;
                teacher.faculty = tbFaculty.Text;
                teacher.headFullName = tbHeadFullName.Text;
                teacher.institute = tbInstitute.Text;
                teacher.post = tbPost.Text;
                teacher.fullName = tbTeacherFullName.Text;

                try
                {
                    DBHelper.updateTeacher(teacher);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception {0} caught", ex);
                }

                this.Close();
            }
            else
            {
                MessageBox.Show("Попередження", "Заповніть всі необхідні поля!");
            }
        }
    }
}
