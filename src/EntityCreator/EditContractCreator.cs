using Humanizer;
using System.Text;

namespace EntityCreator;

public class EditContractCreator(string @namespace, string path)
{
    public bool Create(string entityName, List<PropertyModel> properties)
    {
        entityName = entityName.Dehumanize();

        string artifactName = $"CreateUpdate{entityName}Dto";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application.Contracts\\{groupName}";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        stringBuilder.AppendLine("using Volo.Abp.Application.Dtos;");
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
            .AppendLine(artifactName);

        stringBuilder.AppendLine("{");

        foreach (var property in properties)
        {
            if (property.IsRequired)
            {
                stringBuilder
                    .AppendLine("\t[Required]");
            }

            if (property.Type.Equals("string", StringComparison.OrdinalIgnoreCase) && property.Size > 0)
            {
                stringBuilder
                    .Append("\t[StringLength(")
                    .Append(property.Size)
                    .AppendLine(")]");
            }

            if (property.Type.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                stringBuilder
                    .AppendLine("\t[DataType(DataType.Date)]");
            }

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
