using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class DtosCreator(EntityModel entity)
{
    private string? folder;

    public bool Create()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\{entity.Pluralized}\\Dtos";

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
        string artifactName = $"{entity.Name}Dto";
        string filename = $"{folder}\\{artifactName}.cs";

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using Volo.Abp.Application.Dtos;");
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : FullAuditedEntityDto<Guid>");

        stringBuilder.AppendLine("{");

        foreach (var property in entity.Properties!)
        {
            if (property.Type == BaseTypes.Entity || 
                property.Type == BaseTypes.ValueObject || 
                property.Type == BaseTypes.AggregatedRoot)
                continue;

            string propertyType = property.Type!;

            if (!property.IsRequired && propertyType == BaseTypes.String)
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
        string artifactName = $"CreateUpdate{entity.Name}Dto";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder!);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.ComponentModel.DataAnnotations;");
        stringBuilder.AppendLine("using Volo.Abp.Application.Dtos;");
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .AppendLine("[Serializable]");

        stringBuilder
            .Append("public class ")
            .AppendLine(artifactName);

        stringBuilder.AppendLine("{");

        foreach (var property in entity.Properties!)
        {
            if (property.Type == BaseTypes.Entity || 
                property.Type == BaseTypes.ValueObject || 
                property.Type == BaseTypes.AggregatedRoot)
                continue;

            if (property.IsRequired)
            {
                stringBuilder
                    .AppendLine("\t[Required]");
            }

            if (property.Type!.Equals(BaseTypes.String, StringComparison.OrdinalIgnoreCase) && property.Size > 0)
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
            
            if (!property.IsRequired && propertyType == BaseTypes.String)
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
        string artifactName = $"{entity.Name}GetListInputDto";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder!);

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
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        stringBuilder
            .AppendLine("[Serializable]");

        stringBuilder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : PagedAndSortedResultRequestDto");

        stringBuilder.AppendLine("{");

        foreach (var property in entity.Properties!)
        {
            if (property.Type == BaseTypes.Entity || 
                property.Type == BaseTypes.ValueObject || 
                property.Type == BaseTypes.AggregatedRoot || 
                property.IsCollection)
                continue;

            string propertyType = property.Type!;

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
