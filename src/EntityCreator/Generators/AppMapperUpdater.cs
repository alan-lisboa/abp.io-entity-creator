using EntityCreator.Helpers;
using EntityCreator.Models;

namespace EntityCreator.Generators;

public class AppMapperUpdater(EntityModel entity) : BaseGenerator
{
    public override bool Handle() 
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application";
        artifactName = $"{entity.ProjectName}ApplicationAutoMapperProfile";
        filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        string entityDto = $"{entity.Name}Dto";
        string createUpdateDto = $"CreateUpdate{entity.Name}Dto";

        bool added = false;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        while (line != null)
        {
            if (line.Contains("using AutoMapper;"))
            {
                builder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;");
            }

            if (line.Contains('}') && !added)
            {
                added = true;

                builder.AppendLine();

                indentationLevel = 2;

                builder
                    .Append(Indentation)
                    .Append("CreateMap")
                    .Append($"<{entity.Name}, {entityDto}>")
                    .AppendLine("();");

                builder
                    .Append(Indentation)
                    .Append("CreateMap")
                    .Append($"<{createUpdateDto}, {entity.Name}>")
                    .AppendLine("(MemberList.Source);");

                foreach (var property in entity.Properties!)
                {
                    if (BaseTypeHelper.IsAggregatedChild(property.Type!))
                    {
                        builder
                            .Append(Indentation)
                            .Append("CreateMap")
                            .Append($"<{property.Name}, {entity.Name}{property.Name}Dto>();")
                            .AppendLine();

                        builder
                            .AppendLine(Indentation)
                            .Append("CreateMap")
                            .Append($"<{entity.Name}{property.Name}Dto, {property.Name}>();")
                            .AppendLine();
                    }
                }
            }

            builder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        return WriteToFile();
    }

}
