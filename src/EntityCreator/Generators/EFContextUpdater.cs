using EntityCreator.Models;
using Humanizer;
using System.Text;

namespace EntityCreator.Generators;

public class EFContextUpdater(EntityModel entity) : BaseGenerator
{
    public override bool Handle()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.EntityFrameworkCore\\EntityFrameworkCore";

        UpdateContext();

        UpdateModule();

        return true;
    }

    public bool UpdateContext()
    {
        artifactName = $"{entity.ProjectName}DbContext";
        filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        Initialize();

        bool modelCreating = false;
        bool isUsing = true;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;
        
        while (line != null)
        {
            if (!line.Contains("using") && isUsing)
            {
                builder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};");

                isUsing = false;
            }

            if (line.Contains($"public {artifactName}"))
            {
                indentationLevel++;

                builder
                    .Append(Indentation)
                    .Append("public DbSet<")
                    .Append(entity.Name)
                    .Append("> ")
                    .Append(entity.Pluralized)
                    .AppendLine(" { get; set; }")
                    .AppendLine();
            }

            if (line.Contains("protected override void OnModelCreating"))
                modelCreating = true;

            if (line.TrimEnd().EndsWith('}') && modelCreating)
            {
                builder.AppendLine();

                builder
                    .Append("\t\tbuilder.Entity<")
                    .Append(entity.Name)
                    .AppendLine(">(b =>");

                builder
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
                        builder
                            .Append("\t\t\tb.Property(x => x.")
                            .Append(property.Name)
                            .Append(')');
                        
                        if (property.Size > 0)
                        {
                            builder
                                .Append(".HasMaxLength(")
                                .Append(property.Size)
                                .Append(')');
                        }

                        builder
                            .AppendLine(";");
                    }

                    // ValueObejct
                    if (property.Type == "ValueObject")
                    {
                        if (property.IsCollection)
                        {
                            builder
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
                            builder
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
                            builder
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
                            builder
                                .Append("\t\t\tb.HasOne(x => x.")
                                .Append(property.Name)
                                .AppendLine(");");
                        }
                    }
                }

                builder
                    .AppendLine("\t\t});")
                    .AppendLine();

                modelCreating = false;
            }

            builder.AppendLine(line);

            line = reader.ReadLine()!;
        }
        
        reader.Dispose();

        File.WriteAllText(filename, builder.ToString());

        return true;
    }

    public bool UpdateModule()
    {
        artifactName = $"{entity.ProjectName}EntityFrameworkCoreModule";
        filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        Initialize();

        bool addedRepository = false;
        bool isUsing = true;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine()!;

        while (line != null)
        {
            if (!line.Contains("using") && isUsing)
            {
                builder
                    .AppendLine($"using {entity.Namespace}.{entity.Pluralized};");

                isUsing = false;
            }

            if (line.Contains("options.AddRepository"))
                addedRepository = true;

            if (line.TrimEnd().EndsWith("});") && addedRepository)
            {
                indentationLevel = 3;

                builder
                    .Append(Indentation)
                    .Append("options.AddRepository")
                    .Append($"<{entity.Name}")
                    .Append(", ")
                    .Append($"{entity.Name}Repository>")
                    .AppendLine("();");

                addedRepository = false;
            }

            builder.AppendLine(line);

            line = reader.ReadLine()!;
        }

        reader.Dispose();

        return WriteToFile();
    }
}
