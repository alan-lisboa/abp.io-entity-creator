using EntityCreator.Helpers;
using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators
{
    public class MvcViewModelCreator(EntityModel entity) : BaseGenerator()
    {
        private string? folderViewModels;
        private string? mapper;

        public override bool Handle()
        {
            folder = $"{entity.Location}\\src\\{entity.Namespace}.Web";
            folderViewModels = $"{folder}\\Pages\\{entity.Pluralized}\\{entity.Name}\\ViewModels";
            mapper = $"{entity.ProjectName}WebAutoMapperProfile";

            CreateDirectory(folderViewModels);

            CreateFilterInputViewModel();

            CreateEditViewModel();

            CreateAggregatedViewModel();

            UpdateMappings();

            return true;
        }

        private bool CreateFilterInputViewModel()
        {
            artifactName = $"{entity.Name}FilterInput";
            filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            Initialize();

            builder
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                .AppendLine();

            builder
                .Append("namespace ")
                .Append(entity.Namespace)
                .Append(".Web.Pages.")
                .Append(entity.Pluralized)
                .Append('.')
                .Append(entity.Name)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            builder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            indentationLevel++;

            foreach (var property in entity.Properties!)
            {
                if (property.IsCollection || BaseTypeHelper.IsEntityType(property.Type!))
                    continue;

                builder
                    .Append(Indentation)
                    .Append("[Display(Name = \"")
                    .Append(entity.Name)
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();

                builder
                    .Append(Indentation)
                    .Append("public ")
                    .Append(property.Type);

                if (property.Type == BaseTypeHelper.String)
                    builder.Append('?');

                builder
                    .Append(' ')
                    .Append(property.Name)
                    .AppendLine(" { get; set; }")
                    .AppendLine();
            }

            builder.AppendLine("}");

            indentationLevel--;

            return WriteToFile();
        }

        private bool CreateEditViewModel()
        {
            artifactName = $"CreateEdit{entity.Name}ViewModel";
            filename = $"{folderViewModels}\\{artifactName}.cs";

            if (File.Exists(filename))
                return false;

            Initialize();

            builder
                .AppendLine("using System;")
                .AppendLine("using System.ComponentModel.DataAnnotations;")
                .AppendLine("using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;")
                .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                .AppendLine();

            builder
                .Append("namespace ")
                .Append(entity.Namespace)
                .Append(".Web.Pages.")
                .Append(entity.Pluralized)
                .Append('.')
                .Append(entity.Name)
                .Append(".ViewModels")
                .AppendLine(";")
                .AppendLine();

            builder
                .Append("public class ")
                .Append(artifactName)
                .AppendLine()
                .AppendLine("{");

            indentationLevel++;

            foreach (var property in entity.Properties!)
            {
                string propertyType = property.Type!;

                if (BaseTypeHelper.IsAggregatedChild(property.Type!))
                    propertyType = $"{entity.Name}{property.Name!}ViewModel";

                if (property.IsCollection)
                    propertyType = $"List<{propertyType}>";

                if (!property.IsRequired && (BaseTypeHelper.IsNullable(property.Type!) || property.IsCollection))
                    propertyType += "?";

                if (property.IsRequired)
                {
                    builder
                        .Append(Indentation)
                        .AppendLine("[Required]");
                }
                    
                
                if (property.Size > 0)
                {
                    builder
                        .Append(Indentation)
                        .Append("[MaxLength(")
                        .Append(property.Size)
                        .AppendLine(")]");
                }

                builder
                    .Append(Indentation)
                    .Append("[Display(Name = \"")
                    .Append(entity.Name)
                    .Append(property.Name)
                    .Append("\")]")
                    .AppendLine();
                
                builder
                    .Append(Indentation)
                    .Append("public ")
                    .Append(propertyType);

                builder
                    .Append(' ')
                    .Append(property.Name)
                    .Append(" { get; set; }")
                    .AppendLine()
                    .AppendLine();
            }

            indentationLevel--;

            builder.AppendLine("}");

            return WriteToFile();
        }

        private bool CreateAggregatedViewModel()
        {
            foreach (var entityProperty in entity.Properties!)
            {
                if (!BaseTypeHelper.IsAggregatedChild(entityProperty.Type!))
                    continue;

                artifactName = $"{entity.Name}{entityProperty.Name}ViewModel";
                filename = $"{folderViewModels}\\{artifactName}.cs";

                if (File.Exists(filename))
                    return false;

                Initialize();

                builder
                    .AppendLine("using System;")
                    .AppendLine("using System.ComponentModel.DataAnnotations;")
                    .AppendLine("using Volo.Abp.AspNetCore.Mvc.UI.Bootstrap.TagHelpers.Form;")
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};")
                    .AppendLine();

                builder
                    .Append("namespace ")
                    .Append(entity.Namespace)
                    .Append(".Web.Pages.")
                    .Append(entity.Pluralized)
                    .Append('.')
                    .Append(entity.Name)
                    .Append(".ViewModels")
                    .AppendLine(";")
                    .AppendLine();

                builder
                    .Append("public class ")
                    .Append(artifactName)
                    .AppendLine()
                    .AppendLine("{");

                indentationLevel++;

                foreach (var property in entityProperty.Properties!)
                {
                    string propertyType = property.Type!;

                    if (BaseTypeHelper.IsEntityType(property.Type!))
                        continue;

                    if (!property.IsRequired && property.Type! == BaseTypeHelper.String)
                        propertyType += "?";

                    builder
                        .Append(Indentation)
                        .Append("public ")
                        .Append(propertyType)
                        .Append(' ').Append(property.Name)
                        .Append(" { get; set; }")
                        .AppendLine();
                }

                indentationLevel--;

                builder.AppendLine("}");

                WriteToFile();
            }
            
            return true;
        }

        private bool UpdateMappings()
        {
            filename = $"{folder}\\{mapper}.cs";

            if (!File.Exists(filename))
                return false;

            Initialize();

            bool added = false;
            bool isUsing = true;

            using StreamReader reader = new(filename);
            string line = reader.ReadLine()!;
            while (line != null)
            {
                if (!line.Contains("using") && isUsing)
                {
                    isUsing = false;

                    builder
                        .Append("using ")
                        .Append(entity.Namespace)
                        .Append(".Web.Pages.")
                        .Append(entity.Pluralized)
                        .Append('.')
                        .Append(entity.Name)
                        .Append(".ViewModels;")
                        .AppendLine();

                    builder
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

                    indentationLevel = 2;

                    builder
                        .AppendLine();

                    builder
                        .Append(Indentation)
                        .Append("CreateMap")
                        .Append($"<{entity.Name}Dto, CreateEdit{entity.Name}ViewModel>")
                        .AppendLine("();");

                    builder
                        .Append(Indentation)
                        .Append("CreateMap")
                        .Append($"<CreateEdit{entity.Name}ViewModel, CreateUpdate{entity.Name}Dto>")
                        .AppendLine("();");

                    foreach (var property in entity.Properties!)
                    {
                        if (BaseTypeHelper.IsAggregatedChild(property.Type!))
                        {
                            builder
                                .Append(Indentation)
                                .Append("CreateMap")
                                .Append($"<{entity.Name}{property.Name}Dto, {entity.Name}{property.Name}ViewModel>")
                                .AppendLine("();");

                            builder
                                .Append(Indentation)
                                .Append("CreateMap")
                                .Append($"<{entity.Name}{property.Name}ViewModel, {entity.Name}{property.Name}Dto>")
                                .AppendLine("();");
                        }
                    }

                    indentationLevel = 0;
                }

                builder.AppendLine(line);

                line = reader.ReadLine()!;
            }

            reader.Dispose();

            return WriteToFile();
        }
    }
}
