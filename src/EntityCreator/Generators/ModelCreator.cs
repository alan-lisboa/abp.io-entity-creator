using EntityCreator.Helpers;
using EntityCreator.Models;
using Humanizer;
using System.DirectoryServices.ActiveDirectory;
using System.Text;

namespace EntityCreator.Generators;

public class ModelCreator(EntityModel entity) : BaseGenerator
{
    private readonly string[] reservedNames = ["event", "class", "bool", "string", "public", "private", "protected"];

    public override bool Handle()
    {
        folder = $"{entity.Location}\\src\\{entity.Namespace}.Domain\\Entities\\{entity.Pluralized}";

        CreateDirectory(folder);

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
        filename = $"{folder}\\{entityName}.cs";

        if (File.Exists(filename))
            return false;

        // namespaces
        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Collections.ObjectModel;");
        builder.AppendLine("using Volo.Abp.Domain.Entities;");
        builder.AppendLine("using Volo.Abp.Domain.Entities.Auditing;");
        builder.AppendLine("using Volo.Abp.Domain.Values;");
        
        builder.AppendLine();

        builder
            .Append("namespace ")
            .Append(entity.Namespace)
            .Append('.')
            .Append(entity.Pluralized)
            .AppendLine(";")
            .AppendLine();

        // class
        builder
            .Append("public class ")
            .Append(entityName)
            .Append(" : ")
            .AppendLine(type);

        builder.AppendLine("{");

        indentationLevel++;

        // constructor
        builder
            .Append(Indentation)
            .Append("protected ")
            .Append(entityName)
            .AppendLine("()")
            .Append(Indentation)
            .AppendLine("{")
            .Append(Indentation)
            .AppendLine("}")
            .AppendLine();

        builder
            .Append(Indentation)
            .Append("public ")
            .Append(entityName);

        bool firstProperty = true;

        if (type != BaseTypeHelper.ValueObject)
        {
            builder.Append("(Guid id");
            firstProperty = false;
        }
        else
        {
            builder.Append('(');
        }
        
        foreach (var property in properties)
        {
            if (property.IsCollection)
                continue;

            var propertyName = property.Name.Camelize();
            var propertyType = property.Type;

            if (reservedNames.Any(x => x == propertyName))
                propertyName = "@" + propertyName;

            if (property.Type == BaseTypeHelper.Entity || 
                property.Type == BaseTypeHelper.ValueObject || 
                property.Type == BaseTypeHelper.AggregateRoot)
                propertyType = property.Name;

            if (!firstProperty)
                builder.Append(", ");

            builder
                .Append(propertyType)
                .Append(' ')
                .Append(propertyName);

            firstProperty = false;
        }

        if (type != BaseTypeHelper.ValueObject)
            builder.AppendLine(") : base(id)");
        else
            builder.AppendLine(")");

        builder
            .Append(Indentation)
            .AppendLine("{");

        indentationLevel++;

        foreach (var property in properties)
        {
            if (property.IsCollection)
                continue;

            var propertyName = property.Name;
            var propertyInput = propertyName.Camelize();

            if (reservedNames.Any(x => x == propertyInput))
                propertyInput = "@" + propertyInput;

            builder
                .Append(Indentation)
                .Append(propertyName)
                .Append(" = ")
                .Append(propertyInput)
                .AppendLine(";");
        }

        indentationLevel--;

        builder
            .Append(Indentation)
            .AppendLine("}")
            .AppendLine();

        // Properties
        foreach (var property in properties)
        {
            var propertyName = property.Name;
            var propertyType = property.Type;

            if (property.Type == BaseTypeHelper.Entity || 
                property.Type == BaseTypeHelper.ValueObject || 
                property.Type == BaseTypeHelper.AggregateRoot)
                propertyType = property.Name;

            if (property.IsCollection)
            {
                propertyType = $"Collection<{propertyType}>";
                propertyName = propertyName.Pluralize();
            }

            if (!property.IsRequired && (property.Type == BaseTypeHelper.String || property.IsCollection))
                propertyType += "?";

            builder
                .Append(Indentation)
                .Append("public virtual ")
                .Append(propertyType)
                .Append(' ')
                .Append(propertyName)
                .Append(" { get; set; }")
                .AppendLine();
        }

        // methods
        if (type == BaseTypeHelper.ValueObject)
        {
            builder
                .AppendLine()
                .Append(Indentation)
                .AppendLine("protected override IEnumerable<object> GetAtomicValues()")
                .Append(Indentation)
                .AppendLine("{");

            indentationLevel++;

            foreach (var property in properties)
            {
                builder
                    .Append(Indentation)
                    .Append("yield return ")
                    .Append(property.Name);

                if (!property.IsRequired && property.Type == "string")
                    builder.Append('!');
                
                builder
                    .AppendLine(";");
            }

            indentationLevel--;

            builder
                .Append(Indentation)
                .AppendLine("}");
        }

        builder.AppendLine("}");

        return WriteToFile();
    }
}
