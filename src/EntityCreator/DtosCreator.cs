using Humanizer;
using System.Text;

namespace EntityCreator;

public class DtosCreator(string @namespace, string path)
{
    private string entityName;
    private List<PropertyModel> properties;
    private string groupName;
    private string folder;

    public bool Create(string entityName, List<PropertyModel> properties)
    {
        this.entityName = entityName.Dehumanize();
        this.properties = properties;
        this.groupName = entityName.Pluralize();
        this.folder = $"{path}\\src\\{@namespace}.Application.Contracts\\{groupName}\\Dtos";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!CreateDomainDto())
            return false;

        if (!CreateUpdateDto())
            return false;

        if (!CreateListInputDto())
            return false;

        return true;
    }

    public bool CreateDomainDto()
    {
        entityName = entityName.Dehumanize();

        string artifactName = $"{entityName}Dto";
        string filename = $"{folder}\\{artifactName}.cs";

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using Volo.Abp.Application.Dtos;");
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(@namespace)
            .Append('.')
            .Append(groupName)
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : FullAuditedEntityDto<Guid>");

        stringBuilder.AppendLine("{");

        foreach (var property in properties)
        {
            if (property.Type == "Entity" || property.Type == "ValueObject" || property.Type == "AggregateRoot")
                continue;

            string propertyType = property.Type;

            if (!property.IsRequired && propertyType == "string")
                propertyType += "?";

            stringBuilder
                .Append("\tpublic ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .Append(" { get; set; }")
                .AppendLine();
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

    private bool CreateUpdateDto()
    {
        string artifactName = $"CreateUpdate{entityName}Dto";
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
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .AppendLine("[Serializable]");

        stringBuilder
            .Append("public class ")
            .AppendLine(artifactName);

        stringBuilder.AppendLine("{");

        foreach (var property in properties)
        {
            if (property.Type == "Entity" || property.Type == "ValueObject" || property.Type == "AggregateRoot")
                continue;

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

            string propertyType = property.Type;
            
            if (!property.IsRequired && propertyType == "string")
                propertyType += "?";
            
            stringBuilder
                .Append("\tpublic ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .AppendLine(" { get; set; }")
                .AppendLine();
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;

    }

    private bool CreateListInputDto()
    {
        string artifactName = $"{entityName}GetListInputDto";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.ComponentModel;");
        stringBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        stringBuilder.AppendLine("using Volo.Abp.Application.Dtos;");
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(@namespace)
            .Append('.')
            .Append(groupName)
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .AppendLine("[Serializable]");

        stringBuilder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : PagedAndSortedResultRequestDto");

        stringBuilder.AppendLine("{");

        foreach (var property in properties)
        {
            if (property.Type == "Entity" || 
                property.Type == "ValueObject" || 
                property.Type == "AggregateRoot" || 
                property.IsCollection)
                continue;

            string propertyType = property.Type;

            if (!property.IsRequired && propertyType == "string")
                propertyType += "?";

            stringBuilder
                .Append("\tpublic ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .Append(" { get; set; }")
                .AppendLine();
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;

    }
}
