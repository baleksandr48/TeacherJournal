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

        }
    }
}
