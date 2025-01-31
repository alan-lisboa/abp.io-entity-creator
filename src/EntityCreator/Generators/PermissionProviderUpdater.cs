using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class PermissionProviderUpdater(EntityModel entity)
{
    public bool Update()
    {
        string artifactName = $"{entity.ProjectName}PermissionDefinitionProvider";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Permissions";
        string filename = $"{folder}\\{artifactName}.cs";
        string permissions = $"{entity.ProjectName}Permissions.{entity.Name}";
        string localizer = $"Permission:{entity.Name}";
        string obj = $"{entity.Pluralized.Camelize()}Permission";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        bool foundGroup = false;

        while (line != null)
        {
            if (line.Contains("var myGroup = "))
                foundGroup = true;

            if (line.TrimEnd().EndsWith('}') && foundGroup)
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

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

}
