using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators;

public class AppServiceCreator(EntityModel entity)
{
    public bool Create()
    {
        string artifactName = $"{entity.Name}AppService";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Application\\Services\\{entity.Pluralized}";
        string filename = $"{folder}\\{artifactName}.cs";
        string permissions = $"{entity.ProjectName}Permissions.{entity.Name}";
        string entityDto = $"{entity.Name}Dto";
        string createUpdateDto = $"CreateUpdate{entity.Name}Dto";
        string getListDto = $"{entity.Name}GetListInputDto";
        string irepository = $"I{entity.Name}Repository";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        // usings
        stringBuilder
            .AppendLine("using System;")
            .AppendLine("using System.Linq;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Authorization;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine("using Volo.Abp.Application.Services;")
            .AppendLine("using Volo.Abp.Domain.Repositories;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;")
            .AppendLine($"using {entity.Namespace}.Permissions;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {entity.Namespace}.{entity.Pluralized};")
            .AppendLine();

        // class
        stringBuilder
            .AppendLine($"[Authorize({permissions}.Default)]");

        stringBuilder
            .AppendLine($"public class {artifactName} :")
            .AppendLine("\tCrudAppService<")
            .AppendLine($"\t\t{entity.Name}, ")
            .AppendLine($"\t\t{entityDto}, ")
            .AppendLine("\t\tGuid, ")
            .AppendLine($"\t\t{getListDto}, ")
            .AppendLine($"\t\t{createUpdateDto}, ")
            .AppendLine($"\t\t{createUpdateDto}>,")
            .AppendLine($"\tI{artifactName}");

        stringBuilder.AppendLine("{");

        // fields
        stringBuilder
            .Append("\tprivate readonly ")
            .Append($"{irepository} ")
            .AppendLine("_repository;")
            .AppendLine();

        // constructor
        stringBuilder
            .Append($"\tpublic {artifactName} ")
            .Append($"({irepository} repository) : ")
            .AppendLine("base(repository)")
            .AppendLine("\t{")
            .AppendLine("\t\t_repository = repository;")
            .AppendLine();

        // permissions
        stringBuilder
            .Append("\t\tGetPolicyName = ")
            .AppendLine($"{permissions}.Default;");
            
        stringBuilder
            .Append("\t\tGetListPolicyName = ")
            .AppendLine($"{permissions}.Default;");

        stringBuilder
            .Append("\t\tCreatePolicyName = ")
            .AppendLine($"{permissions}.Create;");
        { }
        stringBuilder
            .Append("\t\tUpdatePolicyName = ")
            .AppendLine($"{permissions}.Edit;");
            
        stringBuilder
            .Append("\t\tDeletePolicyName = ")
            .AppendLine($"{permissions}.Delete;");

        stringBuilder.AppendLine("\t}");

        // methods
        stringBuilder
            .Append("\tprotected override async Task<IQueryable<")
            .Append($"{entity.Name}>>" )
            .Append("CreateFilteredQueryAsync(")
            .AppendLine($"{getListDto} input)")
            .AppendLine("\t{");

        stringBuilder
            .AppendLine("\t\treturn (await base.CreateFilteredQueryAsync(input))");

        var whereif = false;

        foreach (var property in entity.Properties!)
        {
            if (property.Type != BaseTypes.String)
                continue;

            if (!whereif)
                whereif = true;
            else
                stringBuilder.AppendLine();

            stringBuilder
                .Append("\t\t\t.WhereIf(!input.")
                .Append($"{property.Name}")
                .Append(".IsNullOrWhiteSpace(), x => x.")
                .Append($"{property.Name}!.Contains(")
                .Append($"input.{property.Name}!))");
        }

        stringBuilder
            .AppendLine("\t\t\t;");

        stringBuilder
            .AppendLine("\t}");

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
