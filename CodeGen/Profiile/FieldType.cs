using System;
using System.Collections.Generic;
public enum FieldType
{
    class_,
    int_,
    string_,
    bool_,
    long_,
    enum_,
    list_,
    dictionary_,
    float_,
    bigint_,
    hashset_,
}

public static class FieldTypeExt
{
    public static bool SupportsMath(this FieldType e)
    {
        switch (e)
        {
            case FieldType.class_:
                return false;
            case FieldType.enum_:
                return false;

            case FieldType.int_:
                return true;
            case FieldType.string_:
                return false;
            case FieldType.bool_:
                return false;
            case FieldType.long_:
                return true;
            case FieldType.list_:
                return false;
            case FieldType.dictionary_:
            case FieldType.float_:
                return true;
            case FieldType.bigint_:
                return true;
            case FieldType.hashset_:
                return false;
            default:
                throw new Exception("unknown field type");
        }
    }
    public static bool NeedConcreteString(this FieldType e)
    {
        switch (e)
        {
            case FieldType.class_:
            case FieldType.enum_:
                return true;

            case FieldType.int_:
            case FieldType.string_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.list_:
            case FieldType.dictionary_:
            case FieldType.float_:
            case FieldType.bigint_:
            case FieldType.hashset_:
                return false;
            default:
                throw new Exception("unknown field type");
        }
    }
    public static int SubtypeCount(this FieldType e)
    {
        switch (e)
        {
            case FieldType.class_:
            case FieldType.enum_:
            case FieldType.int_:
            case FieldType.string_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.float_:
            case FieldType.bigint_:
                return 0;

            case FieldType.list_:
            case FieldType.hashset_:
                return 1;
            case FieldType.dictionary_:
                return 2;
            default:
                throw new Exception("unknown field type");
        }
    }
    public static string CalcFieldTypeInfoName(this FieldTypeInfo info)
    {
        switch (info.type)
        {
            case FieldType.class_:
            case FieldType.enum_:
                info.name = info.concreteString;
                break;

            case FieldType.int_:
                info.name = "int";
                break;

            case FieldType.string_:
                info.name = "string";
                break;

            case FieldType.bool_:
                info.name = "bool";
                break;

            case FieldType.long_:
                info.name = "long";
                break;

            case FieldType.float_:
                info.name = "float";
                break;

            case FieldType.list_:
                info.name = "List<" + info.subInfos[0].CalcFieldTypeInfoName() + ">";
                break;

            case FieldType.hashset_:
                info.name = "HashSet<" + info.subInfos[0].CalcFieldTypeInfoName() + ">";
                break;
            case FieldType.dictionary_:
                info.name = "Dictionary<" +
                    info.subInfos[0].CalcFieldTypeInfoName() + ", " +
                    info.subInfos[1].CalcFieldTypeInfoName() + ">";
                break;

            case FieldType.bigint_:
                info.name = "BigInteger";
                break;

            default:
                throw new Exception("unknown field type");
        }

        return info.name;
    }
    public static string CalcFieldTypeInfoName_Db(this FieldTypeInfo info)
    {
        switch (info.type)
        {
            case FieldType.class_:
                info.nameDb = info.concreteString + "_Db";
                break;
            case FieldType.enum_:
                info.nameDb = info.concreteString;
                break;

            case FieldType.int_:
                info.nameDb = "int";
                break;

            case FieldType.string_:
                info.nameDb = "string";
                break;

            case FieldType.bool_:
                info.nameDb = "bool";
                break;

            case FieldType.long_:
                info.nameDb = "long";
                break;

            case FieldType.float_:
                info.nameDb = "float";
                break;

            case FieldType.list_:
                info.nameDb = "List<" + info.subInfos[0].CalcFieldTypeInfoName_Db() + ">";
                break;

            case FieldType.hashset_:
                info.nameDb = "HashSet<" + info.subInfos[0].CalcFieldTypeInfoName_Db() + ">";
                break;
            case FieldType.dictionary_:
                info.nameDb = "Dictionary<" +
                    info.subInfos[0].CalcFieldTypeInfoName_Db() + ", " +
                    info.subInfos[1].CalcFieldTypeInfoName_Db() + ">";
                break;

            case FieldType.bigint_:
                info.nameDb = "BigInteger";
                break;

            default:
                throw new Exception("unknown field type");
        }

        return info.nameDb;
    }

