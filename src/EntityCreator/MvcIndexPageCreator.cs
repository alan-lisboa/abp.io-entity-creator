using Humanizer;
using System.Text;

namespace EntityCreator;

public class MvcIndexPageCreator(string @namespace, string path)
{
    private string entityName;
    private string projectName;
    private string groupName;
    private string folder;
    private string htmlPage;
    private string modelPage;
    private List<PropertyModel> properties;

    public bool Create(string entityName, List<PropertyModel> properties)
    {
        this.entityName = entityName.Dehumanize();
        this.properties = properties;

        projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        groupName = entityName.Pluralize();
        folder = $"{path}\\src\\{@namespace}.Web\\Pages\\{groupName}\\{this.entityName}";
        htmlPage = $"{folder}\\Index.cshtml";
        modelPage = $"{folder}\\Index.chtml.cs";
        
        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

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
        if (File.Exists(htmlPage))
            return false;

        StringBuilder stringBuilder = new();

        // usings
        stringBuilder
            .AppendLine("@page")
            .AppendLine($"@using {@namespace}.Permissions")
            .AppendLine("@using Microsoft.AspNetCore.Authorization")
            .AppendLine("@using Microsoft.AspNetCore.Mvc.Localization")
            .AppendLine("@using Volo.Abp.AspNetCore.Mvc.UI.Layout")
            .AppendLine($"@using {@namespace}.Web.Pages.{groupName}.{entityName}")
            .AppendLine($"@using {@namespace}.Localization")
            .AppendLine($"@using {@namespace}.Web.Menus");

        // model
        stringBuilder
            .AppendLine("@model IndexModel");

        // injects
        stringBuilder
            .AppendLine("@inject IPageLayout PageLayout")
            .AppendLine($"@inject IHtmlLocalizer<{projectName}Resource> L")
            .AppendLine("@inject IAuthorizationService Authorization");

        // code
        stringBuilder
            .AppendLine("@{")
            .AppendLine($"\tPageLayout.Content.Title = L[\"{entityName}\"].Value;")
            .AppendLine($"\tPageLayout.Content.BreadCrumb.Add(L[\"Menu:{entityName}\"].Value);")
            .AppendLine($"\tPageLayout.Content.MenuItemName = {projectName}Menus.{entityName};")
            .AppendLine("}");

        // scripts
        stringBuilder
            .AppendLine("@section scripts")
            .AppendLine("{")
            .AppendLine($"\t<abp-script src=\"/Pages/{groupName}/{entityName}/index.js\" />")
            .AppendLine("}");

        // styles
        stringBuilder
            .AppendLine("@section styles")
            .AppendLine("{")
            .AppendLine($"\t<abp-style src=\"/Pages/{groupName}/{entityName}/index.css\" />")
            .AppendLine("}");

        // toolbar
        stringBuilder
            .AppendLine("@section content_toolbar")
            .AppendLine("{");

        stringBuilder
            .AppendLine($"\t<abp-button abp-collapse-id=\"{entityName}Collapse\"")
            .AppendLine("\t\t\t\tbutton-type=\"Light\"")
            .AppendLine("\t\t\t\ticon=\"filter\"")
            .AppendLine("\t\t\t\tclass=\"mx-3\"")
            .AppendLine("\t\t\t\ttext=\"@L[\"TableFilter\"].Value\" />")
            .AppendLine();
        
        stringBuilder
            .AppendLine($"\t@if (await Authorization.IsGrantedAsync({projectName}Permissions.{entityName}.Create))")
            .AppendLine("\t{")
            .AppendLine($"\t\t<abp-button id=\"New{entityName}Button\" text=\"@L[\"Create{entityName}\"].Value\"")
            .AppendLine($"\t\t\t\t\ticon=\"plus\"")
            .AppendLine($"\t\t\t\t\tbutton-type=\"Primary\" />")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        // body
        stringBuilder.AppendLine("<abp-card>");
        
        stringBuilder.AppendLine("\t<abp-card-body>");

        // filter
        stringBuilder
            .Append($"\t\t<abp-dynamic-form ")
            .Append($"abp-model=\"{entityName}Filter\" ")
            .Append($"id=\"{entityName}Filter\" ")
            .Append($"required-symbols=\"false\" ")
            .AppendLine($"column-size=\"_3\">");

        stringBuilder
            .AppendLine($"\t\t\t<abp-collapse-body id=\"{entityName}Collapse\">");

        stringBuilder
            .AppendLine("\t\t\t\t<abp-form-content />");

        stringBuilder
            .AppendLine("\t\t\t</abp-collapse-body>");

        stringBuilder
            .AppendLine("\t\t</abp-dynamic-form>");

        stringBuilder.AppendLine("\t\t<hr />");

        // table
        stringBuilder
            .Append("\t\t<abp-table ")
            .Append("striped-rows=\"true\" ")
            .Append($"id=\"{groupName}Table\" ")
            .AppendLine("class=\"nowrap\" />");
        
        stringBuilder.AppendLine("\t</abp-card-body>");

        stringBuilder.AppendLine("</abp-card>");
        
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
            .AppendLine("using Microsoft.AspNetCore.Mvc.RazorPages;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {@namespace}.Web.Pages.{groupName}.{entityName};")
            .AppendLine();

        stringBuilder
            .AppendLine("public class IndexModel : PageModel");
        
        stringBuilder
            .AppendLine("{");

        stringBuilder
            .Append($"\tpublic {entityName}FilterInput {entityName}Filter ")
            .AppendLine("{ get; set; }")
            .AppendLine();

        stringBuilder
            .AppendLine("\tpublic virtual async Task OnGetAsync()")
            .AppendLine("\t{")
            .AppendLine("\t\tawait Task.CompletedTask;")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(modelPage, stringBuilder.ToString());

        return true;
    }

