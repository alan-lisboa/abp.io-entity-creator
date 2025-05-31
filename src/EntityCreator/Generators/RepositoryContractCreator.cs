using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class RepositoryContractCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        artifactName = $"I{entity.Name}Repository";
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Domain\\Entities\\{entity.Pluralized}";
        filename = $"{folder}\\{artifactName}.cs";

        CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        builder
            .AppendLine("using System;")
            .AppendLine("using System.Collections.Generic;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Volo.Abp.Domain.Repositories;")
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
            .Append(" : IRepository<")
            .Append(entity.Name)
            .AppendLine(", Guid>");

        builder
            .AppendLine("{");

        builder
            .AppendLine("}");

        return WriteToFile();
    }

}
