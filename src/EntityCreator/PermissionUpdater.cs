using Humanizer;
using System.Text;

namespace EntityCreator;

public class PermissionUpdater(string @namespace, string path)
{
    public bool Update(string entityName)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{projectName}Permissions";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application.Contracts\\Permissions";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        
        while (line != null)
        {
            stringBuilder.AppendLine(line);

            if (line.Contains("public const string GroupName"))
            {
                stringBuilder
                    .AppendLine()
                    .Append("\tpublic static class ")
                    .AppendLine(groupName)
                    .AppendLine("\t{");
                    
                stringBuilder
                    .Append("\t\tpublic const string Default = GroupName + \".")
                    .Append(groupName)
                    .AppendLine("\";");

                stringBuilder.AppendLine("\t\tpublic const string Create = Default + \".Create\";");
                stringBuilder.AppendLine("\t\tpublic const string Edit = Default + \".Edit\";");
                stringBuilder.AppendLine("\t\tpublic const string Delete = Default + \".Delete\";");
                stringBuilder.AppendLine("\t}");
            }

            line = reader.ReadLine();
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
