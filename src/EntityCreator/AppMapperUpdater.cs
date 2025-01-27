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
        string createUpdateDto = $"CreateUpdate{entityName}Dto";

        if (!File.Exists(filename))
            return false;

        bool added = false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        while (line != null)
        {
            if (line.Contains("using AutoMapper;"))
            {
                stringBuilder
                    .AppendLine($"using {@namespace}.{groupName};")
                    .AppendLine($"using {@namespace}.{groupName}.Dtos;");
            }

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
                    .Append($"<{createUpdateDto}, {entityName}>")
                    .AppendLine("(MemberList.Source);");
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine();
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
