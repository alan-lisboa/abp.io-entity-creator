using EntityCreator.Helpers;
using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators;

public class AppServiceCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"{entity.Name}AppService";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Application\\Services\\{entity.Pluralized}";
        filename = $"{folder}\\{artifactName}.cs";
        
        CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        string permissions = $"{entity.ProjectName}Permissions.{entity.Name}";
        string entityDto = $"{entity.Name}Dto";
        string createUpdateDto = $"CreateUpdate{entity.Name}Dto";
        string getListDto = $"{entity.Name}GetListInputDto";
        string irepository = $"I{entity.Name}Repository";

        // usings
        builder
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

        builder
            .AppendLine($"namespace {entity.Namespace}.{entity.Pluralized};")
            .AppendLine();

        // class
        builder
            .AppendLine($"[Authorize({permissions}.Default)]");

        builder
            .AppendLine($"public class {artifactName} :");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("CrudAppService<");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"{entity.Name}, ");

        builder
            .Append(Indentation)
            .AppendLine($"{entityDto}, ");

        builder
            .Append(Indentation)
            .AppendLine("Guid, ");

        builder
            .Append(Indentation)
            .AppendLine($"{getListDto}, ");

        builder
            .Append(Indentation)
            .AppendLine($"{createUpdateDto}, ");

        builder
            .Append(Indentation)
            .AppendLine($"{createUpdateDto}>,");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine($"I{artifactName}");

        builder
            .AppendLine("{");

        // fields
        builder
            .Append(Indentation)
            .Append("private readonly ")
            .Append($"{irepository} ")
            .AppendLine("_repository;")
            .AppendLine();

        // constructor
        builder
            .Append(Indentation)
            .Append($"public {artifactName} ")
            .Append($"({irepository} repository) : ")
            .AppendLine("base(repository)")
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("_repository = repository;")
            .AppendLine();

        // permissions
        builder
            .Append(Indentation)
            .Append("GetPolicyName = ")
            .AppendLine($"{permissions}.Default;");
            
        builder
            .Append(Indentation)
            .Append("GetListPolicyName = ")
            .AppendLine($"{permissions}.Default;");

        builder
            .Append(Indentation)
            .Append("CreatePolicyName = ")
            .AppendLine($"{permissions}.Create;");
        
        builder
            .Append(Indentation)
            .Append("UpdatePolicyName = ")
            .AppendLine($"{permissions}.Edit;");
            
        builder
            .Append(Indentation)
            .Append("DeletePolicyName = ")
            .AppendLine($"{permissions}.Delete;");

        indentationLevel--;

        builder
            .AppendLine("}");

        // methods
        builder
            .Append(Indentation)
            .Append("protected override async Task<IQueryable<")
            .Append($"{entity.Name}>>" )
            .Append("CreateFilteredQueryAsync(")
            .AppendLine($"{getListDto} input)");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("return (await base.CreateFilteredQueryAsync(input))");

        var whereif = false;

        indentationLevel++;

        foreach (var property in entity.Properties!)
        {
            if (property.Type != BaseTypeHelper.String)
                continue;

            if (!whereif)
                whereif = true;
            else
                builder.AppendLine();

            builder
                .Append(Indentation)
                .Append(".WhereIf(!input.")
                .Append($"{property.Name}")
                .Append(".IsNullOrWhiteSpace(), x => x.")
                .Append($"{property.Name}!.Contains(")
                .Append($"input.{property.Name}!))");
        }

        builder
            .Append(Indentation)
            .AppendLine(";");

        indentationLevel = 1;

        builder
            .Append(Indentation)
            .AppendLine("}");

        builder
            .AppendLine("}");

        return WriteToFile();
    }
}
