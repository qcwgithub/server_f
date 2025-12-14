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
}

public class XInfoConfig
{
    public string name;
    public bool addLastDiffField;
    public List<XInfoFieldConfig> fields;
    public bool ensureEx;
    public bool math;
    public bool createFromHelper;
    public CacheType cacheType;
}