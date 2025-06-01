using EntityCreator.Helpers;
using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class MvcIndexPageCreator(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Web\\Pages\\{entity.Pluralized}\\{entity.Name}";

        CreateDirectory(folder);

        if (!CreatePage())
            return false;

        if (!CreateModel())
            return false;

        if (!CreateStyle())
            return false;

        if (!CreateScript())
            return false;

        return true;
    }

    private bool CreatePage()
    {
        var htmlPage = $"{folder}\\Index.cshtml";

        if (File.Exists(htmlPage))
            return false;

        Initialize();

        // usings
        builder
            .AppendLine("@page")
            .AppendLine($"@using {entity.Namespace}.Permissions")
            .AppendLine("@using Microsoft.AspNetCore.Authorization")
            .AppendLine("@using Microsoft.AspNetCore.Mvc.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Layout")
            .AppendLine($"@using {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}")
            .AppendLine($"@using {entity.Namespace}.Localization")
            .AppendLine($"@using {entity.Namespace}.Web.Menus");

        // model
        builder
            .AppendLine("@model IndexModel");

        // injects
        builder
            .AppendLine("@inject IPageLayout PageLayout")
            .AppendLine($"@inject IHtmlLocalizer<{entity.ProjectName}Resource> L")
            .AppendLine("@inject IAuthorizationService Authorization");

        // code
        builder
            .AppendLine("@{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"PageLayout.Content.Title = L[\"{entity.Name}\"].Value;");

        builder
            .Append(Indentation)
            .AppendLine($"PageLayout.Content.BreadCrumb.Add(L[\"Menu:{entity.Name}\"].Value);");

        builder
            .Append(Indentation)
            .AppendLine($"PageLayout.Content.MenuItemName = {entity.ProjectName}Menus.{entity.Name};");

        indentationLevel--;

        builder
            .AppendLine("}");

        // scripts
        builder
            .AppendLine("@section scripts")
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"<abp-script src=\"/Pages/{entity.Pluralized}/{entity.Name}/index.js\" />");

        indentationLevel--;

        builder
            .AppendLine("}");

        // styles
        builder
            .AppendLine("@section styles")
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"<abp-style src=\"/Pages/{entity.Pluralized}/{entity.Name}/index.css\" />");

        indentationLevel--;

        builder
            .AppendLine("}");

        // header
        builder
            .AppendLine("<div class=\"d-flex\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<div class=\"col\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"<h1 class=\"content-header-title\">@L[\"{entity.Name}\"].Value</h1>");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</div>");

        builder
            .Append(Indentation)
            .AppendLine("<div class=\"col-auto\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"@if (await Authorization.IsGrantedAsync({entity.ProjectName}Permissions.{entity.Name}.Create))");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"<abp-button id=\"New{entity.Name}Button\"");

        indentationLevel += 3;

        builder
            .Append(Indentation)
            .AppendLine($"text=\"@L[\"Create{entity.Name}\"].Value\"");

        builder
            .Append(Indentation)
            .AppendLine($"icon=\"plus\"");

        builder
            .Append(Indentation)
            .AppendLine($"class=\"btn-sm\"");

        builder
            .Append(Indentation)
            .AppendLine($"button-type=\"Primary\" />");

        indentationLevel -= 4;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</div>");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</div>");

        // body
        builder
            .AppendLine("<abp-card>");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<abp-card-body>");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<div class=\"d-flex\">");

        // Search
        var filteredProperties = entity.Properties!
            .Where(p => p.Type == BaseTypeHelper.String)
            .OrderBy(p => p.SearchIndex)
            .ToList();

        if (filteredProperties.Count > 0)
        {
            indentationLevel++;

            builder
                .Append(Indentation)
                .AppendLine("<div class=\"col\">");

            indentationLevel++;

            builder
                .Append(Indentation)
                .AppendLine("<div class=\"input-group\">");

            indentationLevel++;

            builder
                .Append(Indentation)
                .Append("<input class=\"form-control\" ")
                .Append($"placeholder=\"@L[\"Search\"].Value\"")
                .Append($"id=\"{entity.Name}SearchText\"")
                .AppendLine("type=\"search\">");

            builder
                .Append(Indentation)
                .Append("<button class=\"btn btn-primary\" ")
                .Append("type=\"button\"")
                .Append($"id=\"{entity.Name}SearchButton\" >")
                .Append("<i class=\"fa fa-search\"></i>")
                .AppendLine("</button>");

            indentationLevel--;

            builder
                .Append(Indentation)
                .AppendLine("</div>");

            indentationLevel--;

            builder
                .Append(Indentation)
                .AppendLine("</div>");

            if (filteredProperties.Count > 1)
            {
                // filter
                builder
                    .Append(Indentation)
                    .AppendLine("<div class=\"col-auto ms-4\">");

                indentationLevel++;

                builder
                    .Append(Indentation)
                    .AppendLine($"<abp-button abp-collapse-id=\"{entity.Name}Collapse\"");

                indentationLevel += 3;

                builder
                    .Append(Indentation)
                    .AppendLine("button-type=\"Outline_Primary\"");

                builder
                    .Append(Indentation)
                    .AppendLine("icon=\"chevron-down\"");

                builder
                    .Append(Indentation)
                    .AppendLine("text=\"@L[\"Filter\"].Value\" />")
                    .AppendLine();

                indentationLevel -= 4;

                builder
                    .Append(Indentation)
                    .AppendLine("</div>");
            }

            indentationLevel--;

            builder
                .Append(Indentation)
                .AppendLine("</div>");
        }

        builder
            .Append(Indentation)
            .Append($"<abp-dynamic-form ")
            .Append($"abp-model=\"{entity.Name}Filter\" ")
            .Append($"id=\"{entity.Name}Filter\" ")
            .Append($"required-symbols=\"false\" ")
            .AppendLine($"column-size=\"_3\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"<abp-collapse-body id=\"{entity.Name}Collapse\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<div class=\"mt-3\">");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("<abp-form-content />");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</div>");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</abp-collapse-body>");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</abp-dynamic-form>");

        builder
            .Append(Indentation)
            .AppendLine("<hr />");

        // table
        builder
            .Append(Indentation)
            .Append("<abp-table ")
            .Append("striped-rows=\"true\" ")
            .Append($"id=\"{entity.Name}Table\" ")
            .AppendLine("class=\"nowrap\" />");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("</abp-card-body>");

        indentationLevel--;

        builder
            .AppendLine("</abp-card>");

        return WriteToFile(htmlPage);
    }

    private bool CreateModel()
    {
        var modelPage = $"{folder}\\Index.cshtml.cs";

        if (File.Exists(modelPage))
            return false;

        Initialize();

        builder
            .AppendLine("using System.Threading.Tasks;")
            .AppendLine("using Microsoft.AspNetCore.Mvc.RazorPages;")
            .AppendLine($"using {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name}.ViewModels;")
            .AppendLine();

        builder
            .AppendLine($"namespace {entity.Namespace}.Web.Pages.{entity.Pluralized}.{entity.Name};")
            .AppendLine();

        builder
            .AppendLine("public class IndexModel : PageModel");

        builder
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append($"public {entity.Name}FilterInput? {entity.Name}Filter ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("public virtual async Task OnGetAsync()");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("await Task.CompletedTask;");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        builder
            .AppendLine("}");

        return WriteToFile(modelPage);
    }

    private bool CreateStyle()
    {
        var style = $"{folder}\\index.css";

        if (File.Exists(style))
            return false;

        File.WriteAllText(style, "");

        return true;
    }

    private bool CreateScript()
    {
        string script = $"{folder}\\index.js";
        string permissions = $"{entity.ProjectName}.{entity.Name}";

        string[] endpointTree = $"{entity.Namespace}.{entity.Pluralized}.{entity.Name}".Split(".");
        string endpoint = "";

        for (int i = 0; i < endpointTree.Length; i++)
        {
            if (endpoint.Length > 0)
                endpoint += ".";

            endpoint += endpointTree[i].Camelize();
        }

        if (File.Exists(script))
            return false;

        Initialize();

        builder
            .AppendLine("$(function () {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append($"$(\"#{entity.Name}Filter :input\").on('input', function ()")
            .AppendLine(" {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("dataTable.ajax.reload();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("});")
            .AppendLine();

        builder
            .Append(Indentation)
            .Append($"$(\"#{entity.Name}SearchText\").on('input', function ()")
            .AppendLine(" {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("dataTable.ajax.reload();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("});")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("var getFilter = function () {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("var input = {};");

        builder
            .Append(Indentation)
            .AppendLine($"$('#{entity.Name}Filter')");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine(".serializeArray()");

        builder
            .Append(Indentation)
            .AppendLine(".forEach(function (data) {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("if (data.value != '') {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine($"input[abp.utils.toCamelCase(data.name.replace(/{entity.Name}Filter./g, ''))] = data.value;");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("})")
            .AppendLine();

        indentationLevel--;

        builder
            .Append(Indentation)
            .Append($"if ($(\"#{entity.Name}SearchText\").val() !== \"\") ")
            .AppendLine("{");

        indentationLevel++;
        
        builder
            .Append(Indentation)
            .Append("input.search = $(\"#")
            .Append(entity.Name)
            .AppendLine("SearchText\").val();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("return input;");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("};")
            .AppendLine();

        builder
            .Append(Indentation)
            .Append("var l = abp.localization.getResource('")
            .Append(entity.ProjectName)
            .AppendLine("');");

        builder
            .Append(Indentation)
            .AppendLine($"var service = {endpoint};");

        builder
            .Append(Indentation)
            .Append("var createModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{entity.Pluralized}/{entity.Name}/CreateModal')");

        builder
            .Append(Indentation)
            .Append("var editModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{entity.Pluralized}/{entity.Name}/EditModal')");

        builder
            .AppendLine();

        builder
            .Append(Indentation)
            .Append("var dataTable = ")
            .Append($"$('#{entity.Name}Table').DataTable(")
            .AppendLine("abp.libs.datatables.normalizeConfiguration({");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("processing: true,");

        builder
            .Append(Indentation)
            .AppendLine("serverSide: true,");

        builder
            .Append(Indentation)
            .AppendLine("paging: true,");

        builder
            .Append(Indentation)
            .AppendLine("searching: false,");

        builder
            .Append(Indentation)
            .AppendLine("autoWidth: false,");

        builder
            .Append(Indentation)
            .AppendLine("scrollCollapse: true,");

        builder
            .Append(Indentation)
            .AppendLine("order: [[0, \"asc\"]],");

        builder
            .Append(Indentation)
            .Append("ajax: abp.libs.datatables.createAjax(")
            .AppendLine($"service.getList, getFilter),");

        builder
            .Append(Indentation)
            .AppendLine("columnDefs: [");

        // Actions

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("title: l('Actions'),");

        builder
            .Append(Indentation)
            .AppendLine("rowAction: {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("items: [");

        // Edit Action

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("text: l('Edit'),");

        builder
            .Append(Indentation)
            .Append("visible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Edit'),");

        builder
            .Append(Indentation)
            .AppendLine("action: function (data) {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("editModal.open({ id: data.record.id });");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("},");

        // Delete Action

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("text: l('Delete'),");

        builder
            .Append(Indentation)
            .Append("visible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Delete'),");

        builder
            .Append(Indentation)
            .AppendLine("confirmMessage: function (data) {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .Append("return l('")
            .Append($"{entity.Name}DeletionConfirmationMessage', ")
            .AppendLine("data.record.id)");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("},");

        builder
            .Append(Indentation)
            .AppendLine("action: function (data) {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("service.delete(data.record.id)");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine(".then(function () {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("abp.notify.info(l('SuccessfullyDeleted'));");

        builder
            .Append(Indentation)
            .AppendLine("dataTable.ajax.reload();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("});");

        indentationLevel -= 2;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("]");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}");

        indentationLevel--;

        builder
            .Append(Indentation)
            .Append('}');

        // Properties
        foreach (var property in entity.Properties!)
        {
            if (property.IsCollection || BaseTypeHelper.IsEntityType(property.Type!))
                continue;

            builder
                .AppendLine(",");

            builder
                .Append(Indentation)
                .AppendLine("{");

            indentationLevel++;

            builder
                .Append(Indentation)
                .Append("title: ")
                .AppendLine($"l('{entity.Name}{property.Name}'),");

            indentationLevel--;

            builder
                .Append(Indentation)
                .Append("data: ")
                .Append($"l('{property.Name.Camelize()}')");

            if (property.Type!.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                indentationLevel++;

                builder
                    .Append(Indentation)
                    .AppendLine(",")
                    .AppendLine("dataFormat: \"date\"");

                indentationLevel--;
            }
            else
            {
                builder.AppendLine("");
            }

            builder
                .Append(Indentation)
                .Append('}');
        }

        builder
            .AppendLine();

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("]");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}));")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("createModal.onResult(() => dataTable.ajax.reload());")
            .AppendLine();

        builder
            .Append(Indentation)
            .AppendLine("editModal.onResult(() => dataTable.ajax.reload());")
            .AppendLine();

        builder
            .Append(Indentation)
            .Append($"$('#New{entity.Name}Button')")
            .AppendLine(".click((e) => {");

        indentationLevel++;

        builder
            .Append(Indentation)
            .AppendLine("e.preventDefault();");

        builder
            .Append(Indentation)
            .AppendLine("createModal.open();");

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("});");

        builder
            .AppendLine("});");

        return WriteToFile(script);
    }
}
