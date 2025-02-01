using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class EFContextUpdater(EntityModel entity)
{
    string? folder;

    public bool Update()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.EntityFrameworkCore\\EntityFrameworkCore";

        UpdateContext();

        UpdateModule();

        return true;
    }

    public bool UpdateContext()
    {
        string artifactName = $"{entity.ProjectName}DbContext";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        bool modelCreating = false;
        bool isUsing = true;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        while (line != null)
        {
            if (!line.Contains("using") && isUsing)
            {
                stringBuilder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};");

                isUsing = false;
            }

            if (line.Contains($"public {artifactName}"))
            {
                stringBuilder
                    .Append("\tpublic DbSet<")
                    .Append(entity.Name)
                    .Append("> ")
                    .Append(entity.Pluralized)
                    .AppendLine(" { get; set; }")
                    .AppendLine();
            }

            if (line.Contains("protected override void OnModelCreating"))
                modelCreating = true;

            if (line.TrimEnd().EndsWith("}") && modelCreating)
            {
                stringBuilder.AppendLine();

                stringBuilder
                    .Append("\t\tbuilder.Entity<")
                    .Append(entity.Name)
                    .AppendLine(">(b =>")
                    .AppendLine("\t\t{")
                    .Append("\t\t\tb.ToTable(")
                    .Append(entity.ProjectName)
                    .Append("Consts.DbTablePrefix + \"")
                    .Append(entity.Pluralized)
                    .Append("\", ")
                    .Append(entity.ProjectName)
                    .AppendLine("Consts.DbSchema);")
                    .AppendLine("\t\t\tb.ConfigureByConvention();");

                foreach (var property in entity.Properties!)
                {
                    // required
                    if (property.Size > 0 || property.IsRequired)
                    {
                        stringBuilder
                            .Append("\t\t\tb.Property(x => x.")
                            .Append(property.Name)
                            .Append(')');
                        
                        if (property.Size > 0)
                        {
                            stringBuilder
                                .Append(".HasMaxLength(")
                                .Append(property.Size)
                                .Append(')');
                        }

                        stringBuilder
                            .AppendLine(";");
                    }

                    // ValueObejct
                    if (property.Type == "ValueObject")
                    {
                        if (property.IsCollection)
                        {
                            stringBuilder
                                .Append("\t\t\tb.OwnsMany(x => x.")
                                .Append(property.Name.Pluralize())
                                .Append(')')
                                .Append(".ToTable(")
                                .Append(entity.ProjectName)
                                .Append("Consts.DbTablePrefix + \"")
                                .Append(property.Name.Pluralize())
                                .Append("\", ")
                                .Append(entity.ProjectName)
                                .AppendLine("Consts.DbSchema);");
                        }
                        else
                        {
                            stringBuilder
                                .Append("\t\t\tb.OwnsOne(x => x.")
                                .Append(property.Name)
                                .AppendLine(");");
                        }
                    }

                    // Entities
                    if (property.Type == "Entity" || property.Type == "AggregateRoot")
                    {
                        if (property.IsCollection)
                        {
                            stringBuilder
                                .Append("\t\t\tb.HasMany(x => x.")
                                .Append(property.Name.Pluralize())
                                .Append(')')
                                .Append(".WithOne()")
                                .Append(".HasForeignKey(\"")
                                .Append(entity.Name)
                                .AppendLine("Id\");");
                        }
                        else
                        {
                            stringBuilder
                                .Append("\t\t\tb.HasOne(x => x.")
                                .Append(property.Name)
                                .AppendLine(");");
                        }
                    }
                }

                stringBuilder
                    .AppendLine("\t\t});")
                    .AppendLine();

                modelCreating = false;
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine()!;
        }
        
        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }

    public bool UpdateModule()
    {
        string artifactName = $"{entity.ProjectName}EntityFrameworkCoreModule";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        bool addedRepository = false;
        bool isUsing = true;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;

        while (line != null)
        {
            if (!line.Contains("using") && isUsing)
            {
                stringBuilder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};");

                isUsing = false;
            }

            if (line.Contains("options.AddRepository"))
                addedRepository = true;

            if (line.TrimEnd().EndsWith("});") && addedRepository)
            {
                stringBuilder
                    .Append("\t\t\toptions.AddRepository")
                    .Append($"<{entity.Name}")
                    .Append(", ")
                    .Append($"{entity.Name}Repository>")
                    .AppendLine("();");

                addedRepository = false;
            }

            stringBuilder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
