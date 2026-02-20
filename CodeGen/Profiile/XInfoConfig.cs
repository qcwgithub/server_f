using System.Collections.Generic;
using System.Text;

public class FieldTypeInfo
{
    public FieldType type;
    public string concreteString;
    public FieldTypeInfo[] subInfos;


    ////
    public string name;
    public string nameDb;

    public string nameDart;
}

public class XInfoFieldConfig
{
    public FieldTypeInfo typeInfo;
    public bool nullable;

    public string name;
    public string comment;
}

public class XInfoConfig
{
    public string name;

    public static string FirstCharacterLower(string name)
    {
        sb.Clear();
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (i == 0 && c >= 'A' && c <= 'Z')
            {
                if (i > 0)
                {
                    sb.Append('_');
                }
                sb.Append((char)(c - 'A' + 'a'));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    public static string NameToLowerName(string name)
    {
        sb.Clear();
        for (int i = 0; i < name.Length; i++)
        {
            char c = name[i];
            if (c >= 'A' && c <= 'Z')
            {
                if (i > 0)
                {
                    sb.Append('_');
                }
                sb.Append((char)(c - 'A' + 'a'));
            }
            else
            {
                sb.Append(c);
            }
        }
        return sb.ToString();
    }

    static StringBuilder sb = new StringBuilder();
    string _fileNameDart;
    public string fileNameDart
    {
        get
        {
            if (_fileNameDart == null)
            {
                _fileNameDart = NameToLowerName(this.name);
            }
            return _fileNameDart;
        }
    }
    public bool addLastDiffField;
    public List<XInfoFieldConfig> fields;
    public bool ensure;
    public bool ensureEx;
    public bool deepCopy;
    public bool math;
    public bool createFromHelper;
    public CacheType cacheType;
    public string cacheService;
    public bool dart;
}