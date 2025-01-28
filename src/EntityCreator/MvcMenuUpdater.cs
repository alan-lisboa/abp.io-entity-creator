using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator
{
    public class MvcMenuUpdater(string @namespace, string path)
    {
        private string entityName;
        private string projectName;
        private string groupName;
        private string folder;

        public bool Update(string entityName)
        {
            this.entityName = entityName.Dehumanize();
        
            projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
            groupName = entityName.Pluralize();
            folder = $"{path}\\src\\{@namespace}.Web\\Menus";

            if (!Directory.Exists(folder))
                return false;

            UpdateMenu();

            UpdateMenuContributor();

            return true;
        }

        private bool UpdateMenu()
        {
            string artifactName = $"{projectName}Menus";
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
                        .Append(entityName)
                        .Append(" = Prefix + \".")
                        .Append(entityName)
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
            string artifactName = $"{projectName}MenuContributor";
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
                            .AppendLine($"\t\t\t{projectName}Menus.Application,")
                            .AppendLine("\t\t\tl[\"Menu:Application\"],")
                            .AppendLine("\t\t\ticon: \"fa-solid fa-bars-staggered\",")
                            .AppendLine("\t\t\torder: 6")
                            .AppendLine("\t\t);")
                            .AppendLine();
                    }

                    stringBuilder
                        .AppendLine("\t\tapplication.AddItem(")
                        .AppendLine($"\t\t\tnew ApplicationMenuItem(")
                        .Append($"\t\t\t\t{projectName}Menus.{entityName}, ")
                        .Append($"l[\"Menu:{entityName}\"], ")
                        .Append($"url: \"/{entityName}\", ")
                        .AppendLine($"icon: \"fa fa-globe\"")
                        .AppendLine("\t\t\t)")
                        .Append("\t\t).RequirePermissions(")
                        .Append($"{projectName}Permissions.")
                        .Append(entityName)
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
