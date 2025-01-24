using Humanizer;
using System.Text;

namespace EntityCreator;

public class EFContextUpdater(string @namespace, string path)
{
    public bool Update(string entityName, List<PropertyModel> properties)
    {
        entityName = entityName.Dehumanize();

        string projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
        string artifactName = $"{projectName}DbContext";
        string groupName = entityName.Pluralize();
        string folder = $"{path}\\src\\{@namespace}.EntityFrameworkCore\\EntityFrameworkCore";
        string filename = $"{folder}\\{artifactName}.cs";

        if (!File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        bool modelCreating = false;

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        
        while (line != null)
        {
            if (line.Contains($"public {artifactName}"))
            {
                stringBuilder
                    .Append("\tpublic DbSet<")
                    .Append(entityName)
                    .Append("> ")
                    .Append(groupName)
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
                    .Append(entityName)
                    .AppendLine(">(b =>")
                    .AppendLine("\t\t{")
                    .Append("\t\t\tb.ToTable(")
                    .Append(projectName)
                    .Append("Consts.DbTablePrefix + \"")
                    .Append(groupName)
                    .Append("\", ")
                    .Append(projectName)
                    .AppendLine("Consts.DbSchema);")
                    .AppendLine("\t\t\tb.ConfigureByConvention();");

                foreach (var property in properties)
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

                        if (property.IsRequired)
                        {
                            stringBuilder
                                .Append(".IsRequired()");
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
                                .Append(projectName)
                                .Append("Consts.DbTablePrefix + \"")
                                .Append(property.Name.Pluralize())
                                .Append("\", ")
                                .Append(projectName)
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
                    if (property.Type == "Entity" || property.Type == "AggregatedRoot")
                    {
                        if (property.IsCollection)
                        {
                            stringBuilder
                                .Append("\t\t\tb.HasMany(x => x.")
                                .Append(property.Name.Pluralize())
                                .Append(')')
                                .Append(".WithOne()")
                                .Append(".HasForeignKey(\"")
                                .Append(entityName)
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

            line = reader.ReadLine();
        }
        
        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
