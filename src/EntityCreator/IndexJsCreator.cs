using Humanizer;
using System.Text;

namespace EntityCreator;

public class IndexJsCreator(string @namespace, string path)
{
    public bool Create(string entityName, List<PropertyModel> properties)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.Web\\Pages\\{groupName}";
        string filename = $"{folder}\\Index.js";
        string permissions = $"{projectName}.{groupName}";
        
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
            .Append("\tvar l = abp.localization.getResource('")
            .Append(projectName)
            .AppendLine("');");

        stringBuilder
            .Append("\tvar createModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{groupName}/CreateModal')");

        stringBuilder
            .Append("\tvar editModal = new abp.ModalManager(abp.appPath + '")
            .AppendLine($"{groupName}/EditModal')");

        stringBuilder.AppendLine();

        stringBuilder
            .Append("\tvar dataTable = ")
            .AppendLine($"$('#{groupName}Table').DataTable(");

        stringBuilder
            .AppendLine("\t\tabp.libs.datatables.normalizeConfiguration({")
            .AppendLine("\t\t\tserverSide: true,")
            .AppendLine("\t\t\tpaging: true,")
            .AppendLine("\t\t\torder: [[1, \"asc\"]],")
            .AppendLine("\t\t\tsearching: false,")
            .AppendLine("\t\t\tscrollX: true,")
            .Append("\t\t\tajax: abp.libs.datatables.createAjax(")
            .AppendLine($"{endpoint}.getList),");

        stringBuilder
            .AppendLine("\t\t\tcolumnDefs: [");

        // Actions
        stringBuilder
            .AppendLine("\t\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\t\ttitle: l('Actions'),")
            .AppendLine("\t\t\t\t\trowAction: {")
            .AppendLine("\t\t\t\t\t\titems: [");


        // Edit Action
        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t\ttext: l('Edit'),");

        stringBuilder
            .Append("\t\t\t\t\t\t\t\tvisible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Edit'),");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t\taction: function (data) {")
            .AppendLine("\t\t\t\t\t\t\t\t\teditModal.open({ id: data.record.id });")
            .AppendLine("\t\t\t\t\t\t\t\t}");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t},");

        // Delete Action
        
        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t{");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t\ttext: l('Delete'),");

        stringBuilder
            .Append("\t\t\t\t\t\t\t\tvisible: abp.auth.isGranted('")
            .AppendLine($"{permissions}.Delete'),");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t\tconfirmMessage: function (data) {")
            .Append("\t\t\t\t\t\t\t\t\treturn l('")
            .AppendLine($"{entityName}DeletionConfirmationMessage')")
            .AppendLine("\t\t\t\t\t\t\t\t},");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t\t\taction: function (data) {")
            .AppendLine($"\t\t\t\t\t\t\t\t\t{endpoint}")
            .AppendLine("\t\t\t\t\t\t\t\t\t\t.delete(data.record.id)")
            .AppendLine("\t\t\t\t\t\t\t\t\t\t.then(function () {")
            .AppendLine("\t\t\t\t\t\t\t\t\t\t\tabp.notify.info(l('SuccessfullyDeleted'));")
            .AppendLine("\t\t\t\t\t\t\t\t\t\t\tdataTable.ajax.reload();")
            .AppendLine("\t\t\t\t\t\t\t\t\t\t});")
            .AppendLine("\t\t\t\t\t\t\t\t}")
            .AppendLine("\t\t\t\t\t\t\t}");

        stringBuilder
            .AppendLine("\t\t\t\t\t\t]");

        stringBuilder
            .AppendLine("\t\t\t\t\t}");

        stringBuilder
            .Append("\t\t\t\t}");

        // Properties
        foreach (var property in properties)
        {
            stringBuilder.AppendLine(",");
        
            stringBuilder
                .AppendLine("\t\t\t\t{")
                .Append("\t\t\t\t\ttitle: ")
                .AppendLine($"l('{property.Name}'),")
                .Append("\t\t\t\t\tdata: ")
                .Append($"l('{property.Name.Camelize()}')");

            if (property.Type.Equals("DateTime", StringComparison.OrdinalIgnoreCase))
            {
                stringBuilder
                    .AppendLine(",")
                    .AppendLine("\t\t\t\t\tdataFormat: \"date\"");
            }
            else
            {
                stringBuilder.AppendLine("");
            }

            stringBuilder
                .Append("\t\t\t\t}");
        }

        stringBuilder.AppendLine();

        stringBuilder
            .AppendLine("\t\t\t]");

        stringBuilder
            .AppendLine("\t\t})");

        stringBuilder
            .AppendLine("\t);")
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
