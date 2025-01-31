using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators
{
    public class MvcViewModelCreator(EntityModel entity)
    {
        private string? folder;
        private string? folderViewModels;
        private string? mapper;

        public bool Create()
        {
            folder = $"{entity.Location}\\src\\{entity.Namespace}.Web";
            folderViewModels = $"{folder}\\Pages\\{entity.Pluralized}\\{entity.Name}\\ViewModels";
            mapper = $"{entity.ProjectName}WebAutoMapperProfile";

            if (!Directory.Exists(folderViewModels))
                Directory.CreateDirectory(folderViewModels);

            CreateFilterInputViewModel();

            CreateEditViewModel();

            UpdateMappings();

            return true;
        }

        private bool CreateFilterInputViewModel()
        {
            string artifactName = $"{entity.Name}FilterInput";
            string filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                .AppendLine();

            stringBuilder
                .Append("namespace ")
                .Append(entity.Namespace)
                .Append(".Web.Pages.")
                .Append(entity.Pluralized)
                .Append('.')
                .Append(entity.Name)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            stringBuilder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            foreach (var property in entity.Properties!)
            {
                if (property.IsCollection || 
                    property.Type == BaseTypes.Entity || 
                    property.Type == BaseTypes.ValueObject || 
                    property.Type == BaseTypes.AggregatedRoot)
                    continue;

                stringBuilder
                    .Append("\t[Display(Name = \"")
                    .Append(entity.Name)
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();

                stringBuilder
                    .Append("\tpublic ")
                    .Append(property.Type);

                if (property.Type == BaseTypes.String)
                    stringBuilder.Append('?');

                stringBuilder
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
            string artifactName = $"CreateEdit{entity.Name}ViewModel";
            string filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            StringBuilder stringBuilder = new();

            stringBuilder
                .AppendLine("using System;")
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine("using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;")
                .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                .AppendLine();

            stringBuilder
                .Append("namespace ")
                .Append(entity.Namespace)
                .Append(".Web.Pages.")
                .Append(entity.Pluralized)
                .Append('.')
                .Append(entity.Name)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            stringBuilder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            foreach (var property in entity.Properties!)
            {
                if (property.IsCollection ||
                    property.Type == BaseTypes.Entity ||
                    property.Type == BaseTypes.ValueObject ||
                    property.Type == BaseTypes.AggregatedRoot)
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
                    .Append(entity.Name)
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();
                
                stringBuilder
                    .Append("\tpublic ")
                    .Append(property.Type);

                if (property.Type == BaseTypes.String)
                    stringBuilder.Append('?');

                stringBuilder
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
            string line = reader.ReadLine()!;
            while (line != null)
            {
                if (!line.Contains("using") && isUsing)
                {
                    isUsing = false;

                    stringBuilder
                        .Append("using ")
                        .Append(entity.Namespace)
                        .Append(".Web.Pages.")
                        .Append(entity.Pluralized)
                        .Append('.')
                        .Append(entity.Name)
                        .Append(".ViewModels;")
                        .AppendLine();

                    stringBuilder
                        .Append("using ")
                        .Append(entity.Namespace)
                        .Append('.')
                        .Append(entity.Pluralized)
                        .Append(".Dtos;")
                        .AppendLine();
                }

                if (line.Contains('}') && !added)
                {
                    added = true;

                    stringBuilder.AppendLine();

                    stringBuilder
                        .Append("\t\tCreateMap")
                        .Append($"<{entity.Name}Dto, CreateEdit{entity.Name}ViewModel>")
                        .AppendLine("();");

                    stringBuilder
                        .Append("\t\tCreateMap")
                        .Append($"<CreateEdit{entity.Name}ViewModel, CreateUpdate{entity.Name}Dto>")
                        .AppendLine("();");
                }

                stringBuilder.AppendLine(line);

                line = reader.ReadLine()!;
            }

            reader.Dispose();

            File.WriteAllText(filename, stringBuilder.ToString());

            return true;
        }
    }
}
