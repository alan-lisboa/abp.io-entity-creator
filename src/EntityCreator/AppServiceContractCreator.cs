﻿using Humanizer;
using System.Text;

namespace EntityCreator;

public class AppServiceContractCreator(string @namespace, string path)
{
    public bool Create(string entityName)
    {
        entityName = entityName.Dehumanize();

        string artifactName = $"I{entityName}AppService";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Application.Contracts\\{groupName}";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Volo.Abp.Application.Dtos;")
            .AppendLine("using Volo.Abp.Application.Services;")
            .AppendLine($"using {@namespace}.{groupName}.Dtos;")
            .AppendLine();

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
            .AppendLine(" :")
            .AppendLine("\tICrudAppService<")
            .AppendLine($"\t\t{entityName}Dto,")
            .AppendLine("\t\tGuid,")
            .AppendLine($"\t\t{entityName}GetListInputDto,")
            .AppendLine($"\t\tCreateUpdate{entityName}Dto,")
            .AppendLine($"\t\tCreateUpdate{entityName}Dto>");

        stringBuilder.AppendLine("{");

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
