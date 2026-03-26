using System;
using System.Windows;
using JobTracker.Models;

namespace JobTrackerGUI
{
    public partial class AddApplicationDialog : Window
    {
        public string Company => CompanyBox.Text;
        public string Position => PositionBox.Text;
        public ApplicationStatus SelectedStatus =>
            (ApplicationStatus)StatusBox.SelectedItem;

        public AddApplicationDialog()
        {
            InitializeComponent();

            StatusBox.ItemsSource =
                Enum.GetValues(typeof(ApplicationStatus));

            StatusBox.SelectedIndex = 0;
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Company) ||
                string.IsNullOrWhiteSpace(Position))
            {
                MessageBox.Show("Please fill out all fields.");
                return;
            }

            DialogResult = true;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}