    public static bool NeedEnsure(this FieldType e,
        FieldType? parentType)
    {
        switch (e)
        {
            case FieldType.class_:
            case FieldType.list_:
            case FieldType.dictionary_:
            case FieldType.hashset_:
                return true;

            case FieldType.string_:
                if (parentType != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.float_:
            case FieldType.bigint_:
                return false;
            default:
                throw new Exception("unknown field type");
        }
    }
    // public static string DefaultExpForCopy(this FieldTypeInfo typeInfo)
    // {
    //     switch (typeInfo.type)
    //     {
    //         case FieldType.class_:
    //         case FieldType.list_:
    //         case FieldType.dictionary_:
    //             return "new " + typeInfo.name + "()";
    //         case FieldType.string_:
    //             return "string.Empty";

    //         case FieldType.int_:
    //         case FieldType.long_:
    //             return "0";
    //         case FieldType.bool_:
    //             return "false";
    //         case FieldType.enum_:
    //             return "default(" + typeInfo.name + ")";
    //         default:
    //             throw new Exception("unknown field type");
    //     }
    // }

    public static FileFormatter PushEnsure(this FileFormatter f, FieldTypeInfo typeInfo,
        string accessGet, string accessSet)
    {
        switch (typeInfo.type)
        {
            case FieldType.class_:
                {
                    if (accessSet == null)
                    {
                        f.TabPush("{0}.Ensure({1});\n".Format(typeInfo.name, accessGet));
                    }
                    else
                    {
                        f.TabPush("{0} = {1}.Ensure({2});\n".Format(accessSet, typeInfo.name, accessGet));
                    }
                }
                break;

            case FieldType.list_:
                f.TabPush("if ({0} == null)\n".Format(accessGet));
                f.BlockStart();
                f.TabPush("{0} = new {1}();\n".Format(accessSet, typeInfo.name));
                f.BlockEnd();

                if (typeInfo.subInfos[0].type.NeedEnsure(typeInfo.type))
                {
                    f.TabPush("for (int i = 0; i < {0}.Count; i++)\n".Format(accessGet));
                    f.BlockStart();
                    {
                        f.PushEnsure(typeInfo.subInfos[0], accessGet + "[i]", accessSet + "[i]");
                    }
                    f.BlockEnd();
                }
                break;

            case FieldType.hashset_:
                f.TabPush("if ({0} == null)\n".Format(accessGet));
                f.BlockStart();
                f.TabPush("{0} = new {1}();\n".Format(accessSet, typeInfo.name));
                f.BlockEnd();

                if (typeInfo.subInfos[0].type.NeedEnsure(typeInfo.type))
                {
                    throw new Exception("set sub type NeedEnsure() could not be true");
                }
                break;

            case FieldType.dictionary_:
                f.TabPush("if ({0} == null)\n".Format(accessGet));
                f.BlockStart();
                {
                    f.TabPush("{0} = new {1}();\n".Format(accessSet, typeInfo.name));
                }
                f.BlockEnd();

                if (typeInfo.subInfos[1].type.NeedEnsure(typeInfo.type))
                {
                    f.TabPush("foreach (var kv in {0})\n".Format(accessGet));
                    f.BlockStart();
                    {
                        f.PushEnsure(typeInfo.subInfos[1], "kv.Value",
                        // accessGet + "[kv.Key]"
                        null // 不要赋值，会报错
                        );
                    }
                    f.BlockEnd();
                }
                break;

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.float_:
            case FieldType.bigint_:
                break;

            case FieldType.string_:
                f.TabPush("if ({0} == null)\n".Format(accessGet));
                f.BlockStart();
                {
                    f.TabPush("{0} = string.Empty;\n".Format(accessGet));
                }
                f.BlockEnd();
                break;
            default:
                throw new Exception("unknown field type");
        }
        return f;
    }

    public static string ToIsDifferent(this FieldTypeInfo typeInfo, string accessThis, string accessOther)
    {
        switch (typeInfo.type)
        {
            case FieldType.list_:
                switch (typeInfo.subInfos[0].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.enum_:
                    case FieldType.float_:
                    case FieldType.string_:
                        return "{0}.IsDifferent_ListValue({1})".Format(accessThis, accessOther);

                    case FieldType.class_:
                        {
                            return "{0}.IsDifferent_ListClass({1})".Format(accessThis, accessOther);
                        }

                    case FieldType.list_:
                    case FieldType.dictionary_:
                        return "{0}.IsDifferent({1})".Format(accessThis, accessOther);

                    default:
                        throw new Exception("unknown field type");
                }
            // break;

            case FieldType.hashset_:
                switch (typeInfo.subInfos[0].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.enum_:
                    case FieldType.float_:
                    case FieldType.string_:
                        return "{0}.IsDifferent_HashSetValue({1})".Format(accessThis, accessOther);

                    case FieldType.class_:
                    case FieldType.list_:
                    case FieldType.dictionary_:
                    default:
                        throw new Exception("unknown field type");
                }
            // break;

            case FieldType.class_:
                {
                    return "{0}.IsDifferent({1})".Format(accessThis, accessOther);
                }

            case FieldType.dictionary_:
                switch (typeInfo.subInfos[1].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.float_:
                    case FieldType.string_:
                    case FieldType.bigint_:
                        return "{0}.IsDifferent_DictValue({1})".Format(accessThis, accessOther);

                    case FieldType.enum_:
                        return "{0}.IsDifferent_DictEnum({1})".Format(accessThis, accessOther);

                    case FieldType.class_:
                        {
                            return "{0}.IsDifferent_DictClass({1})".Format(accessThis, accessOther);
                        }

                    case FieldType.list_:
                    case FieldType.dictionary_:
                        return "{0}.IsDifferent({1})".Format(accessThis, accessOther);

                    default:
                        throw new Exception("unknown field type");
                }

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.string_:
            case FieldType.float_:
            case FieldType.bigint_:
                return "{0} != {1}".Format(accessThis, accessOther);

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string ToDeepCopyFrom(this FieldTypeInfo typeInfo, string accessThis, string accessOther)
    {
        switch (typeInfo.type)
        {
            case FieldType.list_:
                switch (typeInfo.subInfos[0].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.enum_:
                    case FieldType.string_:
                    case FieldType.float_:
                    case FieldType.bigint_:
                        return "{0}.DeepCopyFrom_ListValue({1})".Format(accessThis, accessOther);

                    case FieldType.class_:
                        return "{0}.DeepCopyFrom_ListClass({1})".Format(accessThis, accessOther);

                    case FieldType.list_:
                    case FieldType.dictionary_:
                    default:
                        throw new Exception("unknown field type");
                }
            // break;

            case FieldType.hashset_:
                switch (typeInfo.subInfos[0].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.enum_:
                    case FieldType.string_:
                    case FieldType.float_:
                    case FieldType.bigint_:
                        return "{0}.DeepCopyFrom_HashSetValue({1})".Format(accessThis, accessOther);

                    case FieldType.class_: // 不允许 HashSet<Class>
                    case FieldType.list_:
                    case FieldType.dictionary_:
                    default:
                        throw new Exception("unknown field type");
                }
            // break;

            case FieldType.dictionary_:
                switch (typeInfo.subInfos[1].type)
                {
                    case FieldType.int_:
                    case FieldType.bool_:
                    case FieldType.long_:
                    case FieldType.enum_:
                    case FieldType.float_:
                    case FieldType.string_:
                    case FieldType.bigint_:
                        return "{0}.DeepCopyFrom_DictValue({1})".Format(accessThis, accessOther);

                    case FieldType.class_:
                        return "{0}.DeepCopyFrom_DictClass({1})".Format(accessThis, accessOther);

                    case FieldType.list_:
                    case FieldType.dictionary_:
                        return "{0}.DeepCopyFrom({1})".Format(accessThis, accessOther);

                    default:
                        throw new Exception("unknown field type");
                }

            case FieldType.class_:
                return "{0}.DeepCopyFrom({1})".Format(accessThis, accessOther);

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.string_:
            case FieldType.float_:
            case FieldType.bigint_:
                return "{0} = {1}".Format(accessThis, accessOther);
            default:
                throw new Exception("unknown field type");
        }
    }

    public static void PushCopy_Db(this FileFormatter f, FieldTypeInfo typeInfo, string accessThis, string accessOther, bool accessOther_appendValueIfPrimitive, out bool canCompareNull)
    {
        canCompareNull = true;
        switch (typeInfo.type)
        {
            case FieldType.list_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.string_:
                        case FieldType.float_:
                        case FieldType.bigint_:
                            f.Push("{0} = XInfoHelper_Db.Copy_ListValue({1});\n".Format(accessThis, accessOther));
                            break;

                        case FieldType.class_:
                            f.Push("{0} = XInfoHelper_Db.Copy_ListClass<{2}, {3}>({1});\n".Format(accessThis, accessOther, typeInfo.subInfos[0].nameDb, typeInfo.subInfos[0].name));
                            break;

                        case FieldType.list_:
                        case FieldType.dictionary_:
                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.hashset_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.string_:
                        case FieldType.float_:
                        case FieldType.bigint_:
                            f.Push("{0} = XInfoHelper_Db.Copy_HashSetValue({1});\n".Format(accessThis, accessOther));
                            break;

                        case FieldType.class_: // 不允许 HashSet<Class>
                        case FieldType.list_:
                        case FieldType.dictionary_:
                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.dictionary_:
                {
                    switch (typeInfo.subInfos[1].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                        case FieldType.bigint_:
                            f.Push("{0} = XInfoHelper_Db.Copy_DictValue({1});\n".Format(accessThis, accessOther));
                            break;

                        case FieldType.class_:
                            f.Push("{0} = XInfoHelper_Db.Copy_DictClass<{2}, {3}, {4}>({1});\n".Format(accessThis, accessOther, typeInfo.subInfos[0].name, typeInfo.subInfos[1].nameDb, typeInfo.subInfos[1].name));
                            break;

                        case FieldType.list_:
                        case FieldType.dictionary_:
                            f.Push("{0} = XInfoHelper_Db.????({1});\n".Format(accessThis, accessOther));
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.class_:
                f.Push("{0} = XInfoHelper_Db.Copy_Class<{2}, {3}>({1});\n".Format(accessThis, accessOther, typeInfo.nameDb, typeInfo.name));
                break;

            case FieldType.enum_:
                {
                    canCompareNull = false;
                    f.Push("{0} = XInfoHelper_Db.Copy_Enum({1});\n".Format(accessThis, accessOther));
                }
                break;

            case FieldType.int_:
                f.Push("{0} = XInfoHelper_Db.Copy_int({1}{2});\n".Format(accessThis, accessOther, accessOther_appendValueIfPrimitive ? ".Value" : string.Empty));
                break;

            case FieldType.long_:
                f.Push("{0} = XInfoHelper_Db.Copy_long({1}{2});\n".Format(accessThis, accessOther, accessOther_appendValueIfPrimitive ? ".Value" : string.Empty));
                break;

            case FieldType.bigint_:
                f.Push("{0} = XInfoHelper_Db.Copy_BigInteger({1}{2});\n".Format(accessThis, accessOther, accessOther_appendValueIfPrimitive ? ".Value" : string.Empty));
                break;

            case FieldType.float_:
                f.Push("{0} = XInfoHelper_Db.Copy_float({1}{2});\n".Format(accessThis, accessOther, accessOther_appendValueIfPrimitive ? ".Value" : string.Empty));
                break;

            case FieldType.bool_:
                f.Push("{0} = XInfoHelper_Db.Copy_bool({1}{2});\n".Format(accessThis, accessOther, accessOther_appendValueIfPrimitive ? ".Value" : string.Empty));
                break;

            case FieldType.string_:
                f.Push("{0} = XInfoHelper_Db.Copy_string({1});\n".Format(accessThis, accessOther));
                break;

            default:
                throw new Exception("unknown field type");
        }
    }

    public static string ToNullable(this FieldTypeInfo info)
    {
        // 把不可空类型，变成可空类型
        switch (info.type)
        {
            case FieldType.class_:
            case FieldType.list_:
            case FieldType.dictionary_:
            case FieldType.string_:
            case FieldType.hashset_:
                return info.name;

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.float_:
            case FieldType.bigint_:
                return info.name + "?";
            default:
                throw new Exception("unknown field type");
        }
    }

    public static string ToDb(this FieldTypeInfo info)
    {
        // 把不可空类型，变成可空类型
        switch (info.type)
        {
            case FieldType.class_:
            case FieldType.list_:
            case FieldType.dictionary_:
            case FieldType.string_:
            case FieldType.hashset_:
                return info.nameDb;

            case FieldType.enum_:
                return info.nameDb;

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.float_:
            case FieldType.bigint_:
                return info.nameDb + "?";
            default:
                throw new Exception("unknown field type");
        }
    }

    public static string HelperRead(this FieldTypeInfo info)
    {
        // 把不可空类型，变成可空类型
        switch (info.type)
        {
            case FieldType.class_:
            case FieldType.list_:
            case FieldType.dictionary_:
                return "NotSupported";
            case FieldType.string_:
                return "ReadString";
            case FieldType.hashset_:
                return "NotSupported";

            case FieldType.int_:
                return "ReadInt";
            case FieldType.bool_:
                return "ReadBool";
            case FieldType.long_:
                return "ReadLong";
            case FieldType.enum_:
                return "NotSupported";
            case FieldType.float_:
                return "ReadFloat";
            case FieldType.bigint_:
                return "ReadBigInteger";
            default:
                throw new Exception("unknown field type");
        }
    }
}