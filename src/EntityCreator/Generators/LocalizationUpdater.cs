using EntityCreator.Helpers;
using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators
{
    public class LocalizationUpdater(EntityModel entity) : BaseGenerator
    {
        public override bool Handle()
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

            Initialize();

            var language = file
                .Replace(folder!, "")
                .Replace("\\", "")
                .Replace(".json", "");

            var definition = entity.Localizations?
                .FirstOrDefault(x => x.Language == language);

            using var reader = new StreamReader(file);
            var line = reader.ReadLine();

            bool isSession = false;
            string indent = new (' ', 4);

            while (line != null)
            {
                if (line.Contains("\"texts\":"))
                {
                    isSession = true;
                    builder.AppendLine(line);
                    line = reader.ReadLine();

                    continue;
                }

                if (!line.TrimEnd().EndsWith('}') && !line.TrimEnd().EndsWith(',') && isSession)
                {
                    builder
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
                    var editEntity = $"{definition?.Edit} {definition?.EntityName}";
                    var deletionMessage = $"{definition?.DeletionMessage}";

                    if (string.IsNullOrEmpty(createEntity.Trim()))
                        createEntity = "Create";

                    if (string.IsNullOrEmpty(editEntity.Trim()))
                        editEntity = "Edit";
                    
                    if (string.IsNullOrEmpty(deletionMessage.Trim()))
                        deletionMessage = $"Are you sure to delete the {entity.Name} " + "{0}?";

                    builder
                        .Append($"{indent}\"Permission:{entity.Name}\": ")
                        .AppendLine($"\"{entityName}\",");

                    builder
                        .Append($"{indent}\"Permission:{entity.Name}.Create\": ")
                        .AppendLine($"\"{permissionCreate}\",");
                    
                    builder
                        .Append($"{indent}\"Permission:{entity.Name}.Edit\": ")
                        .AppendLine($"\"{permissionEdit}\",");
                    
                    builder
                        .Append($"{indent}\"Permission:{entity.Name}.Delete\": ")
                        .AppendLine($"\"{permissionDelete}\",");

                    builder
                        .Append($"{indent}\"Menu:{entity.Name}\": ")
                        .AppendLine($"\"{menuEntry}\",");

                    builder
                        .Append($"{indent}\"{entity.Name}\": ")
                        .AppendLine($"\"{entityName}\",");

                    foreach (var property in entity.Properties!)
                    {
                        if (property.IsCollection || BaseTypeHelper.IsEntityType(property.Type!))
                            continue;

                        var propertyDefinition = definition?.Properties?.Find(x => x.Property == property.Name);
                        var propertyName = propertyDefinition?.Translation ?? property.Name;

                        builder
                            .Append($"{indent}\"{entity.Name}{property.Name}\": ")
                            .AppendLine($"\"{propertyName}\",");
                    }

                    builder
                        .Append($"{indent}\"Create{entity.Name}\": ")
                        .AppendLine($"\"{createEntity}\",");

                    builder
                        .Append($"{indent}\"Edit{entity.Name}\": ")
                        .AppendLine($"\"{editEntity}\",");

                    builder
                        .Append($"{indent}\"{entity.Name}DeletionConfirmationMessage\": ")
                        .AppendLine($"\"{deletionMessage}\"");
                }

                builder.AppendLine(line);
                
                line = reader.ReadLine();
            }

            reader.Close();

            WriteToFile(file);
        }
    }
}
