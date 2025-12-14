using System.Collections.Generic;

public class FieldTypeInfo
{
    public FieldType type;
    public string concreteString;
    public FieldTypeInfo[] subInfos;

    ////
    public string name;
    public string nameDb;
}

public class XInfoFieldConfig
{
    public FieldTypeInfo typeInfo;

    public string name;
    public string comment;

    // 以下只对 Profile.cs 有意义
    // public DataManagement dataManagement;
    public string defaultValueExp;
}

public class XInfoConfig
{
    public string name;
    public bool addLastDiffField;
    public List<XInfoFieldConfig> fields;
    public bool ensureEx;
    public bool math;
    public bool createFromHelper;
    public string cache; // memory or redis
}