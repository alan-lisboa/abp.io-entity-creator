using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator
{
    public class LocalizationUpdater(string @namespace, string path)
    {
        private string entityName;
        private List<PropertyModel> properties;
        private string projectName;
        private string groupName;
        private string folder;
        
        public bool Update(string entityName, List<PropertyModel> properties)
        {
            this.entityName = entityName.Dehumanize();
            this.properties = properties;

            projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
            groupName = entityName.Pluralize();
            folder = $"{path}\\src\\{@namespace}.Domain.Shared\\Localization\\{projectName}";

            if (!Directory.Exists(folder))
                return false;

            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                UpdateLocaliztionFile(file);
            }

            return true;
        }

        private void UpdateLocaliztionFile(string file)
        {
            if (!File.Exists(file))
                return;

            StringBuilder stringBuilder = new();

            using var reader = new StreamReader(file);
            var line = reader.ReadLine();

            bool isSession = false;
            string indent = new (' ', 4);

            while (line != null)
            {
                if (line.Contains("\"texts\":"))
                {
                    isSession = true;

                    stringBuilder
                        .AppendLine(line);

                    line = reader.ReadLine();

                    continue;
                }

                if (!line.TrimEnd().EndsWith('}') && !line.TrimEnd().EndsWith(',') && isSession)
                {
                    stringBuilder
                        .Append(line)
                        .AppendLine(",");

                    line = reader.ReadLine();

                    continue;
                }

                if (line.TrimEnd().EndsWith('}') && isSession)
                {
                    isSession = false;

                    stringBuilder
                        .Append($"{indent}\"Permission:{entityName}\": ")
                        .AppendLine($"\"{entityName}\",");

                    stringBuilder
                        .Append($"{indent}\"Menu:{entityName}\": ")
                        .AppendLine($"\"Menu{entityName}\",");

                    stringBuilder
                        .Append($"{indent}\"{entityName}\": ")
                        .AppendLine($"\"{entityName}\",");

                    foreach (var property in properties)
                    {
                        if (property.IsCollection ||
                            property.Type == "Entity" ||
                            property.Type == "ValueObject" ||
                            property.Type == "AggregatedRoot")
                            continue;

                        stringBuilder
                            .Append($"{indent}\"{entityName}{property.Name}\": ")
                            .AppendLine($"\"{entityName}{property.Name}\",");
                    }

                    stringBuilder
                        .Append($"{indent}\"Create{entityName}\": ")
                        .AppendLine($"\"Create{entityName}\",");

                    stringBuilder
                        .Append($"{indent}\"Edit{entityName}\": ")
                        .AppendLine($"\"Edit{entityName}\",");

                    stringBuilder
                        .Append($"{indent}\"{entityName}DeletionConfirmationMessage\": ")
                        .Append($"\"Are you sure to delete the {entityName}")
                        .AppendLine(" {0}?\"");
                }

                stringBuilder.AppendLine(line);
                
                line = reader.ReadLine();
            }

            reader.Close();

            File.WriteAllText(file, stringBuilder.ToString());
        }
    }
}
