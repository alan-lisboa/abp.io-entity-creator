using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class AppServiceContractCreator(EntityModel entity)
{
    public bool Create()
    {
        string artifactName = $"I{entity.Name}AppService";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Application.Contracts\\{entity.Pluralized}";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine("using Volo.Abp.Application.Services;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;")
            .AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        stringBuilder
            .Append("public interface ")
            .Append(artifactName)
            .AppendLine(" :")
            .AppendLine("\tICrudAppService<")
            .AppendLine($"\t\t{entity.Name}Dto,")
            .AppendLine("\t\tGuid,")
            .AppendLine($"\t\t{entity.Name}GetListInputDto,")
            .AppendLine($"\t\tCreateUpdate{entity.Name}Dto,")
            .AppendLine($"\t\tCreateUpdate{entity.Name}Dto>");

        stringBuilder.AppendLine("{");

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
