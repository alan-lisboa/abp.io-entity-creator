using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityCreator;

public class PropertyModel
{
    public int Index { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public int Size { get; set; }
    public bool IsRequired { get; set; }
}
