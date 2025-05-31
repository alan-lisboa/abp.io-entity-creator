using System.Text;

namespace EntityCreator.Generators
{
    public abstract class BaseGenerator
    {
        protected StringBuilder builder = new();
        protected short indentationLevel = 0;

        protected string? folder;
        protected string? artifactName;
        protected string? filename;

        public abstract bool Handle();

        protected string Indentation => new('\t', indentationLevel);
       
        protected void Initialize()
        {
            builder = new StringBuilder();
            indentationLevel = 0;
        }

        protected static void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        protected bool WriteToFile()
        {
            try
            {
                File.WriteAllText(filename!, builder.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }

        protected bool WriteToFile(string file)
        {
            try
            {
                File.WriteAllText(file, builder.ToString());
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
