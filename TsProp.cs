namespace InterfacesToTS;

public class TsProp
{
    public string Name;

    public bool IsExternal;

    public string ExternalPath;

    public bool IsEnum;

    public TsProp(string name, bool isExternal, string fullPath, bool isEnum)
    {
        Name = name;
        ExternalPath = fullPath;
        IsEnum = isEnum;
        IsExternal = isExternal;
    }
}