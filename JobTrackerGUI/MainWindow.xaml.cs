using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using JobTracker.Data;
using JobTracker.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

using AppModel = JobTracker.Models.Application;

namespace JobTrackerGUI
{
    public partial class MainWindow : Window
    {
        private readonly ApplicationService _service;
        private List<AppModel> _applications = new();

        public MainWindow()
        {
            InitializeComponent();

            var dbPath = System.IO.Path.Combine(AppContext.BaseDirectory, "jobtracker.db");
            var db = new JobTrackerContext(dbPath);

            db.Database.Migrate();

            _service = new ApplicationService(db);

            LoadApplications();
        }

        private void LoadApplications()
        {
            _applications = _service.GetAllApplications();

            ApplicationsList.ItemsSource = _applications.Select(a => new
            {
                a.Id,
                DisplayName = $"{a.Company} - {a.PositionTitle} ({a.Status})"
            }).ToList();

            NotesList.ItemsSource = null;
            ContactsList.ItemsSource = null;
        }

        private void ApplicationsList_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ApplicationsList.SelectedIndex < 0) return;

            var selectedId = _applications[ApplicationsList.SelectedIndex].Id;
            var app = _applications.First(a => a.Id == selectedId);

            NotesList.ItemsSource = app.Notes.Select(n => $"{n.CreatedAt:g}: {n.Content}").ToList();
            ContactsList.ItemsSource = app.Contacts.Select(c => $"{c.Name} ({c.Role}) {c.Email}").ToList();
        }

        private void AddApplication_Click(object sender, RoutedEventArgs e)
        {
            var input = Interaction.InputBox(
                "Enter company, position, status (0:Applied,1:Interviewing,2:Offer,3:Rejected,4:Withdrawn) separated by commas:",
                "Add Application"
            );

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(',');
            if (parts.Length < 3) return;

            var company = parts[0].Trim();
            var position = parts[1].Trim();
            if (!int.TryParse(parts[2].Trim(), out int statusInt)) return;

            _service.AddApplication(company, position, (JobTracker.Models.ApplicationStatus)statusInt);
            LoadApplications();
        }

        private void AddNote_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsList.SelectedIndex < 0) return;

            var appId = _applications[ApplicationsList.SelectedIndex].Id;
            var note = Interaction.InputBox("Enter note content:", "Add Note");
            if (string.IsNullOrWhiteSpace(note)) return;

            _service.AddNote(appId, note);
            LoadApplications();
        }

        private void AddContact_Click(object sender, RoutedEventArgs e)
        {
            if (ApplicationsList.SelectedIndex < 0) return;

            var appId = _applications[ApplicationsList.SelectedIndex].Id;
            var input = Interaction.InputBox(
                "Enter contact name, role, email (optional) separated by commas:",
                "Add Contact"
            );

            if (string.IsNullOrWhiteSpace(input)) return;

            var parts = input.Split(',');
            if (parts.Length < 2) return;

            var name = parts[0].Trim();
            var role = parts[1].Trim();
            var email = parts.Length >= 3 ? parts[2].Trim() : null;

            _service.AddContact(appId, name, role, string.IsNullOrWhiteSpace(email) ? null : email);
            LoadApplications();
        }
    }
}