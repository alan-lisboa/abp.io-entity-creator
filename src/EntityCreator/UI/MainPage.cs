using EntityCreator.Models;
using Microsoft.Web.WebView2.Core;
using PeanutButter.INI;
using System.Text.Json;

namespace EntityCreator.Forms
{
    public partial class MainPage : Form
    {
        private JsonSerializerOptions options = new() { AllowTrailingCommas = true, PropertyNameCaseInsensitive = true };

        public MainPage()
        {
            InitializeComponent();
            InitializeAsync();
        }

        async void InitializeAsync()
        {
            string page = string.Format("{0}Resources/html/MainPage.html",
                AppDomain.CurrentDomain.BaseDirectory);
            
            await webView2.EnsureCoreWebView2Async(null);
            webView2.Source = new Uri(page);


            webView2.WebMessageReceived += WebView2_WebMessageReceived;
        }

        private void WebView2_WebMessageReceived(object? sender, CoreWebView2WebMessageReceivedEventArgs e)
        {
            var messageReceived = e.TryGetWebMessageAsString();
            var message = JsonSerializer.Deserialize<WebView2Message>(messageReceived, options);
            
            CommandExecutor commandExecutor = new();
            var messageToSend = commandExecutor.ProcessMessage(message);

            if (messageToSend != null)
                webView2.CoreWebView2.PostWebMessageAsString(JsonSerializer.Serialize(messageToSend));
        }
    }
}
