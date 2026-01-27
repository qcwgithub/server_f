using System.Collections.Generic;
using System.Text;

public class EnumFieldConfig
{
    public string name;
    public int value;
    public bool dartDefault;
}

public class EnumConfig
{
    public string name;
    public List<EnumFieldConfig> fields;
}