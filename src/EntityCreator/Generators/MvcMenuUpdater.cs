using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators
{
    public class MvcMenuUpdater(EntityModel entity) : BaseGenerator
    {
        public override bool Handle()
        {
            folder = $"{entity.Location}\\src\\{entity.Namespace}.Web\\Menus";

            if (!Directory.Exists(folder))
                return false;

            UpdateMenu();

            UpdateMenuContributor();

            return true;
        }

        private bool UpdateMenu()
        {
            artifactName = $"{entity.ProjectName}Menus";
            filename = $"{folder}\\{artifactName}.cs";

            if (!File.Exists(filename))
                return false;

            Initialize();

            using StreamReader readerForContent = new(filename);
            var content = readerForContent.ReadToEnd();
            bool hasApplicationMenu = content.Contains("public const string Application");

            readerForContent.Close();

            using StreamReader reader = new(filename);
            string? line = reader.ReadLine();
            
            while (line != null) 
            {
                if (line.TrimEnd().EndsWith('}'))
                {
                    if (!hasApplicationMenu)
                    {
                        builder
                            .AppendLine("\tpublic const string Application = Prefix + \".Application\";");
                    }

                    indentationLevel++;
                    
                    builder
                        .Append(Indentation)
                        .Append("public const string ")
                        .Append(entity.Name)
                        .Append(" = Prefix + \".")
                        .Append(entity.Name)
                        .AppendLine("\";");
                }
                
                builder
                    .AppendLine(line);
                
                line = reader.ReadLine();
            }

            reader.Close();

            return WriteToFile();
        }

        private bool UpdateMenuContributor()
        {
            artifactName = $"{entity.ProjectName}MenuContributor";
            filename = $"{folder}\\{artifactName}.cs";

            if (!File.Exists(filename))
                return false;
            
            Initialize();

            using StreamReader readerForContent = new(filename);
            var content = readerForContent.ReadToEnd();
            bool hasApplicationMenu = content.Contains("var application = new ApplicationMenuItem(");
            
            readerForContent.Close();

            using StreamReader reader = new(filename);
            string? line = reader.ReadLine();

            while (line != null)
            {
                if (line.Contains("return Task.CompletedTask;"))
                {
                    if (!hasApplicationMenu)
                    {
                        indentationLevel = 2;

                        builder
                            .Append(Indentation)
                            .AppendLine("//Application");

                        builder
                            .Append(Indentation)
                            .AppendLine("var application = new ApplicationMenuItem(");

                        indentationLevel++;

                        builder
                            .Append(Indentation)
                            .AppendLine($"{entity.ProjectName}Menus.Application,");

                        builder
                            .Append(Indentation)
                            .AppendLine("l[\"Menu:Application\"],");

                        builder
                            .Append(Indentation)
                            .AppendLine("icon: \"fa-solid fa-bars-staggered\",");

                        builder
                            .Append(Indentation)
                            .AppendLine("order: 6");

                        indentationLevel--;

                        builder
                            .Append(Indentation)
                            .AppendLine(");")
                            .AppendLine();

                        builder
                            .Append(Indentation)
                            .AppendLine("context.Menu.AddItem(application);")
                            .AppendLine();
                    }

                    builder
                        .Append(Indentation)
                        .AppendLine("application.AddItem(");

                    indentationLevel++;

                    builder
                        .Append(Indentation)
                        .AppendLine($"new ApplicationMenuItem(");

                    indentationLevel++;

                    builder
                        .Append(Indentation)
                        .Append($"{entity.ProjectName}Menus.{entity.Name}, ");

                    builder
                        .Append($"l[\"Menu:{entity.Name}\"], ")
                        .Append($"url: \"/{entity.Pluralized}/{entity.Name}\", ")
                        .AppendLine($"icon: \"fa-solid fa-arrow-up-right-from-square\"");

                    builder
                        .Append(Indentation)
                        .AppendLine(")");

                    indentationLevel--;

                    builder
                        .Append(Indentation)
                        .Append(").RequirePermissions(")
                        .Append($"{entity.ProjectName}Permissions.")
                        .Append(entity.Name)
                        .AppendLine(".Default);")
                        .AppendLine();
                }

                builder.AppendLine(line);

                line = reader.ReadLine();
            }

            reader.Close();

            return WriteToFile();
        }
    }
}
