using System.Text;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
public class ServerDataProgram
{
    static string Read(string content, ref int index, int endIndex)
    {
        int j = content.IndexOf(' ', index);
        if (j >= 0 && j <= endIndex)
        {
            string str = content.Substring(index, j - index);
            index = j + 1;
            return str;
        }
        else
        {
            string str = content.Substring(index, endIndex - index + 1);
            index = endIndex;
            return str;
        }
    }
    static string ReadType(string content, ref int index, int endIndex, string profileType, out string modifier)
    {
        string s = Read(content, ref index, endIndex);
        s = (s == "*" ? profileType : s);
        int i = s.IndexOf('!');
        modifier = null;
        if (i >= 0)
        {
            modifier = s.Substring(i + 1);
            s = s.Substring(0, i);
        }
        return s;
    }

    static ServerDataConfig.Query SolveQuery(string content, int startIndex, int endIndex, string profileType)
    {
        var query = new ServerDataConfig.Query();

        int index = startIndex;

        // cond
        query.cond = Read(content, ref index, endIndex);
        switch (query.cond)
        {
            case "all":
                break;
            default:
                query.condField = new ServerDataConfig.Field();
                query.condField.type = ReadType(content, ref index, endIndex, profileType, out query.condField.typeModifier);
                query.condField.name = Read(content, ref index, endIndex);
                break;
        }
        switch (query.cond)
        {
            case "eq2":
                query.condField2 = new ServerDataConfig.Field();
                query.condField2.type = ReadType(content, ref index, endIndex, profileType, out query.condField2.typeModifier);
                query.condField2.name = Read(content, ref index, endIndex);
                break;
        }

        // output
        query.output = Read(content, ref index, endIndex);
        query.outputField1 = new ServerDataConfig.Field();
        query.outputField1.type = ReadType(content, ref index, endIndex, profileType, out query.outputField1.typeModifier);
        query.outputField1.name = Read(content, ref index, endIndex);

        switch (query.output)
        {
            case "dict":
            case "dict2":
                query.outputField2 = new ServerDataConfig.Field();
                query.outputField2.type = ReadType(content, ref index, endIndex, profileType, out query.outputField2.typeModifier);
                query.outputField2.name = Read(content, ref index, endIndex);
                break;
        }

        switch (query.output)
        {
            case "dict2":
                query.outputField3 = new ServerDataConfig.Field();
                query.outputField3.type = ReadType(content, ref index, endIndex, profileType, out query.outputField3.typeModifier);
                query.outputField3.name = Read(content, ref index, endIndex);
                break;
        }

        return query;
    }

    static ServerDataConfig.Field SolveField(string content, int startIndex, int endIndex, string profileType)
    {
        int index = startIndex;
        var f = new ServerDataConfig.Field();
        f.type = ReadType(content, ref index, endIndex, profileType, out f.typeModifier);
        f.name = Read(content, ref index, endIndex);
        return f;
    }

    static ServerDataConfig.Field SolveField(string content, ref int startIndex, int endIndex, string profileType)
    {
        ref int index = ref startIndex;
        var f = new ServerDataConfig.Field();
        f.type = ReadType(content, ref index, endIndex, profileType, out f.typeModifier);
        f.name = Read(content, ref index, endIndex);
        return f;
    }

    static ServerDataConfig.Save SolveSave(string content, int startIndex, int endIndex, string profileType)
    {
        int index = startIndex;

        var save = new ServerDataConfig.Save();
        save.cond = Read(content, ref index, endIndex);

        switch (save.cond)
        {
            case "singleton":
                break;
            case "eq2":
                save.field = SolveField(content, ref index, endIndex, profileType);
                save.field2 = SolveField(content, ref index, endIndex, profileType);
                break;
            default:
                save.field = SolveField(content, index, endIndex, profileType);
                break;
        }

        return save;
    }

    static ServerDataConfig.Index SolveIndex(string content, int startIndex, int endIndex)
    {
        int index = startIndex;

        // cond
        var obj = new ServerDataConfig.Index
        {
            isUnique = Read(content, ref index, endIndex) == "1",
            fieldName = Read(content, ref index, endIndex),
        };
        if (index < endIndex)
        {
            obj.fieldName2 = Read(content, ref index, endIndex);
        }
        return obj;
    }

