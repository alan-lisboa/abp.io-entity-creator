using Humanizer;
using System.Text;

namespace EntityCreator;

public class ModelCreator(string @namespace, string path)
{
    public bool Create(string entityName, List<PropertyModel> properties)
    {
        entityName = entityName.Dehumanize();
        
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Domain\\{groupName}";
        string filename = $"{folder}\\{entityName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using Volo.Abp.Domain.Entities.Auditing;");
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(@namespace)
            .Append('.')
            .Append(groupName)
            .AppendLine(";")
            .AppendLine();

        stringBuilder
            .Append("public class ")
            .Append(entityName)
            .AppendLine(" : FullAuditedAggregateRoot<Guid>");

        stringBuilder.AppendLine("{");

        foreach (var property in properties)
        {
            stringBuilder
                .Append("\tpublic ")
                .Append(property.Type)
                .Append(' ').Append(property.Name)
                .Append(" { get; set; }")
                .AppendLine();
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
