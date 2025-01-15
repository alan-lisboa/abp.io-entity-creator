using Humanizer;
using System.Text;

namespace EntityCreator;

public class AppMapperUpdater(string @namespace, string path)
{
    public bool Update(string entityName)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{projectName}ApplicationAutoMapperProfile";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application";
        string filename = $"{folder}\\{artifactName}.cs";
        string entityDto = $"{entityName}Dto";
        string createDto = $"CreateUpdate{entityName}Dto";

        if (!File.Exists(filename))
            return false;

        bool added = false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        while (line != null)
        {
            if (line.Contains('}') && !added)
            {
                added = true;

                stringBuilder.AppendLine();

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{entityName}, {entityDto}>")
                    .AppendLine("();");

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{createDto}, {entityName}>")
                    .AppendLine("();");

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{entityDto}, {createDto}>")
                    .AppendLine("();");
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine();
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
