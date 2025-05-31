using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class AppServiceContractCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"I{entity.Name}AppService";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\Contracts\\{entity.Pluralized}";
        filename = $"{folder}\\{artifactName}.cs";

        CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        builder
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine("using Volo.Abp.Application.Services;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;")
            .AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        builder
            .Append("public interface ")
            .Append(artifactName)
            .AppendLine(" :");

        indentationLevel++;
        
        builder
            .Append(Indentation)
            .AppendLine("ICrudAppService<");

        indentationLevel++;
        
        builder
            .Append(Indentation)
            .AppendLine($"{entity.Name}Dto,");

        builder
            .Append(Indentation)
            .AppendLine("Guid,");

        builder
            .Append(Indentation)
            .AppendLine($"{entity.Name}GetListInputDto,");

        builder
            .Append(Indentation)
            .AppendLine($"CreateUpdate{entity.Name}Dto,");

        builder
            .Append(Indentation)
            .AppendLine($"CreateUpdate{entity.Name}Dto>");

        indentationLevel = 0;

        builder
            .AppendLine("{");

        builder
            .AppendLine("}");

        return WriteToFile();
    }
}
