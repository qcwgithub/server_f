public static partial class FieldTypeExt
{
    public static string CalcFieldTypeInfoNameDart(this FieldTypeInfo info)
    {
        switch (info.type)
        {
            case FieldType.class_:
            case FieldType.enum_:
                info.nameDart = info.concreteString;
                break;

            case FieldType.int_:
                info.nameDart = "int";
                break;

            case FieldType.string_:
                info.nameDart = "String";
                break;

            case FieldType.bool_:
                info.nameDart = "bool";
                break;

            case FieldType.long_:
                info.nameDart = "int";
                break;

            case FieldType.float_:
                info.nameDart = "float";
                break;

            case FieldType.list_:
                info.nameDart = "List<" + info.subInfos[0].CalcFieldTypeInfoNameDart() + ">";
                break;

            case FieldType.dictionary_:
                info.nameDart = "Map<" +
                    info.subInfos[0].CalcFieldTypeInfoNameDart() + ", " +
                    info.subInfos[1].CalcFieldTypeInfoNameDart() + ">";
                break;

            case FieldType.hashset_:
            case FieldType.bigint_:
            default:
                throw new Exception("unknown field type");
        }

        return info.nameDart;
    }

    public static FileFormatter PushDartToMsgPack(this FileFormatter f, FieldTypeInfo typeInfo,
        string accessGet)
    {
        switch (typeInfo.type)
        {
            case FieldType.class_:
                {
                    f.Push(string.Format("{0}.toMsgPack()", typeInfo.nameDart, accessGet));
                }
                break;

            case FieldType.list_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            f.Push(string.Format("{0}", accessGet));
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("{0}.map((e) => e.toMsgPack()).toList(growable: false)", accessGet));
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.dictionary_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                    switch (typeInfo.subInfos[1].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            f.Push(string.Format("{0}", accessGet));
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("{0}.map((k, v) => MapEntry(k, v.toMsgPack()))", accessGet));
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.float_:
            case FieldType.string_:
                f.Push(string.Format("{0}", accessGet));
                break;
            case FieldType.bigint_:
            case FieldType.hashset_:
            default:
                throw new Exception("unknown field type");
        }
        return f;
    }

    public static FileFormatter PushDartFromMsgPack(this FileFormatter f, FieldTypeInfo typeInfo,
        string accessGet)
    {
        switch (typeInfo.type)
        {
            case FieldType.class_:
                {
                    f.Push(string.Format("{0}.fromMsgPack({1} as List)", typeInfo.nameDart, accessGet));
                }
                break;

            case FieldType.list_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            f.Push(string.Format("{0}.from({1}, growable: true)", typeInfo.nameDart, accessGet));
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("({0} as List)\n", accessGet));
                            f.AddTab(1);
                            f.TabPushF(".map((e) => {0}.fromMsgPack(e as List))\n", typeInfo.subInfos[0].nameDart);
                            f.TabPushF(".toList(growable: true)");
                            f.AddTab(-1);

                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.dictionary_:
                {
                    switch (typeInfo.subInfos[0].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                    switch (typeInfo.subInfos[1].type)
                    {
                        case FieldType.int_:
                        case FieldType.bool_:
                        case FieldType.long_:
                        case FieldType.enum_:
                        case FieldType.float_:
                        case FieldType.string_:
                            f.Push(string.Format("({0} as Map)\n", accessGet));
                            f.AddTab(1);
                            f.TabPushF(".map((k, v) => MapEntry(k as {0}, v as {1}))", typeInfo.subInfos[0].nameDart, typeInfo.subInfos[1].nameDart);
                            f.AddTab(-1);
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("({0} as Map)\n", accessGet));
                            f.AddTab(1);
                            f.TabPushF(".map((k, v) => MapEntry(k as {0}, {1}.fromMsgPack(v as List)))", typeInfo.subInfos[0].nameDart, typeInfo.subInfos[1].nameDart);
                            f.AddTab(-1);
                            break;

                        default:
                            throw new Exception("unknown field type");
                    }
                }
                break;

            case FieldType.int_:
            case FieldType.bool_:
            case FieldType.long_:
            case FieldType.enum_:
            case FieldType.float_:
            case FieldType.string_:
                f.Push(string.Format("{0} as {1}", accessGet, typeInfo.nameDart));
                break;
            case FieldType.bigint_:
            case FieldType.hashset_:
            default:
                throw new Exception("unknown field type");
        }
        return f;
    }
}