using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JobTracker.Data;
using JobTracker.Services;
using Microsoft.EntityFrameworkCore;
using AppModel = JobTracker.Models.Application;

namespace JobTrackerGUI
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationService _service;
        private AppModel? _selectedApplication;
        private List<AppModel> _applications = new();

        public MainWindow()
        {
            InitializeComponent();

            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "jobtracker.db");
            var db = new JobTrackerContext(dbPath);

            db.Database.Migrate();

            _service = new ApplicationService(db);

            StatusEditor.ItemsSource = Enum.GetValues(typeof(JobTracker.Models.ApplicationStatus));

            LoadApplications();
        }

        private void LoadApplications()
        {
            var selectedApp = ApplicationsList.SelectedItem as AppModel;

            _applications = _service.GetAllApplications();

            ApplicationsList.ItemsSource = _applications;

            if (selectedApp != null)
            {
                var restored = _applications.FirstOrDefault(a => a.Id == selectedApp.Id);
                ApplicationsList.SelectedItem = restored;
            }

            if (ApplicationsList.SelectedItem == null)
            {
                _selectedApplication = null;
                StatusEditor.SelectedItem = null;
                NotesList.ItemsSource = null;
                ContactsList.ItemsSource = null;
            }
        }

        private AppModel? GetSelectedApplication()
        {
            return ApplicationsList.SelectedItem as AppModel;
        }

        private void StatusEditor_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (_selectedApplication == null || StatusEditor.SelectedItem == null)
                return;

            var newStatus = (JobTracker.Models.ApplicationStatus)StatusEditor.SelectedItem;
            if (_selectedApplication.Status == newStatus)
                return;

            _service.UpdateStatus(_selectedApplication.Id, newStatus);
            LoadApplications();
        }

        private void ApplicationsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ApplicationsList.SelectedItem is not AppModel app)
                return;

            _selectedApplication = app;

            StatusEditor.SelectionChanged -= StatusEditor_SelectionChanged;
            StatusEditor.SelectedItem = app.Status;
            StatusEditor.SelectionChanged += StatusEditor_SelectionChanged;

            NotesList.ItemsSource = app.Notes;
            ContactsList.ItemsSource = app.Contacts;
        }

        private void AddApplication_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new AddApplicationDialog { Owner = this };

            if (dialog.ShowDialog() == true)
            {
                _service.AddApplication(dialog.Company, dialog.Position, dialog.SelectedStatus);
                LoadApplications();
            }
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            var app = GetSelectedApplication();
            if (app == null) return;

            var note = Microsoft.VisualBasic.Interaction.InputBox("Enter note content:", "Add Note");
            if (string.IsNullOrWhiteSpace(note)) return;

            _service.AddNote(app.Id, note);
            LoadApplications();
        }

        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            var note = (sender as System.Windows.Controls.MenuItem)?.CommandParameter as JobTracker.Models.Note;
            if (_selectedApplication == null || note == null) return;

            var input = Microsoft.VisualBasic.Interaction.InputBox("Edit note content:", "Edit Note", note.Content);
            if (string.IsNullOrWhiteSpace(input)) return;

            _service.UpdateNote(note.Id, input);
            LoadApplications();
        }

        private void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            var note = (sender as System.Windows.Controls.MenuItem)?.CommandParameter as JobTracker.Models.Note;
            if (_selectedApplication == null || note == null) return;

            if (MessageBox.Show("Delete this note?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _service.DeleteNote(note.Id);
                LoadApplications();
            }
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            var app = GetSelectedApplication();
            if (app == null) return;

            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Enter contact name, role, email (optional) separated by commas:",
                "Add Contact");

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(',');
            if (parts.Length < 2) return;

            var name = parts[0].Trim();
            var role = parts[1].Trim();
            var email = parts.Length >= 3 ? parts[2].Trim() : null;

            _service.AddContact(app.Id, name, role, string.IsNullOrWhiteSpace(email) ? null : email);
            LoadApplications();
        }

        private void EditContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = (sender as System.Windows.Controls.MenuItem)?.CommandParameter as JobTracker.Models.Contact;
            if (_selectedApplication == null || contact == null) return;

            var input = Microsoft.VisualBasic.Interaction.InputBox(
                "Edit contact (format: Name,Role,Email):",
                "Edit Contact",
                $"{contact.Name},{contact.Role},{contact.Email}");

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(',');
            if (parts.Length < 2) return;

            var name = parts[0].Trim();
            var role = parts[1].Trim();
            var email = parts.Length >= 3 ? parts[2].Trim() : null;

            _service.UpdateContact(contact.Id, name, role, email);
            LoadApplications();
        }

        private void DeleteContact_Click(object sender, RoutedEventArgs e)
        {
            var contact = (sender as System.Windows.Controls.MenuItem)?.CommandParameter as JobTracker.Models.Contact;
            if (_selectedApplication == null || contact == null) return;

            if (MessageBox.Show("Delete this contact?", "Confirm Delete", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                _service.DeleteContact(contact.Id);
                LoadApplications();
            }
        }
    }
}