using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;

namespace HospitalWpf
{
    public partial class LoginWindow : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private readonly string _apiBaseUrl = "https://localhost:7112";
        private readonly JsonSerializerOptions _jsonOptions;

        public LoginWindow()
        {
            InitializeComponent();
            _httpClient.BaseAddress = new Uri(_apiBaseUrl);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var loginModel = new
                {
                    username = UsernameTextBox.Text.Trim(),
                    password = PasswordBox.Password.Trim()
                };
                var json = JsonSerializer.Serialize(loginModel, _jsonOptions);
                Console.WriteLine("Sending JSON: " + json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("api/auth/login", content);

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response: " + responseContent);

                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Успешный вход!");
                    var mainWindow = new MainWindow();
                    mainWindow.Show();
                    Close();
                }
                else
                {
                    MessageBox.Show("Неверные данные");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _httpClient?.Dispose();
            base.OnClosed(e);
        }
    }
}