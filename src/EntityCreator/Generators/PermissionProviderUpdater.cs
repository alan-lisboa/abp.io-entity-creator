using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class PermissionProviderUpdater(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"{entity.ProjectName}PermissionDefinitionProvider";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Permissions";
        filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        string permissions = $"{entity.ProjectName}Permissions.{entity.Name}";
        string localizer = $"Permission:{entity.Name}";
        string obj = $"{entity.Pluralized.Camelize()}Permission";

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        bool foundGroup = false;

        while (line != null)
        {
            if (line.Contains("var myGroup = "))
                foundGroup = true;

            if (line.TrimEnd().EndsWith('}') && foundGroup)
            {
                indentationLevel = 2;

                builder
                    .AppendLine();

                builder
                    .Append(Indentation)
                    .Append("var ")
                    .Append(obj)
                    .Append(" = myGroup.AddPermission(")
                    .Append(permissions)
                    .Append(".Default, L(\"")
                    .Append(localizer)
                    .AppendLine("\"));");
                    
                builder
                    .Append(Indentation)
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Create, L(\"")
                    .Append(localizer)
                    .AppendLine(".Create\"));");

                builder
                    .Append(Indentation)
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Edit, L(\"")
                    .Append(localizer)
                    .AppendLine(".Edit\"));");

                builder
                    .Append("\t\t")
                    .Append(obj)
                    .Append(".AddChild(")
                    .Append(permissions)
                    .Append(".Delete, L(\"")
                    .Append(localizer)
                    .AppendLine(".Delete\"));");

                foundGroup = false;

                indentationLevel = 0;
            }

            builder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        return WriteToFile();
    }
}
