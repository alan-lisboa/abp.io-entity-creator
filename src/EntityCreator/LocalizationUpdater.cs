using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator
{
    public class LocalizationUpdater(string @namespace, string path)
    {
        private string entityName;
        private List<PropertyModel> properties;
        private string projectName;
        private string groupName;
        private string folder;
        
        public bool Update(string entityName, List<PropertyModel> properties)
        {
            this.entityName = entityName.Dehumanize();
            this.properties = properties;

            projectName = @namespace[(@namespace.IndexOf(".") + 1)..];
            groupName = entityName.Pluralize();
            folder = $"{path}\\src\\{@namespace}.Domain.Shared\\Localization\\{projectName}";

            if (!Directory.Exists(folder))
                return false;

            var files = Directory.GetFiles(folder);

            foreach (var file in files)
            {
                UpdateLocaliztionFile(file);
            }

            return true;
        }

        private void UpdateLocaliztionFile(string file)
        {
        }
    }
}
