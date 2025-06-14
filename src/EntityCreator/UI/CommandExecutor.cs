using EntityCreator.Generators;
using EntityCreator.Models;
using PeanutButter.INI;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EntityCreator.Forms
{
    public class CommandExecutor
    {
        public WebView2Message? ProcessMessage(WebView2Message? message)
        {
            if (message?.Command == "OpenSolutionFolder")
                return ProcessOpenSolutionFolderMessage();

            if (message?.Command == "CreateEntity")
                return ProcessCreateEntityMessage(message.Payload);

            if (message?.Command == "SaveTheme")
                return SaveTheme(message.Payload);

            if (message?.Command == "GetSettings")
                return GetSettings();

            return null;
        }

        private WebView2Message? ProcessOpenSolutionFolderMessage()
        {
            FolderBrowserDialog folderBrowserDialog = new();
            var result = folderBrowserDialog.ShowDialog();
            
            if (result != DialogResult.OK)
                return null;

            string path = folderBrowserDialog.SelectedPath;

            var files = Directory.GetFiles(path).Where(x => x.EndsWith(".sln"));

            if (!files.Any())
                return new WebView2Message("InvalidSolutionFolder", null);
            
            var file = Path.GetFileName(files.First());
            var project = string.Empty;
            var @namespace = string.Empty;

            if (!string.IsNullOrEmpty(file))
            {
                @namespace = file.Replace(".sln", "");
                int index = @namespace.IndexOf('.') + 1;
                project = @namespace[index..];
            }

            var paylod = new
            {
                ProjectName = project,
                Location = path,
                Namespace = @namespace
            };

            var ini = new INIFile("Settings.ini");
            ini.SetValue("Current", "Location", path);
            ini.Persist();

            return new WebView2Message("LoadSolutionData", paylod);
        }

        private WebView2Message? ProcessCreateEntityMessage(object payload)
        {
            try
            {
                var entityModel = JsonSerializer.Deserialize<EntityModel>(payload.ToString());
               
                EntityGenerator generator = new();
                bool result = EntityGenerator.Generate(entityModel!);
                if (!result)
                {
                    return new WebView2Message("EntityCreateFailed", 
                        JsonSerializer.Serialize(new { Message = "Entity creation failed." }));
                }
                
                return new WebView2Message("EntityCreatedSuccessfuly", null);
            }
            catch (Exception ex)
            {
                return new WebView2Message("EntityCreateFailed", 
                    JsonSerializer.Serialize(new { ex.Message }));
            }
        }

        private WebView2Message? SaveTheme(object payload)
        {
            try
            {
                var ini = new INIFile("Settings.ini");
                ini.SetValue("Current", "Theme", payload.ToString());
                ini.Persist();

                return new WebView2Message("ThemeChanged", null);
            }
            catch (Exception ex)
            {
                return new WebView2Message("ThemeChangeFailed", ex.Message);
            }
        }

        private WebView2Message? GetSettings()
        {
            var ini = new INIFile("Settings.ini");
            var theme = ini.GetValue("Current", "Theme", "dark");
            var location = ini.GetValue("Current", "Location", "");
            
            string file = string.Empty;
            string project = string.Empty;
            string @namespace = string.Empty;

            try
            {
                var files = Directory.GetFiles(location).Where(x => x.EndsWith(".sln"));
                if (files.Any())
                {
                    file = Path.GetFileName(files.First());

                    if (!string.IsNullOrEmpty(file))
                    {
                        @namespace = file.Replace(".sln", "");
                        int index = @namespace.IndexOf('.') + 1;
                        project = @namespace[index..];
                    }
                }
            }
            catch
            {
            }

            var payload = new
            {
                Theme = theme,
                ProjectName = project,
                Location = location,
                Namespace = @namespace
            };

            return new WebView2Message("LoadSettings", 
                JsonSerializer.Serialize(payload));
        }
    }
}
