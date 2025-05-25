using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace HospitalWpf
{
    public partial class MainWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiBaseUrl = "https://localhost:7112";
        private readonly JsonSerializerOptions _jsonOptions;

        public MainWindow()
        {
            InitializeComponent();
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            AppointmentHourComboBox.SelectedIndex = 0; // 08:00
            AppointmentMinuteComboBox.SelectedIndex = 0; // 00

            LoadData();
        }

        private async Task LoadData()
        {
            try
            {
                PatientsDataGrid.ItemsSource = await GetData<List<Patient>>("api/patients");
                DoctorsDataGrid.ItemsSource = await GetData<List<Doctor>>("api/doctors");
                AppointmentsDataGrid.ItemsSource = await GetData<List<Appointment>>("api/appointments");
                ContactsDataGrid.ItemsSource = await GetData<List<Contact>>("api/contacts");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
            }
        }

        private async Task<T> GetData<T>(string endpoint)
        {
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        // Patient methods
        private async void AddPatientButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var patient = new Patient
                {
                    FullName = PatientNameTextBox.Text,
                    BirthDate = PatientBirthDatePicker.SelectedDate ?? DateTime.Now
                };
                var content = new StringContent(JsonSerializer.Serialize(patient, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/patients", content);
                response.EnsureSuccessStatusCode();
                await LoadData();
                ClearPatientForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void UpdatePatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                try
                {
                    selectedPatient.FullName = PatientNameTextBox.Text;
                    selectedPatient.BirthDate = PatientBirthDatePicker.SelectedDate ?? DateTime.Now;
                    var content = new StringContent(JsonSerializer.Serialize(selectedPatient, _jsonOptions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"api/patients/{selectedPatient.Id}", content);
                    response.EnsureSuccessStatusCode();
                    await LoadData();
                    ClearPatientForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to update.");
            }
        }

        private async void DeletePatientButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                var result = MessageBox.Show($"Are you sure you want to delete patient '{selectedPatient.FullName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/patients/{selectedPatient.Id}");
                        response.EnsureSuccessStatusCode();
                        await LoadData();
                        ClearPatientForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a patient to delete.");
            }
        }

        private void PatientsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (PatientsDataGrid.SelectedItem is Patient selectedPatient)
            {
                PatientNameTextBox.Text = selectedPatient.FullName;
                PatientBirthDatePicker.SelectedDate = selectedPatient.BirthDate;
            }
        }

        private void ClearPatientForm()
        {
            PatientNameTextBox.Clear();
            PatientBirthDatePicker.SelectedDate = null;
            PatientsDataGrid.SelectedItem = null;
        }

        private void ClearPatientForm_Click(object sender, RoutedEventArgs e)
        {
            ClearPatientForm();
        }

        // Doctor methods
        private async void AddDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var doctor = new Doctor
                {
                    FullName = DoctorNameTextBox.Text,
                    Specialty = DoctorSpecialtyTextBox.Text
                };
                var content = new StringContent(JsonSerializer.Serialize(doctor, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/doctors", content);
                response.EnsureSuccessStatusCode();
                await LoadData();
                ClearDoctorForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void UpdateDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorsDataGrid.SelectedItem is Doctor selectedDoctor)
            {
                try
                {
                    selectedDoctor.FullName = DoctorNameTextBox.Text;
                    selectedDoctor.Specialty = DoctorSpecialtyTextBox.Text;
                    var content = new StringContent(JsonSerializer.Serialize(selectedDoctor, _jsonOptions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"api/doctors/{selectedDoctor.Id}", content);
                    response.EnsureSuccessStatusCode();
                    await LoadData();
                    ClearDoctorForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor to update.");
            }
        }

        private async void DeleteDoctorButton_Click(object sender, RoutedEventArgs e)
        {
            if (DoctorsDataGrid.SelectedItem is Doctor selectedDoctor)
            {
                var result = MessageBox.Show($"Are you sure you want to delete doctor '{selectedDoctor.FullName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/doctors/{selectedDoctor.Id}");
                        response.EnsureSuccessStatusCode();
                        await LoadData();
                        ClearDoctorForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a doctor to delete.");
            }
        }

        private void DoctorsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DoctorsDataGrid.SelectedItem is Doctor selectedDoctor)
            {
                DoctorNameTextBox.Text = selectedDoctor.FullName;
                DoctorSpecialtyTextBox.Text = selectedDoctor.Specialty;
            }
        }

        private void ClearDoctorForm()
        {
            DoctorNameTextBox.Clear();
            DoctorSpecialtyTextBox.Clear();
            DoctorsDataGrid.SelectedItem = null;
        }

        private void ClearDoctorForm_Click(object sender, RoutedEventArgs e)
        {
            ClearDoctorForm();
        }

        // Appointment methods
        private async void AddAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(AppointmentPatientIdTextBox.Text, out int patientId) ||
                    !int.TryParse(AppointmentDoctorIdTextBox.Text, out int doctorId))
                {
                    MessageBox.Show("Please enter valid Patient ID and Doctor ID.");
                    return;
                }

                var appointmentDateTime = GetAppointmentDateTime();

                var appointment = new Appointment
                {
                    PatientId = patientId,
                    DoctorId = doctorId,
                    AppointmentDate = appointmentDateTime
                };
                var content = new StringContent(JsonSerializer.Serialize(appointment, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/appointments", content);
                response.EnsureSuccessStatusCode();
                await LoadData();
                ClearAppointmentForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void UpdateAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsDataGrid.SelectedItem is Appointment selectedAppointment)
            {
                try
                {
                    if (!int.TryParse(AppointmentPatientIdTextBox.Text, out int patientId) ||
                        !int.TryParse(AppointmentDoctorIdTextBox.Text, out int doctorId))
                    {
                        MessageBox.Show("Please enter valid Patient ID and Doctor ID.");
                        return;
                    }

                    var appointmentDateTime = GetAppointmentDateTime();

                    selectedAppointment.PatientId = patientId;
                    selectedAppointment.DoctorId = doctorId;
                    selectedAppointment.AppointmentDate = appointmentDateTime;
                    var content = new StringContent(JsonSerializer.Serialize(selectedAppointment, _jsonOptions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"api/appointments/{selectedAppointment.Id}", content);
                    response.EnsureSuccessStatusCode();
                    await LoadData();
                    ClearAppointmentForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select an appointment to update.");
            }
        }

        private async void DeleteAppointmentButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppointmentsDataGrid.SelectedItem is Appointment selectedAppointment)
            {
                var result = MessageBox.Show("Are you sure you want to delete this appointment?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/appointments/{selectedAppointment.Id}");
                        response.EnsureSuccessStatusCode();
                        await LoadData();
                        ClearAppointmentForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select an appointment to delete.");
            }
        }

        private void AppointmentsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AppointmentsDataGrid.SelectedItem is Appointment selectedAppointment)
            {
                AppointmentPatientIdTextBox.Text = selectedAppointment.PatientId.ToString();
                AppointmentDoctorIdTextBox.Text = selectedAppointment.DoctorId.ToString();
                AppointmentDatePicker.SelectedDate = selectedAppointment.AppointmentDate.Date;
                SetAppointmentTime(selectedAppointment.AppointmentDate);
            }
        }

        private void ClearAppointmentForm()
        {
            AppointmentPatientIdTextBox.Clear();
            AppointmentDoctorIdTextBox.Clear();
            AppointmentDatePicker.SelectedDate = null;
            AppointmentHourComboBox.SelectedIndex = 0;
            AppointmentMinuteComboBox.SelectedIndex = 0;
            AppointmentsDataGrid.SelectedItem = null;
        }

        private void ClearAppointmentForm_Click(object sender, RoutedEventArgs e)
        {
            ClearAppointmentForm();
        }

        private DateTime GetAppointmentDateTime()
        {
            var date = AppointmentDatePicker.SelectedDate ?? DateTime.Now.Date;

            var hourText = (AppointmentHourComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "08";
            var minuteText = (AppointmentMinuteComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "00";

            if (int.TryParse(hourText, out int hour) && int.TryParse(minuteText, out int minute))
            {
                return new DateTime(date.Year, date.Month, date.Day, hour, minute, 0);
            }

            return date;
        }

        private void SetAppointmentTime(DateTime appointmentDate)
        {
            var hourString = appointmentDate.Hour.ToString("D2");
            for (int i = 0; i < AppointmentHourComboBox.Items.Count; i++)
            {
                if ((AppointmentHourComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == hourString)
                {
                    AppointmentHourComboBox.SelectedIndex = i;
                    break;
                }
            }

            var minuteString = appointmentDate.Minute.ToString("D2");
            for (int i = 0; i < AppointmentMinuteComboBox.Items.Count; i++)
            {
                if ((AppointmentMinuteComboBox.Items[i] as ComboBoxItem)?.Content?.ToString() == minuteString)
                {
                    AppointmentMinuteComboBox.SelectedIndex = i;
                    break;
                }
            }
        }

        // Contact methods
        private async void AddContactButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var contact = new Contact { Phone = ContactPhoneTextBox.Text };
                var content = new StringContent(JsonSerializer.Serialize(contact, _jsonOptions), Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/contacts", content);
                response.EnsureSuccessStatusCode();
                await LoadData();
                ClearContactForm();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        private async void UpdateContactButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is Contact selectedContact)
            {
                try
                {
                    selectedContact.Phone = ContactPhoneTextBox.Text;
                    var content = new StringContent(JsonSerializer.Serialize(selectedContact, _jsonOptions), Encoding.UTF8, "application/json");
                    var response = await _httpClient.PutAsync($"api/contacts/{selectedContact.Id}", content);
                    response.EnsureSuccessStatusCode();
                    await LoadData();
                    ClearContactForm();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select a contact to update.");
            }
        }

        private async void DeleteContactButton_Click(object sender, RoutedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is Contact selectedContact)
            {
                var result = MessageBox.Show($"Are you sure you want to delete contact '{selectedContact.Phone}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    try
                    {
                        var response = await _httpClient.DeleteAsync($"api/contacts/{selectedContact.Id}");
                        response.EnsureSuccessStatusCode();
                        await LoadData();
                        ClearContactForm();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error: {ex.Message}");
                    }
                }
            }
            else
            {
                MessageBox.Show("Please select a contact to delete.");
            }
        }

        private void ContactsDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ContactsDataGrid.SelectedItem is Contact selectedContact)
            {
                ContactPhoneTextBox.Text = selectedContact.Phone;
            }
        }

        private void ClearContactForm()
        {
            ContactPhoneTextBox.Clear();
            ContactsDataGrid.SelectedItem = null;
        }

        private void ClearContactForm_Click(object sender, RoutedEventArgs e)
        {
            ClearContactForm();
        }

        protected override void OnClosed(EventArgs e)
        {
            _httpClient?.Dispose();
            base.OnClosed(e);
        }
    }

    public class Patient
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Specialty { get; set; }
    }

    public class Appointment
    {
        public int Id { get; set; }
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }

    public class Contact
    {
        public int Id { get; set; }
        public string Phone { get; set; }
    }
}