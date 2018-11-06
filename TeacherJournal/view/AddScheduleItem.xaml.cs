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

namespace TeacherJournal.view
{
    /// <summary>
    /// Логика взаимодействия для AddScheduleItem.xaml
    /// </summary>
    public partial class AddScheduleItem : Window
    {
        public AddScheduleItem()
        {
            InitializeComponent();
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
    }
}
