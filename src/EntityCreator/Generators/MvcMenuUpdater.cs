using EntityCreator.Models;
using System.Text;

namespace EntityCreator.Generators
{
    public class MvcMenuUpdater(EntityModel entity)
    {
        private string? folder;

        public bool Update()
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
            string artifactName = $"{entity.ProjectName}Menus";
            string filename = $"{folder}\\{artifactName}.cs";

            if (!File.Exists(filename))
                return false;

            using StreamReader readerForContent = new(filename);
            var content = readerForContent.ReadToEnd();
            bool hasApplicationMenu = content.Contains("public const string Application");
            readerForContent.Close();

            StringBuilder stringBuilder = new();

            using StreamReader reader = new(filename);
            string? line = reader.ReadLine();
            
            while (line != null) 
            {
                if (line.TrimEnd().EndsWith('}'))
                {
                    if (!hasApplicationMenu)
                    {
                        stringBuilder
                            .AppendLine("\tpublic const string Application = Prefix + \".Application\";");
                    }

                    stringBuilder
                        .Append("\tpublic const string ")
                        .Append(entity.Name)
                        .Append(" = Prefix + \".")
                        .Append(entity.Name)
                        .AppendLine("\";");
                }
                
                stringBuilder.AppendLine(line);
                
                line = reader.ReadLine();
            }

            reader.Close();

            File.WriteAllText(filename, stringBuilder.ToString());

            return true;
        }

        private bool UpdateMenuContributor()
        {
            string artifactName = $"{entity.ProjectName}MenuContributor";
            string filename = $"{folder}\\{artifactName}.cs";

            if (!File.Exists(filename))
                return false;

            using StreamReader readerForContent = new(filename);
            var content = readerForContent.ReadToEnd();
            bool hasApplicationMenu = content.Contains("var application = new ApplicationMenuItem(");
            readerForContent.Close();

            StringBuilder stringBuilder = new();

            using StreamReader reader = new(filename);
            string? line = reader.ReadLine();

            while (line != null)
            {
                if (line.Contains("return Task.CompletedTask;"))
                {
                    if (!hasApplicationMenu)
                    {
                        stringBuilder
                            .AppendLine("\t\t//Application")
                            .AppendLine("\t\tvar application = new ApplicationMenuItem(")
                            .AppendLine($"\t\t\t{entity.ProjectName}Menus.Application,")
                            .AppendLine("\t\t\tl[\"Menu:Application\"],")
                            .AppendLine("\t\t\ticon: \"fa-solid fa-bars-staggered\",")
                            .AppendLine("\t\t\torder: 6")
                            .AppendLine("\t\t);")
                            .AppendLine()
                            .AppendLine("\t\tcontext.Menu.AddItem(application);")
                            .AppendLine();
                    }

                    stringBuilder
                        .AppendLine("\t\tapplication.AddItem(")
                        .AppendLine($"\t\t\tnew ApplicationMenuItem(")
                        .Append($"\t\t\t\t{entity.ProjectName}Menus.{entity.Name}, ")
                        .Append($"l[\"Menu:{entity.Name}\"], ")
                        .Append($"url: \"/{entity.Pluralized}/{entity.Name}\", ")
                        .AppendLine($"icon: \"fa-solid fa-arrow-up-right-from-square\"")
                        .AppendLine("\t\t\t)")
                        .Append("\t\t).RequirePermissions(")
                        .Append($"{entity.ProjectName}Permissions.")
                        .Append(entity.Name)
                        .AppendLine(".Default);")
                        .AppendLine();
                }

                stringBuilder.AppendLine(line);

                line = reader.ReadLine();
            }

            reader.Close();

            File.WriteAllText(filename, stringBuilder.ToString());

            return true;
        }
    }
}
