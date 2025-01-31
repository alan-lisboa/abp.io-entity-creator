using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class AppMapperUpdater(EntityModel entity)
{
    public bool Update()
    {
        string artifactName = $"{entity.ProjectName}ApplicationAutoMapperProfile";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Application";
        string filename = $"{folder}\\{artifactName}.cs";
        string entityDto = $"{entity.Name}Dto";
        string createUpdateDto = $"CreateUpdate{entity.Name}Dto";

        if (!File.Exists(filename))
            return false;

        bool added = false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        while (line != null)
        {
            if (line.Contains("using AutoMapper;"))
            {
                stringBuilder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;");
            }

            if (line.Contains('}') && !added)
            {
                added = true;

                stringBuilder.AppendLine();

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{entity.Name}, {entityDto}>")
                    .AppendLine("();");

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{createUpdateDto}, {entity.Name}>")
                    .AppendLine("(MemberList.Source);");
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
