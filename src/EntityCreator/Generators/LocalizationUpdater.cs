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
                        .Append($"{indent}\"Permission:{entity.Name}\": ")
                        .AppendLine($"\"{entity.Name}\",");

                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Create\": ")
                        .AppendLine("\"Create\",");

                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Edit\": ")
                        .AppendLine("\"Edit\",");

                    stringBuilder
                        .Append($"{indent}\"Permission:{entity.Name}.Delete\": ")
                        .AppendLine("\"Delete\",");

                    stringBuilder
                        .Append($"{indent}\"Menu:{entity.Name}\": ")
                        .AppendLine($"\"{entity.Pluralized}\",");

                    stringBuilder
                        .Append($"{indent}\"{entity.Name}\": ")
                        .AppendLine($"\"{entity.Name}\",");

                    foreach (var property in entity.Properties!)
                    {
                        if (property.IsCollection ||
                            property.Type == "Entity" ||
                            property.Type == "ValueObject" ||
                            property.Type == "AggregateRoot")
                            continue;

                        stringBuilder
                            .Append($"{indent}\"{entity.Name}{property.Name}\": ")
                            .AppendLine($"\"{property.Name}\",");
                    }

                    stringBuilder
                        .Append($"{indent}\"Create{entity.Name}\": ")
                        .AppendLine($"\"Create\",");

                    stringBuilder
                        .Append($"{indent}\"Edit{entity.Name}\": ")
                        .AppendLine($"\"Edit\",");

                    stringBuilder
                        .Append($"{indent}\"{entity.Name}DeletionConfirmationMessage\": ")
                        .Append($"\"Are you sure to delete the {entity.Name}")
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