    static List<T> IterateArrayInCell<T>(string cell, Func<string, int, int, T> action)
    {
        var list = new List<T>();

        if (!string.IsNullOrEmpty(cell))
        {
            int index = 0;
            if (cell[0] == '[')
            {
                while (true)
                {
                    int startIndex = cell.IndexOf('[', index);
                    if (startIndex < 0)
                    {
                        break;
                    }
                    int endIndex = cell.IndexOf(']', startIndex);
                    index = endIndex;

                    list.Add(action(cell, startIndex + 1, endIndex - 1));
                }
            }
            else
            {
                list.Add(action(cell, 0, cell.Length - 1));
            }
        }

        return list;
    }
    public static void Do()
    {
        var copyConfigDict = new Dictionary<string, ServerDataCopyConfig>();

        Script.CsvHelper helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/ServerDataCopyConfig.csv"));
        while (helper.ReadRow())
        {
            var c = new ServerDataCopyConfig();
            c.name = helper.ReadString(nameof(c.name));
            c.count = helper.ReadInt(nameof(c.count));
            copyConfigDict.Add(c.name, c);
        }

        var list = new List<ServerDataConfig>();

        helper = Script.CsvUtils.Parse(CodeGen.Program.ReadAllText("CodeGen/ServerDataConfig.csv"));
        while (helper.ReadRow())
        {
            string copyName = helper.ReadString("copy");
            ServerDataCopyConfig copyConfig = null;
            if (!string.IsNullOrEmpty(copyName))
            {
                copyConfig = copyConfigDict[copyName];
            }
            int copyCount = copyConfig == null ? 1 : copyConfig.count;
            for (int i = 0; i < copyCount; i++)
            {
                var c = new ServerDataConfig();

                string s = helper.ReadString(nameof(c.profileType));
                string[] ss = s.Split('/');
                if (ss.Length == 1)
                {
                    c.profileType = s;
                }
                else if (ss.Length == 2)
                {
                    c.originalProfileType = ss[0];
                    c.profileType = ss[1];
                }
                else
                {
                    throw new Exception();
                }

                c.createCollectionCs = helper.ReadString(nameof(c.createCollectionCs)) == "1";
                if (c.createCollectionCs)
                {
                    c.fileName = helper.ReadString(nameof(c.fileName));
                    c.className = c.fileName;
                    c.collectionName = helper.ReadString(nameof(c.collectionName));
                    c.unsetEmptyField = helper.ReadString(nameof(c.unsetEmptyField)) == "1";
                    c.partial = helper.ReadString(nameof(c.partial)) == "1";
                    c.dbName = helper.ReadString(nameof(c.dbName));

                    c.index = IterateArrayInCell<ServerDataConfig.Index>(helper.ReadString(nameof(c.index)), SolveIndex);
                    c.query = IterateArrayInCell<ServerDataConfig.Query>(helper.ReadString(nameof(c.query)), (cell, startIndex, endIndex) => SolveQuery(cell, startIndex, endIndex, c.profileType));
                    c.save = IterateArrayInCell<ServerDataConfig.Save>(helper.ReadString(nameof(c.save)), (cell, startIndex, endIndex) => SolveSave(cell, startIndex, endIndex, c.profileType));

                    c.redisDb = helper.ReadString(nameof(c.redisDb));
                    c.keyFunc = helper.ReadString(nameof(c.keyFunc));
                    c.keyParam = IterateArrayInCell<ServerDataConfig.Field>(helper.ReadString(nameof(c.keyParam)), (cell, startIndex, endIndex) => SolveField(cell, startIndex, endIndex, c.profileType));
                    if (c.keyParam.Count > 2)
                    {
                        throw new NotImplementedException();
                    }

                    c.createPersistence = helper.ReadString(nameof(c.createPersistence)) == "1";
                    c.createProxy = helper.ReadString(nameof(c.createProxy)) == "1";
                    c.loadUseQueryIndex = helper.ReadInt(nameof(c.loadUseQueryIndex));
                    c.proxyIsPartial = helper.ReadString(nameof(c.proxyIsPartial)) == "1";
                    c.createPlaceholderWhenNull = helper.ReadString(nameof(c.createPlaceholderWhenNull)) == "1";
                    c.canExpire = helper.ReadString(nameof(c.canExpire)) == "1";

                    c.taskQueueHash = Enum.Parse<ServerDataConfig.TaskQueueHash>(helper.ReadString(nameof(c.taskQueueHash)));

                    if (copyConfig != null)
                    {
                        c.copy = new ServerDataConfig.Copy { index = i, config = copyConfig };
                    }
                }
                list.Add(c);
            }
        }

        foreach (var c in list)
        {
            if (c.createCollectionCs)
            {
                Create_collection_xxx.Create(c);
                Create_MsgQuery_XXX.Create(c);
                Create_MsgSave_XXX.Create(c);
                Create_QueryXXX.Create(c);
                Create_SaveXXX.Create(c);
            }
        }
        Create_DBXXService_var.Create(list);
        foreach (var c in list)
        {
            // if (c.createRedis)
            // {
            //     Create_XXXRedis.Create(c);
            // }

            if (c.createProxy)
            {
                Create_XXXProxy.Create(c);
            }
        }

        Create_PersistenceTask_XXX.Create(list);
        Create_CallCreateIndex.Create(list);
    }
}