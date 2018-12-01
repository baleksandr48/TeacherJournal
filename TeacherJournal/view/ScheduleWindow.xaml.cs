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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для ScheduleWindow.xaml
    /// </summary>
    public partial class ScheduleWindow : Window
    {
        private Term currentTerm;
        public ObservableCollection<Schedule> scheduleList;

        // Форма с progressbar.
        LoadingForm loadingForm;

        public ScheduleWindow()
        {
            InitializeComponent();
        }

        public ScheduleWindow(Term term)
        {
            InitializeComponent();
            this.currentTerm = term;

            scheduleList = new ObservableCollection<Schedule>(DBHelper.selectSchedules(currentTerm));
        }

        // После завершения инициализации всего окна выполняем привязку данных.
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScheduleGrid.ItemsSource = scheduleList;
        }

        // Используем данные с таблицы чтобы заполнить таблицу Lesson в бд.
        private void btnFillSchedule_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Ви підтверджуєте прийняття розкладу?\nУ разі, якщо існує розклад на цей семестр, " +
                "він буде перезаписаний новими даними.", "Підтвердження", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                BackgroundWorker bg = new BackgroundWorker();
                bg.DoWork += new DoWorkEventHandler(bg_DoWork);
                bg.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bg_RunWorkerCompleted);
                // Запуск worker.
                bg.RunWorkerAsync();
                // Отображаем loading form.
                loadingForm = new LoadingForm("Заповнення журналу занять");
                loadingForm.ShowDialog();
            }
        }

        private void bg_DoWork(object sender, DoWorkEventArgs e)
        {
            // Очистка расписания и занятий. Создание новых данных по занятиям и расписанию.
            try
            {
                DBHelper.ClearSchedule(currentTerm);
                DBHelper.ClearLesson(currentTerm);
                DBHelper.addSchedules(scheduleList.ToList());
                DBHelper.addLessons(Lesson.generateLessons(scheduleList.ToList(), currentTerm));
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        private void bg_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            // Закрыть loading form и это окно.
            loadingForm.Close();
            this.DialogResult = true;
        }

        // Добавляем объект Schedule.
        private void btnAddNewScheduleRow_Click(object sender, RoutedEventArgs e)
        {
            ScheduleItemWindow addScheduleItem = new ScheduleItemWindow(currentTerm, this);
            try
            {
                addScheduleItem.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception cought", ex);
            }
        }

        // Удаляем объект Schedule со scheduleList.
        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Schedule obj = ((FrameworkElement)sender).DataContext as Schedule;
                scheduleList.Remove(obj);
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        // Редактируем объект Schedule.
        private void EditeRow_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Schedule obj = ((FrameworkElement)sender).DataContext as Schedule;
                ScheduleItemWindow window = new ScheduleItemWindow(currentTerm, this, obj);
                window.ShowDialog();
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception caught", ex);
            }
        }

        // Генерируем случайный объект Schedule и добавляем его в список.
        private void RandScheduleItem_Click(object sender, RoutedEventArgs e)
        {
            scheduleList.Add(DBHelper.AddRandomSchedule(currentTerm));
        }
    }
}
