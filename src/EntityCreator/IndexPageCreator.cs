using Humanizer;
using System.Text;

namespace EntityCreator;

public class IndexPageCreator(string @namespace, string path)
{
    private string entityName;
    private string projectName;
    private string groupName;
    private string folder; 
    private string permissions;
    private string htmlPage;
    private string modelPage;

    public bool Create(string entityName)
    {
        this.entityName = entityName.Dehumanize();

        projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        groupName = entityName.Pluralize();
        folder = $"{path}\\src\\{@namespace}.Web\\Pages\\{groupName}";
        htmlPage = $"{folder}\\Index.cshtml";
        modelPage = $"{folder}\\Index.chtml.cs";
        permissions = $"{projectName}Permissions.{groupName}";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        if (!CreateWeb())
            return false;

        if (!CreateModel())
            return false;

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
            .AppendLine($"@using {@namespace}.Permissions")
            .AppendLine($"@using {@namespace}.Web.Pages.{groupName}")
            .AppendLine("@using Microsoft.AspNetCore.Authorization")
            .AppendLine("@using Microsoft.Extensions.Localization")
            .AppendLine("@model IndexModel");

        stringBuilder
            .Append("@inject IStringLocalizer")
            .AppendLine($"<{projectName}Resource> L")
            .AppendLine("@inject IAuthorizationService AuthorizationService");

        stringBuilder
            .AppendLine("@section scripts")
            .AppendLine("{")
            .Append("\t<abp-script src=\"/Pages")
            .Append($"/{groupName}/Index.js\"")
            .AppendLine(" />")
            .AppendLine("}");

        stringBuilder.AppendLine("<abp-card>");
        stringBuilder.AppendLine("\t<abp-card-header>");
        stringBuilder.AppendLine("\t\t<abp-row>");
        
        stringBuilder.AppendLine("\t\t\t<abp-column size-md=\"_6\">");
        stringBuilder.AppendLine($"\t\t\t\t<abp-card-title>@L[\"{groupName}\"]</abp-card-title>");
        stringBuilder.AppendLine("\t\t\t</abp-column>");
        
        stringBuilder.AppendLine("\t\t\t<abp-column size-md=\"_6\" class=\"text-end\">");
        stringBuilder
            .Append("\t\t\t\t@if (await AuthorizationService.IsGrantedAsync(")
            .Append(permissions)
            .AppendLine(".Create))");
        stringBuilder.AppendLine("\t\t\t\t{");
        
        stringBuilder
            .Append("\t\t\t\t\t<abp-button ")
            .Append($"id=\"New{entityName}Button\" ")
            .Append($"text=\"@L[\"New{entityName}\"].Value\" ")
            .Append("icon=\"plus\"")
            .AppendLine("button-type=\"Primary\" />");

        stringBuilder.AppendLine("\t\t\t\t}");
        stringBuilder.AppendLine("\t\t\t</abp-column>");

        stringBuilder.AppendLine("\t\t</abp-row>");
        stringBuilder.AppendLine("\t</abp-card-header>");

        stringBuilder.AppendLine("\t<abp-card-body>");
        stringBuilder
            .Append("\t\t<abp-table ")
            .Append("striped-rows=\"true\" ")
            .Append($"id=\"{groupName}Table\">")
            .AppendLine("</abp-table>");
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
            .AppendLine("using Microsoft.AspNetCore.Mvc.RazorPages;")
            .AppendLine();

        stringBuilder
            .AppendLine($"namespace {@namespace}.Web.Pages.{groupName};")
            .AppendLine();

        stringBuilder
            .AppendLine("public class IndexModel : PageModel");
        
        stringBuilder
            .AppendLine("{");

        stringBuilder
            .AppendLine("\tpublic void OnGet()");
        stringBuilder
            .AppendLine("\t{")
            .AppendLine("\t}");

        stringBuilder
            .AppendLine("}");

        File.WriteAllText(modelPage, stringBuilder.ToString());

        return true;
    }
}
