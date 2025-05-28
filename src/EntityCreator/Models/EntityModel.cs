using Humanizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator.Models;

public class EntityModel
{
    private string? _plularized;

    private string? _name;

    private string? _projectName;

    public string? Name
    {
        get { return _name; }
        set 
        {
            if (!string.IsNullOrEmpty(value))
                value = value.Dehumanize();

            _name = value; 
        }
    }

    public string? Location { get; set; }

    public string? Namespace { get; set; }

    public string? ProjectName 
    {
        get
        {
            if (string.IsNullOrEmpty(_projectName) && !string.IsNullOrEmpty(Namespace))
                _projectName = Namespace[(Namespace.IndexOf('.') + 1)..];

            return _projectName;
        }
    }

    public string? Pluralized
    {
        get 
        {
            if (string.IsNullOrEmpty(_plularized) && !string.IsNullOrEmpty(Name))
                _plularized = Name.Pluralize();

            return _plularized; 
        }
        set
        {
            _plularized = value;
        }
    }

    public List<PropertyModel>? Properties { get; set; }
    public List<LocalizationModel>? Localizations { get; set; }
}
