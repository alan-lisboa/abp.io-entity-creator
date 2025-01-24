using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator;

public class EFRepositoryCreator(string @namespace, string path)
{
    public bool Create(string entityName)
    {
        entityName = entityName.Dehumanize();
        
        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string dbContext = $"{projectName}DbContext";
        string artifactName = $"{entityName}Repository";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.EntityFrameworkCore\\{groupName}";
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
            .AppendLine($"using {@namespace}.EntityFrameworkCore;")
            .AppendLine("using Volo.Abp.Domain.Repositories.EntityFrameworkCore;")
            .AppendLine("using Volo.Abp.EntityFrameworkCore;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using Microsoft.EntityFrameworkCore;")
            .AppendLine($"using {@namespace}.{groupName};")
            .AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(@namespace)
            .Append('.')
            .Append(groupName)
            .AppendLine(";")
            .AppendLine();

        stringBuilder
            .Append("public class ")
            .Append(entityName)
            .Append("Repository : EfCoreRepository<")
            .Append(dbContext)
            .Append(", ")
            .Append(entityName)
            .Append(", ")
            .Append("Guid")
            .Append(">, I")
            .Append(entityName)
            .AppendLine("Repository");

        stringBuilder
            .AppendLine("{");

        stringBuilder
            .Append("\tpublic ")
            .Append(entityName)
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
