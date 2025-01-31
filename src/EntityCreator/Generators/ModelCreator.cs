using EntityCreator.Models;
using Humanizer;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace EntityCreator.Generators;

public class ModelCreator(EntityModel entity)
{
    private readonly string[] reservedNames = ["event", "class", "bool", "string", "public", "private", "protected"];
    
    private string? folder;

    public bool Create()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Domain\\{entity.Pluralized}";

        if (!Directory.Exists(folder))
            Directory.CreateDirectory(folder);

        CreateEntity(entity.Name!, entity.Properties!, "FullAuditedAggregateRoot<Guid>");

        foreach (var property in entity.Properties!)
        {
            if (property.Type == "Entity") 
                CreateEntity(property.Name!, property.Properties!, "Entity<Guid>");

            if (property.Type == "ValueObject")
                CreateEntity(property.Name!, property.Properties!, "ValueObject");
        }

        return true;
    }

    private bool CreateEntity(string entityName, List<PropertyModel> properties, string type)
    {
        string filename = $"{folder}\\{entityName}.cs";

        if (File.Exists(filename))
            return false;

        StringBuilder stringBuilder = new();

        // namespaces
        stringBuilder.AppendLine("using System;");
        stringBuilder.AppendLine("using System.Collections.Generic;");
        stringBuilder.AppendLine("using System.Collections.ObjectModel;");
        stringBuilder.AppendLine("using Volo.Abp.Domain.Entities;");
        stringBuilder.AppendLine("using Volo.Abp.Domain.Entities.Auditing;");
        stringBuilder.AppendLine("using Volo.Abp.Domain.Values;");
        
        stringBuilder.AppendLine();

        stringBuilder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        // class
        stringBuilder
            .Append("public class ")
            .Append(entityName)
            .Append(" : ")
            .AppendLine(type);

        stringBuilder.AppendLine("{");

        // constructor
        stringBuilder
            .Append("\tprotected ")
            .Append(entityName)
            .AppendLine("()")
            .AppendLine("\t{")
            .AppendLine("\t}")
            .AppendLine();

        stringBuilder
            .Append("\tpublic ")
            .Append(entityName);

        bool firstProperty = true;

        if (type != BaseTypes.ValueObject)
        {
            stringBuilder.Append("(Guid id");
            firstProperty = false;
        }
        else
        {
            stringBuilder.Append('(');
        }
        
        foreach (var property in properties)
        {
            if (property.IsCollection)
                continue;

            var propertyName = property.Name.Camelize();
            var propertyType = property.Type;

            if (reservedNames.Any(x => x == propertyName))
                propertyName = "@" + propertyName;

            if (property.Type == BaseTypes.Entity || 
                property.Type == BaseTypes.ValueObject || 
                property.Type == BaseTypes.AggregatedRoot)
                propertyType = property.Name;

            if (!firstProperty)
                stringBuilder.Append(", ");

            stringBuilder
                .Append(propertyType)
                .Append(' ')
                .Append(propertyName);

            firstProperty = false;
        }

        if (type != BaseTypes.ValueObject)
            stringBuilder.AppendLine(") : base(id)");
        else
            stringBuilder.AppendLine(")");

        stringBuilder.AppendLine("\t{");

        foreach (var property in properties)
        {
            if (property.IsCollection)
                continue;

            var propertyName = property.Name;
            var propertyInput = propertyName.Camelize();

            if (reservedNames.Any(x => x == propertyInput))
                propertyInput = "@" + propertyInput;

            stringBuilder
                .Append("\t\t")
                .Append(propertyName)
                .Append(" = ")
                .Append(propertyInput)
                .AppendLine(";");
        }

        stringBuilder
            .AppendLine("\t}")
            .AppendLine();

        // Properties
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyType = property.Type;

            if (property.Type == BaseTypes.Entity || 
                property.Type == BaseTypes.ValueObject || 
                property.Type == BaseTypes.AggregatedRoot)
                propertyType = property.Name;

            if (property.IsCollection)
            {
                propertyType = $"Collection<{propertyType}>";
                propertyName = propertyName.Pluralize();
            }

            if (!property.IsRequired && (property.Type == BaseTypes.String || property.IsCollection))
                propertyType += "?";

            stringBuilder
                .Append("\tpublic virtual ")
                .Append(propertyType)
                .Append(' ')
                .Append(propertyName);
            
            stringBuilder
                .Append(" { get; set; }")
                .AppendLine();
        }

        // methods
        if (type == BaseTypes.ValueObject)
        {
            stringBuilder
                .AppendLine()
                .AppendLine("\tprotected override IEnumerable<object> GetAtomicValues()")
                .AppendLine("\t{");

            foreach (var property in properties)
            {
                stringBuilder
                    .Append("\t\tyield return ")
                    .Append(property.Name);

                if (!property.IsRequired && property.Type == "string")
                    stringBuilder.Append('!');
                
                stringBuilder
                    .AppendLine(";");
            }

            stringBuilder
                .AppendLine("\t}");
        }

        stringBuilder.AppendLine("}");

        File.WriteAllText(filename, stringBuilder.ToString());

        return true;
    }
}
