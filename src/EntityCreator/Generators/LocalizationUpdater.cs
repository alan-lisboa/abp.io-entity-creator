using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators
{
    public class LocalizationUpdater(EntityModel entity)
    {
        private string? folder;
        
        public bool Update()
        {
            folder = $"{entity.Location}\\src\\{entity.Namespace}.Domain.Shared\\Localization\\{entity.ProjectName}";

            if (!Directory.Exists(folder))
                return false;

            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                UpdateLocalizationFile(file);
            }

            return true;
        }

        private void UpdateLocalizationFile(string file)
        {
            if (!File.Exists(file))
                return;

            var lang = file
                .Replace(folder!, "")
                .Replace("\\", "")
                .Replace(".json", "");

            var definition = entity.Localizations?
                .FirstOrDefault(x => x.Language == lang);

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

                    var entityName = definition?.EntityName ?? entity.Name;
                    var permissionCreate = definition?.Create ?? "Create";
                    var permissionEdit = definition?.Edit ?? "Edit";
                    var permissionDelete = definition?.Delete ?? "Delete";
                    var menuEntry = definition?.PluralizedName ?? entity.Pluralized;
                    var createEntity = $"{definition?.Create} {definition?.EntityName}";
                    if (string.IsNullOrEmpty(createEntity.Trim()))
                        createEntity = "Create";

                    var editEntity = $"{definition?.Edit} {definition?.EntityName}";
                    if (string.IsNullOrEmpty(editEntity.Trim()))
                        editEntity = "Edit";

                    var deletionMessage = $"{definition?.DeletionMessage}";
                    if (string.IsNullOrEmpty(deletionMessage.Trim()))
                        deletionMessage = $"Are you sure to delete the {entity.Name} " + "{0}?";

                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}\": ")
                        .AppendLine($"\"{entityName}\",");

                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Create\": ")
                        .AppendLine($"\"{permissionCreate}\",");
                    
                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Edit\": ")
                        .AppendLine($"\"{permissionEdit}\",");
                    
                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Delete\": ")
                        .AppendLine($"\"{permissionDelete}\",");

                    stringBuilder
                        .Append($"{indent}\"Menu:{entity.Name}\": ")
                        .AppendLine($"\"{menuEntry}\",");

                    stringBuilder
                        .Append($"{indent}\"{entity.Name}\": ")
                        .AppendLine($"\"{entityName}\",");

                    foreach (var property in entity.Properties!)
                    {
                        if (property.IsCollection ||
                            property.Type == "Entity" ||
                            property.Type == "ValueObject" ||
                            property.Type == "AggregateRoot")
                            continue;

                        var propertyDefinition = definition?.Properties?.Find(x => x.Property == property.Name);
                        var propertyName = propertyDefinition?.Translation ?? property.Name;

                        stringBuilder
                            .Append($"{indent}\"{entity.Name}{property.Name}\": ")
                            .AppendLine($"\"{propertyName}\",");
                    }

                    stringBuilder
                        .Append($"{indent}\"Create{entity.Name}\": ")
                        .AppendLine($"\"{createEntity}\",");

                    stringBuilder
                        .Append($"{indent}\"Edit{entity.Name}\": ")
                        .AppendLine($"\"{editEntity}\",");

                    stringBuilder
                        .Append($"{indent}\"{entity.Name}DeletionConfirmationMessage\": ")
                        .AppendLine($"\"{deletionMessage}\"");
                }

                stringBuilder.AppendLine(line);
                
                line = reader.ReadLine();
            }

            reader.Close();

            File.WriteAllText(file, stringBuilder.ToString());
        }
    }
}
