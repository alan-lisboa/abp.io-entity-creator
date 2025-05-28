using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator.Models;

public class LocalizationModel
{
    public string? Language { get; set; }
    public string? EntityName { get; set; }
    public string? PluralizedName { get; set; }
    public string? Create { get; set; }
    public string? Edit { get; set; }
    public string? Delete { get; set; }
    public string? DeletionMessage { get; set; }
    public List<LocalizationPropertyModel>? Properties { get; set; }
}


