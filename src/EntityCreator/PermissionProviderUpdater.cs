using Humanizer;
using System.Text;

namespace EntityCreator;

public class PermissionProviderUpdater(string @namespace, string path)
{
    public bool Update(string entityName)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{projectName}PermissionDefinitionProvider";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application.Contracts\\Permissions";
        string filename = $"{folder}\\{artifactName}.cs";
        string permissions = $"{projectName}Permissions.{entityName}";
        string localizer = $"Permission:{entityName}";
        string obj = $"{groupName.Camelize()}Permission";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        
        bool foundGroup = false;

        while (line != null)
        {
            if (line.Contains("var myGroup = "))
                foundGroup = true;

            if (line.TrimEnd().EndsWith("}") && foundGroup)
            {
                stringBuilder
                    .AppendLine()
                    .Append("\t\tvar ")
                    .Append(obj)
                    .Append(" = myGroup.AddPermission(")
                    .Append(permissions)
                    .Append(".Default, L(\"")
                    .Append(localizer)
                    .AppendLine("\"));");
                    
                stringBuilder
                    .Append("\t\t")
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Create, L(\"")
                    .Append(localizer)
                    .AppendLine(".Create\"));");

                stringBuilder
                    .Append("\t\t")
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Edit, L(\"")
                    .Append(localizer)
                    .AppendLine(".Edit\"));");

                stringBuilder
                    .Append("\t\t")
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Delete, L(\"")
                    .Append(localizer)
                    .AppendLine(".Delete\"));");

                foundGroup = false;
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine();
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
