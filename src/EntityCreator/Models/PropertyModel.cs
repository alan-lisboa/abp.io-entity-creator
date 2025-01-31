namespace EntityCreator.Models;

public class PropertyModel
{
    public int Index { get; set; }
    public string? Name { get; set; }
    public string? Type { get; set; }
    public int Size { get; set; }
    public bool IsRequired { get; set; }
    public bool IsCollection { get; set; }
    public List<PropertyModel>? Properties { get; set; }
}
