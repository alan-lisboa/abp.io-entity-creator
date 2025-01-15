using Humanizer;
using System.Text;

namespace EntityCreator;

public class CreateModalCreator(string @namespace, string path)
{
    private string entityName;
    private string projectName;
    private string groupName;
    private string folder;
    private string permissions;
    private string htmlPage;
    private string modelPage;
    private string appServiceName;
    private string iAppService;
    private string appService;
    private string entityDto;
    private string createDto;
    private string mapper;

    public bool Create(string entityName)
    {
        this.entityName = entityName.Dehumanize();

        projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        groupName = entityName.Pluralize();
        folder = $"{path}\\src\\{@namespace}.Web\\Pages\\{groupName}";
        htmlPage = $"{folder}\\CreateModal.cshtml";
        modelPage = $"{folder}\\CreateModalModel.chtml.cs";
        permissions = $"{projectName}Permissions.{groupName}";
        appServiceName = $"{entityName}AppService";
        iAppService = $"I{appServiceName}";
        appService = $"_{appServiceName.Camelize()}";
        entityDto = $"{entityName}Dto";
        createDto = $"CreateUpdate{entityName}Dto";
        mapper = $"{projectName}WebAutoMapperProfile";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!CreateWeb())
            return false;

        if (!CreateModel())
            return false;

        UpdateMapper();

        return true;
    }

    private bool CreateWeb()
    {
        if (File.Exists(htmlPage))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("@page")
            .AppendLine($"@using {@namespace}.Localization")
            .AppendLine($"@using {@namespace}.Web.Pages.{groupName}")
            .AppendLine("@using Microsoft.Extensions.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal")
            .AppendLine("@model CreateModalModel");

        stringBuilder
            .Append("@inject IStringLocalizer")
            .AppendLine($"<{projectName}Resource> L");

        stringBuilder
            .AppendLine("@{")
            .AppendLine("\tLayout = null;")
            .AppendLine("}");

        stringBuilder
            .Append("<abp-dynamic-form ")
            .Append($"abp-model=\"{entityName}\" ")
            .AppendLine($"asp-page=\"/{groupName}/CreateModal\">");

        stringBuilder.AppendLine("\t<abp-modal>");
        
        stringBuilder
            .Append("\t\t<abp-modal-header ")
            .AppendLine($"title=\"@L[\"New{entityName}\"].Value\"></abp-modal-header>");
        
        stringBuilder.AppendLine("\t\t<abp-modal-body>");
        stringBuilder.AppendLine("\t\t\t<abp-form-content />");
        stringBuilder.AppendLine("\t\t</abp-modal-body>");
        
        stringBuilder
            .Append("\t\t<abp-modal-footer ")
            .Append("buttons=\"@(AbpModalButtons.Cancel|AbpModalButtons.Save)\">")
            .AppendLine("</abp-modal-footer>");
        
        stringBuilder.AppendLine("\t</abp-modal>");

        stringBuilder.AppendLine("</abp-dynamic-form>");

        File.WriteAllText(htmlPage, stringBuilder.ToString());

        return true;
    }

    private bool CreateModel()
    {
        if (File.Exists(modelPage))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Mvc;")
            .AppendLine($"using {@namespace}.{groupName};")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {@namespace}.Web.Pages.{groupName};")
            .AppendLine();

        stringBuilder
            .Append("public class CreateModalModel : ")
            .AppendLine($"{projectName}PageModel");

        stringBuilder
            .AppendLine("{");

        stringBuilder
            .AppendLine("\t[BindProperty]")
            .Append($"\tpublic {createDto} {entityName} ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        stringBuilder
            .AppendLine($"\tprivate readonly {iAppService} {appService};")
            .AppendLine();

        stringBuilder
            .Append("\tpublic CreateModalModel")
            .AppendLine($"({iAppService} {appServiceName.Camelize()})")
            .AppendLine("\t{")
            .AppendLine($"\t\t{appService} = {appServiceName.Camelize()};")
            .AppendLine("\t}")
            .AppendLine();

        stringBuilder
            .AppendLine("\tpublic void OnGet()")
            .AppendLine("\t{")
            .Append($"\t\t{entityName} = ")
            .AppendLine($"new {createDto.Dehumanize()}();")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("\tpublic async Task<IActionResult> OnPostAsync()")
            .AppendLine("\t{")
            .Append($"\t\tawait {appService}.")
            .AppendLine($"CreateAsync({entityName});")
            .AppendLine("\t\treturn NoContent();")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(modelPage, stringBuilder.ToString());

        return true;
    }

    private bool UpdateMapper()
    {
        string filename = $"{folder}\\{mapper}.cs";

        if (!File.Exists(filename))
            return false;

        bool added = false;

        StringBuilder stringBuilder = new();

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        while (line != null)
        {
            if (line.Contains('}') && !added)
            {
                added = true;

                stringBuilder.AppendLine();

                stringBuilder
                    .Append("\t\tCreateMap")
                    .Append($"<{entityDto}, {createDto}>")
                    .AppendLine("();");
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine();
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
