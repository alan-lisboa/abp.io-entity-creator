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

        using StreamReader reader = new(filename);
        string line = reader.ReadLine();
        while (line != null)
        {
            if (line.Contains("#region Entities from the modules"))
            {
                stringBuilder
                    .Append("\tpublic DbSet<")
                    .Append(entityName)
                    .Append("> ")
                    .Append(groupName)
                    .AppendLine(" { get; set; }")
                    .AppendLine();
            }

            stringBuilder.AppendLine(line);

            if (line.Contains("/* Configure your own tables/entities inside here */"))
            {
                stringBuilder.AppendLine();

                stringBuilder
                    .Append("\t\tbuilder.Entity<")
                    .Append(entityName)
                    .AppendLine(">(e =>")
                    .AppendLine("\t\t{")
                    .Append("\t\t\te.ToTable(")
                    .Append(projectName)
                    .Append("Consts.DbTablePrefix + \"")
                    .Append(groupName)
                    .Append("\", ")
                    .Append(projectName)
                    .AppendLine("Consts.DbSchema);")
                    .AppendLine("\t\t\te.ConfigureByConvention();");

                foreach (var property in properties)
                {
                    if (property.Size > 0 || property.IsRequired)
                    {
                        stringBuilder
                            .Append("\t\t\te.Property(p => p.")
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
                }

                stringBuilder
                    .AppendLine("\t\t});")
                    .AppendLine();
            }

            line = reader.ReadLine();
        }
        
        reader.Dispose();

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
