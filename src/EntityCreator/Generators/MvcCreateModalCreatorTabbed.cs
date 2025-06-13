using EntityCreator.Helpers;
using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators;

public class MvcCreateModalCreatorTabbed(EntityModel entity) : BaseGenerator
{    
    private string? appServiceName;
    private string? createDto;
    private string? viewModel;

    public override bool Handle()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Web\\Pages\\{entity.Pluralized}\\{entity.Name}";
        
        appServiceName = $"{entity.Name}AppService";
        createDto = $"CreateUpdate{entity.Name}Dto";
        viewModel = $"CreateEdit{entity.Name}ViewModel";

        CreateDirectory(folder);

        if (!CreatePage())
            return false;

        if (!CreateModel())
            return false;

        return true;
    }

    private bool CreatePage()
    {
        var htmlFile = $"{folder}\\CreateModal.cshtml";

        if (File.Exists(htmlFile))
            return false;

        Initialize();

        var mainProperties = entity.Properties!
            .Where(p => !BaseTypeHelper.IsAggregatedChild(p.Type!));

        var secondaryProperties = entity.Properties!
            .Where(p => BaseTypeHelper.IsAggregatedChild(p.Type!));

        // usings
        builder
            .AppendLine("@page")
            .AppendLine("@using Microsoft.AspNetCore.Mvc.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Modal")
            .AppendLine($"@using {entity.Namespace}.Localization");

        // inject
        builder
            .Append("@inject IHtmlLocalizer")
            .AppendLine($"<{entity.ProjectName}Resource> L");

        // model
        builder
            .AppendLine($"@model {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}.CreateModalModel");

        // layout
        builder
            .AppendLine("@{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("Layout = null;");

        indentationLevel--;

        builder
            .AppendLine("}");

        builder
            .Append("<form method=\"post\" ")
            .AppendLine($"action=\"@Url.Page(\"/{entity.Pluralized}/{entity.Name}/CreateModal\")\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<abp-modal>");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append("<abp-modal-header ")
            .Append($"title=\"@L[\"Create{entity.Name}\"].Value\">")
            .AppendLine("</abp-modal-header>");

        builder
            .Append(Indentation)
            .AppendLine("<abp-modal-body>");

        if (secondaryProperties.Any())
        {
            indentationLevel++;

            builder
                .Append(Indentation)
                .AppendLine("<abp-tabs>");

            indentationLevel++;

            builder
                .Append(Indentation)
                .AppendLine("<abp-tab title=\"@L[\"Home\"].Value\">");
        }

        indentationLevel++;

        foreach (var property in mainProperties)
        {
            builder
                .Append(Indentation)
                .Append("<abp-input ")
                .AppendLine($"asp-for=\"ViewModel.{property.Name}\" />");
        }

        indentationLevel--;

        if (secondaryProperties.Any())
        {
            builder
                .Append(Indentation)
                .AppendLine("</abp-tab>");

            foreach (var property in secondaryProperties)
            {
                builder
                    .Append(Indentation)
                    .Append("<abp-tab title=\"@L[\"")
                    .Append(property.Name)
                    .AppendLine("\"].Value\">");

                indentationLevel++;

                foreach (var subProperty in property.Properties!)
                {
                    builder
                        .Append(Indentation)
                        .Append("<abp-input ")
                        .AppendLine($"asp-for=\"ViewModel.{property.Name}.{subProperty.Name}\" />");
                }

                indentationLevel--;

                builder
                    .Append(Indentation)
                    .AppendLine("</abp-tab>");

                indentationLevel--;
            }

            builder
                .Append(Indentation)
                .AppendLine("</abp-tabs>");

            indentationLevel--;
        }

        builder
            .Append(Indentation)
            .AppendLine("</abp-modal-body>");

        builder
            .Append(Indentation)
            .Append("<abp-modal-footer ")
            .Append("buttons=\"@(AbpModalButtons.Cancel|AbpModalButtons.Save)\">")
            .AppendLine("</abp-modal-footer>");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</abp-modal>");

        indentationLevel--;

        builder
            .AppendLine("</form>");

        return WriteToFile(htmlFile);
    }

    private bool CreateModel()
    {
        var modelFile = $"{folder}\\CreateModal.cshtml.cs";

        if (File.Exists(modelFile))
            return false;

        Initialize();

        // usings
        builder
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Mvc;")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
            .AppendLine($"using {entity.Namespace}.{entity.Pluralized}.Dtos;")
            .AppendLine($"using {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}.ViewModels;")
            .AppendLine();

        // namespace
        builder
            .AppendLine($"namespace {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name};")
            .AppendLine();

        builder
            .Append("public class CreateModalModel : ")
            .AppendLine($"{entity.ProjectName}PageModel");

        builder
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("[BindProperty]");

        builder
            .Append(Indentation)
            .Append($"public {viewModel} ViewModel ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine($"private readonly I{appServiceName} _service;")
            .AppendLine();

        builder
            .Append(Indentation)
            .Append("public CreateModalModel")
            .AppendLine($"(I{appServiceName} service)");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"_service = service;");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("public virtual async Task<IActionResult> OnPostAsync()");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"var dto = ObjectMapper.Map<{viewModel}, {createDto}>(ViewModel);");

        builder
            .Append(Indentation)
            .AppendLine("await _service.CreateAsync(dto);");

        builder
            .Append(Indentation)
            .AppendLine("return NoContent();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        builder
            .AppendLine("}");

        return WriteToFile(modelFile);
    }
}
