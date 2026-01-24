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
                info.name = "Map<" +
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

    public static FileFormatter PushPrepareDartFromMsgPack(this FileFormatter f, FieldTypeInfo typeInfo,
        string accessGet, string accessSet)
    {
        switch (typeInfo.type)
        {
            case FieldType.class_:
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
            case FieldType.string_:
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
                    f.Push(string.Format("{0}.fromMsgPack({1} as List),\n", typeInfo.nameDart, accessGet));
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
                            f.Push(string.Format("{0}.from({1}, growable: true),\n", typeInfo.nameDart, accessGet));
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("({0} as List)\n", accessGet));
                            f.AddTab(1);
                            f.TabPushF(".map((e) => {0}.fromMsgPack(e as List))\n", typeInfo.subInfos[0].nameDart);
                            f.TabPushF(".toList(growable: true),\n");
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
                            break;

                        case FieldType.class_:
                            f.Push(string.Format("({0} as Map).forEach(k, v) {{\n", accessGet));
                            f.AddTab(1);
                            f.TabPushF(".map((k, v) => {0}.fromMsgPack(v as List))\n", typeInfo.subInfos[0].nameDart);
                            f.TabPushF(".toList(growable: true),\n");
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
                f.Push(string.Format("{0} as {1},\n", accessGet, typeInfo.nameDart));
                break;
            case FieldType.bigint_:
            case FieldType.hashset_:
            default:
                throw new Exception("unknown field type");
        }
        return f;
    }
}