    private bool CreateStyle()
    {
        string filename = $"{folder}\\index.css";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        File.WriteAllText(filename, "");

        return true;
    }

    private bool CreateScript()
    {
        string filename = $"{folder}\\index.js";
        string permissions = $"{projectName}.{entityName}";

        string[] endpointTree = $"{@namespace}.{groupName}.{entityName}".Split(".");
        string endpoint = "";

        for (int i = 0; i < endpointTree.Length; i++)
        {
            if (endpoint.Length > 0)
                endpoint += ".";

            endpoint += endpointTree[i].Camelize();
        }

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        stringBuilder
            .AppendLine("$(function () {");

        stringBuilder
            .Append($"\t$(\"#{entityName}Filter :input\").on('input', function ()")
            .AppendLine(" {")
            .AppendLine("\t\tdataTable.ajax.reload();")
            .AppendLine("\t});")
            .AppendLine();

        stringBuilder
            .AppendLine("\tvar getFilter = function () {")
            .AppendLine("\t\tvar input = {};")
            .AppendLine($"\t\t$(#{entityName}Filter)")
            .AppendLine("\t\t\t.serializeArray()")
            .AppendLine("\t\t\t.forEach(function (data) {")
            .AppendLine("\t\t\t\tif (data.value != '') {")
            .AppendLine($"\t\t\t\t\tinput[abp.utils.toCamelCase(data.name.replace(/{entityName}Filter./g, ''))] = data.value;")
            .AppendLine("\t\t\t\t}")
            .AppendLine("\t\t\t})")
            .AppendLine("\t\treturn input;")
            .AppendLine("\t};")
            .AppendLine();

        stringBuilder
            .Append("\tvar l = abp.localization.getResource('")
            .Append(projectName)
            .AppendLine("');");

        stringBuilder
            .AppendLine($"\tvar service = {endpoint};");

        stringBuilder
            .Append("\tvar createModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{groupName}/{entityName}/CreateModal')");

        stringBuilder
            .Append("\tvar editModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{groupName}/{entityName}/EditModal')");

        stringBuilder.AppendLine();

        stringBuilder
            .Append("\tvar dataTable = ")
            .Append($"$('#{entityName}Table').DataTable(")
            .AppendLine("abp.libs.datatables.normalizeConfiguration({");

        stringBuilder
            .AppendLine("\t\tprocessing: true,")
            .AppendLine("\t\tserverSide: true,")
            .AppendLine("\t\tpaging: true,")
            .AppendLine("\t\tsearching: false,")
            .AppendLine("\t\tautoWidth: false,")
            .AppendLine("\t\tscrollCollapse: true,")
            .AppendLine("\t\torder: [[0, \"asc\"]],")
            .Append("\t\tajax: abp.libs.datatables.createAjax(")
            .AppendLine($"service.getList, getFilter),");

        stringBuilder
            .AppendLine("\t\tcolumnDefs: [");

        // Actions
        stringBuilder
            .AppendLine("\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\ttitle: l('Actions'),")
            .AppendLine("\t\t\t\trowAction: {")
            .AppendLine("\t\t\t\t\titems: [");


        // Edit Action
        stringBuilder
            .AppendLine("\t\t\t\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\ttext: l('Edit'),");

        stringBuilder
            .Append("\t\t\t\t\t\t\tvisible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Update'),");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\taction: function (data) {")
            .AppendLine("\t\t\t\t\t\t\t\teditModal.open({ id: data.record.id });")
            .AppendLine("\t\t\t\t\t\t\t}");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t},");

        // Delete Action

        stringBuilder
            .AppendLine("\t\t\t\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\ttext: l('Delete'),");

        stringBuilder
            .Append("\t\t\t\t\t\t\tvisible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Delete'),");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\tconfirmMessage: function (data) {")
            .Append("\t\t\t\t\t\t\t\treturn l('")
            .Append($"{entityName}DeletionConfirmationMessage', ")
            .AppendLine("data.record.id)")
            .AppendLine("\t\t\t\t\t\t\t},");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\taction: function (data) {")
            .AppendLine("\t\t\t\t\t\t\t\tservice.delete(data.record.id)")
            .AppendLine("\t\t\t\t\t\t\t\t\t.then(function () {")
            .AppendLine("\t\t\t\t\t\t\t\t\t\tabp.notify.info(l('SuccessfullyDeleted'));")
            .AppendLine("\t\t\t\t\t\t\t\t\t\tdataTable.ajax.reload();")
            .AppendLine("\t\t\t\t\t\t\t\t\t});")
            .AppendLine("\t\t\t\t\t\t\t}")
            .AppendLine("\t\t\t\t\t\t}");

        stringBuilder
            .AppendLine("\t\t\t\t\t]");

        stringBuilder
            .AppendLine("\t\t\t\t}");

        stringBuilder
            .Append("\t\t\t}");

        // Properties
        foreach (var property in properties)
        {
            stringBuilder.AppendLine(",");

            stringBuilder
                .AppendLine("\t\t\t{")
                .Append("\t\t\t\ttitle: ")
                .AppendLine($"l('{entityName}{property.Name}'),")
                .Append("\t\t\t\tdata: ")
                .Append($"l('{property.Name.Camelize()}')");

            if (property.Type.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                stringBuilder
                    .AppendLine(",")
                    .AppendLine("\t\t\t\tdataFormat: \"date\"");
            }
            else
            {
                stringBuilder.AppendLine("");
            }

            stringBuilder
                .Append("\t\t\t}");
        }

        stringBuilder.AppendLine();

        stringBuilder
            .AppendLine("\t\t]");

        stringBuilder
            .AppendLine("\t}));")
            .AppendLine();

        stringBuilder
            .AppendLine("\tcreateModal.onResult(() => dataTable.ajax.reload());")
            .AppendLine();

        stringBuilder
            .AppendLine("\teditModal.onResult(() => dataTable.ajax.reload());")
            .AppendLine();

        stringBuilder
            .Append($"\t$('#New{entityName}Button')")
            .AppendLine(".click((e) => {")
            .AppendLine("\t\te.preventDefault();")
            .AppendLine("\t\tcreateModal.open();")
            .AppendLine("\t});");

        stringBuilder
            .AppendLine("});");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
