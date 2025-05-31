using EntityCreator.Models;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator.Generators;

public class EFRepositoryCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"{entity.Name}Repository";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.EntityFrameworkCore\\Entities\\{entity.Pluralized}";
        filename = $"{folder}\\{artifactName}.cs";
        
        CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        string dbContext = $"{entity.ProjectName}DbContext";

        builder
            .AppendLine("using System;")
            .AppendLine("using System.Linq;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine($"using {entity.Namespace}.EntityFrameworkCore;")
            .AppendLine("using Volo.Abp.Domain.Repositories.EntityFrameworkCore;")
            .AppendLine("using Volo.Abp.EntityFrameworkCore;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using Microsoft.EntityFrameworkCore;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
            .AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        builder
            .Append("public class ")
            .Append(entity.Name)
            .Append("Repository : EfCoreRepository<")
            .Append(dbContext)
            .Append(", ")
            .Append(entity.Name)
            .Append(", ")
            .Append("Guid")
            .Append(">, I")
            .Append(entity.Name)
            .AppendLine("Repository");

        builder
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append("public ")
            .Append(entity.Name)
            .Append("Repository(")
            .Append("IDbContextProvider<")
            .Append(dbContext)
            .AppendLine("> dbContextProvider) : base(dbContextProvider)")
            .Append(Indentation)
            .AppendLine("{")
            .Append(Indentation)
            .AppendLine("}")
            .AppendLine();

        builder
            .AppendLine("}");

        return WriteToFile();
    }
}
