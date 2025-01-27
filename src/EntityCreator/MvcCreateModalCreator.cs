using Humanizer;
using System.Text;

namespace EntityCreator;

public class MvcCreateModalCreator(string @namespace, string path)
{
    private string? entityName;
    private string? projectName;
    private string? groupName;    
    private string? folder;
    private string? htmlFile;
    private string? modelFile;
    private string? appServiceName;
    private string? createDto;
    private string? viewModel;

    public bool Create(string entityName)
    {
        this.entityName = entityName.Dehumanize();

        projectName = @namespace[(@namespace.IndexOf('.') + 1)..];
        groupName = entityName.Pluralize();
        folder = $"{path}\\src\\{@namespace}.Web\\Pages\\{groupName}\\{this.entityName}";
        htmlFile = $"{folder}\\CreateModal.cshtml";
        modelFile = $"{folder}\\CreateModalModel.chtml.cs";
        appServiceName = $"{entityName}AppService";
        createDto = $"CreateUpdate{entityName}Dto";
        viewModel = $"CreateEdit{entityName}ViewModel";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!CreatePage())
            return false;

        if (!CreateModel())
            return false;

        return true;
    }

    private bool CreatePage()
    {
        if (File.Exists(htmlFile))
            return false;

        StringBuilder stringBuilder = new();

        // usings
        stringBuilder
            .AppendLine("@page")
            .AppendLine("@using Microsoft.AspNetCore.Mvc.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal")
            .AppendLine($"@using {@namespace}.Localization");

        // inject
        stringBuilder
            .Append("@inject IHtmlLocalizer")
            .AppendLine($"<{projectName}Resource> L");

        // model
        stringBuilder
            .AppendLine($"@model {@namespace}.Web.Pages.{groupName}.{entityName}.CreateModalModel");

        // layout
        stringBuilder
            .AppendLine("@{")
            .AppendLine("\tLayout = null;")
            .AppendLine("}");

        stringBuilder
            .Append("<abp-dynamic-form ")
            .Append("abp-model=\"ViewModel\" ")
            .Append("data-ajaxForm=\"true\" ")
            .AppendLine($"asp-page=\"CreateModal\">");

        stringBuilder.AppendLine("\t<abp-modal>");
        
        stringBuilder
            .Append("\t\t<abp-modal-header ")
            .AppendLine($"title=\"@L[\"Create{entityName}\"].Value\"></abp-modal-header>");
        
        stringBuilder.AppendLine("\t\t<abp-modal-body>");
        stringBuilder.AppendLine("\t\t\t<abp-form-content />");
        stringBuilder.AppendLine("\t\t</abp-modal-body>");
        
        stringBuilder
            .Append("\t\t<abp-modal-footer ")
            .Append("buttons=\"@(AbpModalButtons.Cancel|AbpModalButtons.Save)\">")
            .AppendLine("</abp-modal-footer>");
        
        stringBuilder.AppendLine("\t</abp-modal>");

        stringBuilder.AppendLine("</abp-dynamic-form>");

        File.WriteAllText(htmlFile, stringBuilder.ToString());

        return true;
    }

    private bool CreateModel()
    {
        if (File.Exists(modelFile))
            return false;

        StringBuilder stringBuilder = new();

        // usings
        stringBuilder
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Mvc;")
            .AppendLine($"using {@namespace}.{groupName};")
            .AppendLine($"using {@namespace}.{groupName}.Dtos;")
            .AppendLine($"using {@namespace}.Web.Pages.{groupName}.{entityName}.ViewModels;")
            .AppendLine();

        // namespace
        stringBuilder
            .AppendLine($"namespace {@namespace}.Web.Pages.{groupName}.{entityName};")
            .AppendLine();

        stringBuilder
            .Append("public class CreateModalModel : ")
            .AppendLine($"{projectName}PageModel");

        stringBuilder
            .AppendLine("{");

        stringBuilder
            .AppendLine("\t[BindProperty]")
            .Append($"\tpublic {viewModel} ViewModel ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        stringBuilder
            .AppendLine($"\tprivate readonly I{appServiceName} _service;")
            .AppendLine();

        stringBuilder
            .Append("\tpublic CreateModalModel")
            .AppendLine($"(I{appServiceName} service)")
            .AppendLine("\t{")
            .AppendLine($"\t\t_service = service;")
            .AppendLine("\t}")
            .AppendLine();

        stringBuilder
            .AppendLine("\tpublic virtual async Task<IActionResult> OnPostAsync()")
            .AppendLine("\t{")
            .AppendLine($"\t\tvar dto = ObjectMapper.Map<{viewModel}, {createDto}>(ViewModel);")
            .AppendLine("\t\tawait _service.CreateAsync(dto);")
            .AppendLine("\t\treturn NoContent();")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(modelFile, stringBuilder.ToString());

        return true;
    }
}
