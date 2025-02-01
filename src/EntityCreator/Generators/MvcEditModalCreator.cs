using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class MvcEditModalCreator(EntityModel entity)
{
    private string? folder;
    private string? htmlFile;
    private string? modelFile;
    private string? appServiceName;
    private string? entityDto;
    private string? createDto;
    private string? viewModel;

    public bool Create()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Web\\Pages\\{entity.Pluralized}\\{entity.Name}";
        htmlFile = $"{folder}\\EditModal.cshtml";
        modelFile = $"{folder}\\EditModal.chtml.cs";
        appServiceName = $"{entity.Name}AppService";
        entityDto = $"{entity.Name}Dto";
        createDto = $"CreateUpdate{entity.Name}Dto";
        viewModel = $"CreateEdit{entity.Name}ViewModel";

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
            .AppendLine($"@using {entity.Namespace}.Localization")
            .AppendLine("@using Microsoft.AspNetCore.Mvc.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal");

        // inject
        stringBuilder
            .Append("@inject IHtmlLocalizer")
            .AppendLine($"<{entity.ProjectName}Resource> L");

        // model
        stringBuilder
            .AppendLine($"@model {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}.EditModalModel");

        stringBuilder
            .AppendLine("@{")
            .AppendLine("\tLayout = null;")
            .AppendLine("}");

        stringBuilder
            .Append("<form method=\"post\" ")
            .AppendLine($"action=\"@Url.Page(\"/{entity.Pluralized}/{entity.Name}/EditModal\")\">");

        stringBuilder.AppendLine("\t<abp-modal>");
        stringBuilder.AppendLine($"\t\t<abp-modal-header title=\"@L[\"Edit{entity.Name}\"].Value\"></abp-modal-header>");
        stringBuilder.AppendLine("\t\t<abp-modal-body>");
        stringBuilder.AppendLine("\t\t\t<abp-input asp-for=\"Id\" />");

        stringBuilder.AppendLine("\t\t\t<abp-tabs>");

        stringBuilder.AppendLine("\t\t\t\t<abp-tab title=\"@L[\"Home\"].Value\">");

        var mainProperties = entity.Properties!
            .Where(p => !BaseTypes.IsAggregatedChild(p.Type!));

        foreach (var property in mainProperties)
        {
            stringBuilder
                .Append("\t\t\t\t\t<abp-input ")
                .AppendLine($"asp-for=\"ViewModel.{property.Name}\" />");
        }

        stringBuilder.AppendLine("\t\t\t\t</abp-tab>");

        var secondaryProperties = entity.Properties!
            .Where(p => BaseTypes.IsAggregatedChild(p.Type!));

        foreach (var property in secondaryProperties)
        {
            stringBuilder
                .Append("\t\t\t\t<abp-tab title=\"@L[\"")
                .Append(property.Name)
                .AppendLine("\"].Value\">");

            foreach (var subProperty in property.Properties!)
            {
                stringBuilder
                    .Append("\t\t\t\t\t<abp-input ")
                    .AppendLine($"asp-for=\"ViewModel.{property.Name}.{subProperty.Name}\" />");
            }

            stringBuilder.AppendLine("\t\t\t\t</abp-tab>");
        }

        
        stringBuilder.AppendLine("\t\t\t</abp-tabs>");

        stringBuilder.AppendLine("\t\t</abp-modal-body>");
        
        stringBuilder
            .Append("\t\t<abp-modal-footer ")
            .Append("buttons=\"@(AbpModalButtons.Cancel|AbpModalButtons.Save)\">")
            .AppendLine("</abp-modal-footer>");

        stringBuilder.AppendLine("\t</abp-modal>");

        stringBuilder.AppendLine("</form>");

        File.WriteAllText(htmlFile!, stringBuilder.ToString());

        return true;
    }

    private bool CreateModel()
    {
        if (File.Exists(modelFile))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("using System;")
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Mvc;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;")
            .AppendLine($"using {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}.ViewModels;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name};")
            .AppendLine();

        stringBuilder
            .Append("public class EditModalModel : ")
            .AppendLine($"{entity.ProjectName}PageModel");

        stringBuilder
            .AppendLine("{");

        stringBuilder
            .AppendLine("\t[HiddenInput]")
            .AppendLine("\t[BindProperty(SupportsGet = true)]")
            .AppendLine("\tpublic Guid Id { get; set; }")
            .AppendLine();

        stringBuilder
            .AppendLine("\t[BindProperty]")
            .Append($"\tpublic {viewModel} ViewModel ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        stringBuilder
            .AppendLine($"\tprivate readonly I{appServiceName} _service;")
            .AppendLine();

        stringBuilder
            .Append("\tpublic EditModalModel")
            .AppendLine($"(I{appServiceName} service)")
            .AppendLine("\t{")
            .AppendLine($"\t\t_service = service;")
            .AppendLine("\t}")
            .AppendLine();

        stringBuilder
            .AppendLine("\tpublic async Task OnGetAsync()")
            .AppendLine("\t{")
            .AppendLine("\t\tvar dto = await _service.GetAsync(Id);")
            .AppendLine($"\t\tViewModel = ObjectMapper.Map<{entityDto}, {viewModel}>(dto);")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("\tpublic async Task<IActionResult> OnPostAsync()")
            .AppendLine("\t{")
            .AppendLine($"\t\tvar dto = ObjectMapper.Map<{viewModel}, {createDto}>(ViewModel);")
            .AppendLine($"\t\tawait _service.UpdateAsync(Id, dto);")
            .AppendLine("\t\treturn NoContent();")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(modelFile!, stringBuilder.ToString());

        return true;
    }
}
