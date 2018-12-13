﻿using System;
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
using System.ComponentModel;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для ExportWindow.xaml
    /// </summary>
    public partial class ExportWindow : Window
    {
        private List<Term> _termList;
        private Term _firstTerm;
        private Term _secondTerm;
        private Teacher teacher;
        private String _teacherFullName;
        private String _headFullName;
        private String _instute;
        private String _faculty;
        private String _department;
        private String _academic;
        private String _post;


        public LoadingForm loadingForm;

        public ExportWindow()
        {
            InitializeComponent();

            try
            {
                _termList = DBHelper.selectTerms();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception {0} cought", ex);
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (_termList.Count != 0)
            {
                cbFirstTerm.ItemsSource = _termList;
                cbSecondTerm.ItemsSource = _termList;
            }
        }

        private void AcceptAndConvert(object sender, RoutedEventArgs e)
        {
            teacher = DBHelper.getTeacher();
            _teacherFullName = teacher.fullName;
            _headFullName = teacher.headFullName;
            _instute = teacher.institute;
            _faculty = teacher.faculty;
            _department = teacher.department;
            _academic = teacher.academicRank;
            _post = teacher.post;

            _firstTerm = cbFirstTerm.SelectedItem as Term;
            _secondTerm = cbSecondTerm.SelectedItem as Term;

            if ((_firstTerm != null) && (_secondTerm != null))
            {
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(bg_DoWork);
                bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
                // Запуск worker.
                bg.RunWorkerAsync();
                // Отображаем loading form.
                loadingForm = new LoadingForm("Конвертація в Word DOC");
                loadingForm.ShowDialog();
            }
            else
            {
                MessageBox.Show("Заповніть всі необхідні поля!", "Попередження");
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            Export exportObject = new Export();
            exportObject.ConvertToWord(_firstTerm, _secondTerm, teacher);
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Закрыть loading form и это окно.
            loadingForm.WorkEnded = true;
            loadingForm.Close();
            this.DialogResult = true;
        }
    }
}
