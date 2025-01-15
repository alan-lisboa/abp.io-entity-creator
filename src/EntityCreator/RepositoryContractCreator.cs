using Humanizer;
using System.Text;

namespace EntityCreator;

public class RepositoryContractCreator(string @namespace, string path)
{
    public bool Create(string entityName)
    {
        entityName = entityName.Dehumanize();

        string artifactName = $"I{entityName}Repository";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Domain\\{groupName}";
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
            .Append(@namespace)
            .Append('.')
            .Append(groupName)
            .AppendLine(";")
            .AppendLine();

        stringBuilder
            .Append("public interface ")
            .Append(artifactName)
            .Append(" : IRepository<")
            .Append(entityName)
            .AppendLine(", Guid>");

        stringBuilder.AppendLine("{");
        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
