using EntityCreator.Models;
using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator.Generators;

public class EFRepositoryCreator(EntityModel entity)
{
    public bool Create()
    {
        string dbContext = $"{entity.ProjectName}DbContext";
        string artifactName = $"{entity.Name}Repository";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.EntityFrameworkCore\\{entity.Pluralized}";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;
        
        StringBuilder stringBuilder = new();
        
        stringBuilder
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

        stringBuilder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        stringBuilder
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

        stringBuilder
            .AppendLine("{");

        stringBuilder
            .Append("\tpublic ")
            .Append(entity.Name)
            .Append("Repository(")
            .Append("IDbContextProvider<")
            .Append(dbContext)
            .AppendLine("> dbContextProvider) : base(dbContextProvider)")
            .AppendLine("\t{")
            .AppendLine("\t}")
            .AppendLine();

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
