using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class PermissionUpdater(EntityModel entity)
{
    public bool Update()
    {
        string artifactName = $"{entity.ProjectName}Permissions";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Permissions";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        while (line != null)
        {
            if (line.TrimEnd() == "}")
            {
                stringBuilder
                    .AppendLine()
                    .Append("\tpublic static class ")
                    .AppendLine(entity.Name)
                    .AppendLine("\t{");
                    
                stringBuilder
                    .Append("\t\tpublic const string Default = GroupName + \".")
                    .Append(entity.Name)
                    .AppendLine("\";");

                stringBuilder.AppendLine("\t\tpublic const string Create = Default + \".Create\";");
                stringBuilder.AppendLine("\t\tpublic const string Edit = Default + \".Edit\";");
                stringBuilder.AppendLine("\t\tpublic const string Delete = Default + \".Delete\";");
                stringBuilder.AppendLine("\t}");
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
