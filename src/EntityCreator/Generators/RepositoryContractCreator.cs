using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class RepositoryContractCreator(EntityModel entity)
{
    public bool Create()
    {
        string artifactName = $"I{entity.Name}Repository";
        string folder = $"{entity.Location}\\src\\{entity.Namespace}.Domain\\{entity.Pluralized}";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.Collections.Generic;");
        stringBuilder.AppendLine("using System.Threading.Tasks;");
        stringBuilder.AppendLine("using Volo.Abp.Domain.Repositories;");

        stringBuilder.AppendLine();
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
            .Append(" : IRepository<")
            .Append(entity.Name)
            .AppendLine(", Guid>");

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
