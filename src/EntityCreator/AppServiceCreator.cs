using Humanizer;
using System.Text;

namespace EntityCreator;

public class AppServiceCreator(string @namespace, string path)
{
    public bool Create(string entityName)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{entityName}AppService";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application\\{groupName}";
        string filename = $"{folder}\\{artifactName}.cs";
        string permissions = $"{projectName}Permissions.{groupName}";
        string entityDto = $"{entityName}Dto";
        string createDto = $"CreateUpdate{entityName}Dto";
        string repository = $"{entityName.Camelize()}Repository";
        string irepository = $"I{entityName}Repository";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("using System;")
            .AppendLine("using Microsoft.AspNetCore.Authorization;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine("using Volo.Abp.Application.Services;")
            .AppendLine("using Volo.Abp.Domain.Repositories;")
            .AppendLine($"using {@namespace}.Permissions;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {@namespace}.{groupName};")
            .AppendLine();

        stringBuilder
            .AppendLine($"[Authorize({permissions}.Default)]");

        stringBuilder
            .AppendLine($"public class {artifactName} :")
            .AppendLine("\tCrudAppService<")
            .AppendLine($"\t\t{entityName},")
            .AppendLine($"\t\t{entityDto},")
            .AppendLine("\t\tGuid,")
            .AppendLine("\t\tPagedAndSortedResultRequestDto,")
            .AppendLine($"\t\t{createDto}>,")
            .AppendLine($"\tI{artifactName}");

        stringBuilder.AppendLine("{");

        stringBuilder
            .Append($"\tpublic {artifactName} ")
            .Append($"(IRepository<{entityName}, Guid> repository) : ")
            .AppendLine("base(repository)");

        stringBuilder.AppendLine("\t{");

        stringBuilder
            .Append("\t\tGetPolicyName = ")
            .AppendLine($"{permissions}.Default;");
            
        stringBuilder
            .Append("\t\tGetListPolicyName = ")
            .AppendLine($"{permissions}.Default;");

        stringBuilder
            .Append("\t\tCreatePolicyName = ")
            .AppendLine($"{permissions}.Create;");
            
        stringBuilder
            .Append("\t\tUpdatePolicyName = ")
            .AppendLine($"{permissions}.Edit;");
            
        stringBuilder
            .Append("\t\tDeletePolicyName = ")
            .AppendLine($"{permissions}.Delete;");

        stringBuilder.AppendLine("\t}");

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
