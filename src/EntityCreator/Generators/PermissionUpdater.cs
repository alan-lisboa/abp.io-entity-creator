using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class PermissionUpdater(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"{entity.ProjectName}Permissions";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Permissions";
        filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        while (line != null)
        {
            if (line.TrimEnd() == "}")
            {
                indentationLevel++;

                builder
                    .AppendLine();

                builder
                    .Append(Indentation)
                    .Append("public static class ")
                    .AppendLine(entity.Name);

                builder
                    .Append(Indentation)
                    .AppendLine("{");

                indentationLevel++;

                builder
                    .Append(Indentation)
                    .Append("public const string Default = GroupName + \".")
                    .Append(entity.Name)
                    .AppendLine("\";");

                builder
                    .Append(Indentation)
                    .AppendLine("public const string Create = Default + \".Create\";");

                builder
                    .Append(Indentation)
                    .AppendLine("public const string Edit = Default + \".Edit\";");

                builder
                    .Append(Indentation)
                    .AppendLine("public const string Delete = Default + \".Delete\";");

                indentationLevel--;

                builder
                    .Append(Indentation)
                    .AppendLine("}");
            }

            builder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        indentationLevel = 0;

        return WriteToFile();
    }
}
