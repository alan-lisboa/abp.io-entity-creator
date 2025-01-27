using Humanizer;
using System;
using System.Text;

namespace EntityCreator;

public class AppServiceCreator(string @namespace, string path)
{
    public bool Create(string entityName, List<PropertyModel> properties)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{entityName}AppService";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application\\{groupName}";
        string filename = $"{folder}\\{artifactName}.cs";
        string permissions = $"{projectName}Permissions.{groupName}";
        string entityDto = $"{entityName}Dto";
        string createUpdateDto = $"CreateUpdate{entityName}Dto";
        string getListDto = $"{entityName}GetListInputDto";
        string irepository = $"I{entityName}Repository";

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
            .AppendLine($"using {@namespace}.{groupName}.Dtos;")
            .AppendLine($"using {@namespace}.Permissions;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {@namespace}.{groupName};")
            .AppendLine();

        // class
        stringBuilder
            .AppendLine($"[Authorize({permissions}.Default)]");

        stringBuilder
            .AppendLine($"public class {artifactName} :")
            .AppendLine("\tCrudAppService<")
            .AppendLine($"\t\t{entityName}, ")
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
            .Append($"{entityName}>>" )
            .Append("CreateFilteredQueryAsync(")
            .AppendLine($"{getListDto} input)")
            .AppendLine("\t{");

        stringBuilder
            .AppendLine("\t\treturn (await base.CreateFilteredQueryAsync(input))");

        foreach (var property in properties)
        {
            if (property.Type == "Entity" || 
                property.Type == "ValueObject" || 
                property.Type == "AggregatedRoot" || 
                property.IsCollection)
                continue;

            stringBuilder
                .Append("\t\t\t.WhereIf(!input.")
                .Append($"{property.Name}")
                .Append(".IsNullOrWhiteSpace(), x => x.")
                .Append($"{property.Name}.Contains(")
                .Append($"input.{property.Name}))");
        }

        stringBuilder
            .AppendLine("\t\t\t;")
            .AppendLine("\t}");

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
