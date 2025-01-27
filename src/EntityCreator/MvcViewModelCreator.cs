using Humanizer;
using System.Text;

namespace EntityCreator
{
    public class MvcViewModelCreator(string @namespace, string path)
    {
        private string? entityName;
        private List<PropertyModel>? properties;
        private string? projectName;
        private string? groupName;
        private string? folder;
        private string? folderViewModels;
        private string? mapper;

        public bool Create(string entityName, List<PropertyModel> properties)
        {
            this.entityName = entityName.Dehumanize();
            this.properties = properties;

            projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
            groupName = entityName.Pluralize();
            folder = $"{path}\\src\\{@namespace}.Web";
            folderViewModels = $"{folder}\\Pages\\{groupName}\\{this.entityName}\\ViewModels";
            mapper = $"{projectName}WebAutoMapperProfile";

            if (!Directory.Exists(folderViewModels))
                Directory.CreateDirectory(folderViewModels);

            CreateFilterInputViewModel();

            CreateEditViewModel();

            UpdateMappings();

            return true;
        }

        private bool CreateFilterInputViewModel()
        {
            string artifactName = $"{entityName}FilterInput";
            string filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine($"using {@namespace}.{groupName};")
                .AppendLine();

            stringBuilder
                .Append("namespace ")
                .Append(@namespace)
                .Append(".Web.Pages.")
                .Append(groupName)
                .Append('.')
                .Append(this.entityName)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            stringBuilder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            foreach (var property in properties)
            {
                if (property.IsCollection || 
                    property.Type == "Entity" || 
                    property.Type == "ValueObject" || 
                    property.Type == "AggregatedRoot")
                    continue;

                stringBuilder
                    .Append("\t[Display(Name = \"")
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();

                stringBuilder
                    .Append("\tpublic ")
                    .Append(property.Type)
                    .Append(' ')
                    .Append(property.Name)
                    .Append(" { get; set; }")
                    .AppendLine();
            }

            stringBuilder.AppendLine("}");

            File.WriteAllText(filename, stringBuilder.ToString());

            return true;
        }

        private bool CreateEditViewModel()
        {
            string artifactName = $"CreateEdit{entityName}ViewModel";
            string filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine("using System;")
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine("using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;")
                .AppendLine($"using {@namespace}.{groupName};")
                .AppendLine();

            stringBuilder
                .Append("namespace ")
                .Append(@namespace)
                .Append(".Web.Pages.")
                .Append(groupName)
                .Append('.')
                .Append(this.entityName)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            stringBuilder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            foreach (var property in properties)
            {
                if (property.IsCollection ||
                    property.Type == "Entity" ||
                    property.Type == "ValueObject" ||
                    property.Type == "AggregatedRoot")
                    continue;

                if (property.IsRequired)
                    stringBuilder.AppendLine("\t[Required]");
                
                if (property.Size > 0)
                {
                    stringBuilder
                        .Append("\t[MaxLength(")
                        .Append(property.Size)
                        .AppendLine(")]");
                }

                stringBuilder
                    .Append("\t[Display(Name = \"")
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();
                
                stringBuilder
                    .Append("\tpublic ")
                    .Append(property.Type)
                    .Append(' ')
                    .Append(property.Name)
                    .Append(" { get; set; }")
                    .AppendLine()
                    .AppendLine();
            }

            stringBuilder.AppendLine("}");

            File.WriteAllText(filename, stringBuilder.ToString());
            return true;
        }

        private bool UpdateMappings()
        {
            string filename = $"{folder}\\{mapper}.cs";

            if (!File.Exists(filename))
                return false;

            bool added = false;
            bool isUsing = true;

            StringBuilder stringBuilder = new();

            using StreamReader reader = new(filename);
            string line = reader.ReadLine();
            while (line != null)
            {
                if (!line.Contains("using") && isUsing)
                {
                    isUsing = false;

                    stringBuilder
                        .Append("using ")
                        .Append(@namespace)
                        .Append(".Web.Pages.")
                        .Append(groupName)
                        .Append('.')
                        .Append(this.entityName)
                        .Append(".ViewModels;")
                        .AppendLine();

                    stringBuilder
                        .Append("using ")
                        .Append(@namespace)
                        .Append('.')
                        .Append(groupName)
                        .Append(".Dtos;")
                        .AppendLine();
                }

                if (line.Contains('}') && !added)
                {
                    added = true;

                    stringBuilder.AppendLine();

                    stringBuilder
                        .Append("\t\tCreateMap")
                        .Append($"<{entityName}Dto, CreateEdit{entityName}ViewModel>")
                        .AppendLine("();");

                    stringBuilder
                        .Append("\t\tCreateMap")
                        .Append($"<CreateEdit{entityName}ViewModel, CreateUpdate{entityName}Dto>")
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
}
