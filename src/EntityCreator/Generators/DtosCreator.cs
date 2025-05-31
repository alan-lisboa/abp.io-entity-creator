using EntityCreator.Helpers;
using EntityCreator.Models;
using Humanizer;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace EntityCreator.Generators;

public class DtosCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Contracts\\{entity.Pluralized}\\Dtos";

        CreateDirectory(folder);

        if (!CreateDomainDto())
            return false;

        if (!CreateUpdateDto())
            return false;

        if (!CreateListInputDto())
            return false;

        if (!CreateAggregatedChildDto())
            return false;

        return true;
    }

    public bool CreateDomainDto()
    {
        artifactName = $"{entity.Name}Dto";
        filename = $"{folder}\\{artifactName}.cs";
        
        if (File.Exists(filename))
            return false;

        Initialize();

        builder
            .AppendLine("using System;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        builder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : FullAuditedEntityDto<Guid>");

        builder.AppendLine("{");

        indentationLevel++;

        foreach (var property in entity.Properties!)
        {
            string propertyType = property.Type!;

            if (BaseTypeHelper.IsAggregatedChild(property.Type!))
                propertyType = $"{entity.Name}{property.Name!}Dto";

            if (!property.IsRequired && BaseTypeHelper.IsNullable(property.Type!))
                propertyType += "?";

            builder
                .Append(Indentation)
                .Append("public ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .Append(" { get; set; }")
                .AppendLine();
        }

        indentationLevel--;

        builder.AppendLine("}");

        return WriteToFile();
    }

    private bool CreateUpdateDto()
    {
        artifactName = $"CreateUpdate{entity.Name}Dto";
        filename = $"{folder}\\{artifactName}.cs";

        CreateDirectory(folder!);

        if (File.Exists(filename))
            return false;

        Initialize();

        builder
            .AppendLine("using System;")
            .AppendLine("using System.ComponentModel.DataAnnotations;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        builder
            .AppendLine("[Serializable]");

        builder
            .Append("public class ")
            .AppendLine(artifactName);

        builder
            .AppendLine("{");

        indentationLevel++;

        foreach (var property in entity.Properties!)
        {
            string propertyType = property.Type!;

            if (BaseTypeHelper.IsAggregatedChild(property.Type!))
                propertyType = $"{entity.Name}{property.Name!}Dto";

            if (!property.IsRequired && BaseTypeHelper.IsNullable(property.Type!))
                propertyType += "?";

            if (property.IsRequired)
            {
                builder
                    .Append(Indentation)
                    .AppendLine("[Required]");
            }

            if (property.Type!.Equals(BaseTypeHelper.String, StringComparison.OrdinalIgnoreCase) && property.Size > 0)
            {
                builder
                    .Append(Indentation)
                    .Append("[StringLength(")
                    .Append(property.Size)
                    .AppendLine(")]");
            }

            if (property.Type.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                builder
                    .Append(Indentation)
                    .AppendLine("[DataType(DataType.Date)]");
            }
            
            builder
                .Append(Indentation)
                .Append("public ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .AppendLine(" { get; set; }")
                .AppendLine();
        }

        indentationLevel--;

        builder
            .AppendLine("}");

        return WriteToFile();
    }

    private bool CreateListInputDto()
    {
        artifactName = $"{entity.Name}GetListInputDto";
        filename = $"{folder}\\{artifactName}.cs";

        CreateDirectory(folder!);

        if (File.Exists(filename))
            return false;

        Initialize();

        builder
            .AppendLine("using System;")
            .AppendLine("using System.ComponentModel;")
            .AppendLine("using System.ComponentModel.DataAnnotations;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(".Dtos;")
            .AppendLine();

        builder
            .AppendLine("[Serializable]");

        builder
            .Append("public class ")
            .Append(artifactName)
            .AppendLine(" : PagedAndSortedResultRequestDto");

        builder
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append("public string? Search { get; set; }")
            .AppendLine();

        foreach (var property in entity.Properties!)
        {
            if (BaseTypeHelper.IsEntityType(property.Type!) || property.IsCollection)
                continue;

            string propertyType = property.Type!;

            if (!property.IsRequired && propertyType == "string")
                propertyType += "?";

            builder
                .Append(Indentation)
                .Append("public ")
                .Append(propertyType)
                .Append(' ').Append(property.Name)
                .Append(" { get; set; }")
                .AppendLine();
        }

        indentationLevel--;

        builder.AppendLine("}");

        return WriteToFile();
    }

    private bool CreateAggregatedChildDto()
    {
        foreach (var entityProperty in entity.Properties!)
        {
            if (!BaseTypeHelper.IsAggregatedChild(entityProperty.Type!))
                continue;

            artifactName = $"{entity.Name}{entityProperty.Name}Dto";
            filename = $"{folder}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            Initialize();

            builder
                .AppendLine("using System;")
                .AppendLine("using Volo.Abp.Application.Dtos;")
                .AppendLine();

            builder
                .Append("namespace ")
                .Append(entity.Namespace)
                .Append('.')
                .Append(entity.Pluralized)
                .AppendLine(".Dtos;")
                .AppendLine();

            builder
                .Append("public class ")
                .Append(artifactName);

            if (entityProperty.Type == BaseTypeHelper.Entity)
                builder.AppendLine(" : Entity<Guid>");
            else
                builder.AppendLine();

            builder.AppendLine("{");

            indentationLevel++;

            foreach (var property in entityProperty.Properties!)
            {
                string propertyType = property.Type!;

                if (BaseTypeHelper.IsEntityType(property.Type!))
                    continue;

                if (!property.IsRequired && property.Type! == BaseTypeHelper.String)
                    propertyType += "?";

                builder
                    .Append(Indentation)
                    .Append("public ")
                    .Append(propertyType)
                    .Append(' ').Append(property.Name)
                    .Append(" { get; set; }")
                    .AppendLine();
            }

            indentationLevel--;

            builder.AppendLine("}");

            WriteToFile();
        }

        return true;
    }
}
