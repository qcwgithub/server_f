using System;
using System.Collections.Generic;
using System.Linq;

public class DbFilesConfig
{
    public string server_var;
    public string server_path;
    public string serviceType;//DBPlayer,DBAccount
    public string serviceClassName;//DBPlayerService,DBAccountService
    public string scriptFolder;//DBPlayer,DBAccount
    public string scriptOnStart;

    public string PersistenceTaskQueueHandler_class;
    public string PersistenceTaskQueueHandler_path;
    public Func<string, string, string> PersistenceTaskQueueHandler_path2;
}

public class ServerDataConfig
{
    public string originalProfileType;
    public string profileType;
    public string profileType_dbPostfix
    {
        get
        {
            return this.profileType + this.dbPostfix;
        }
    }
    public string dbPostfix
    {
        get
        {
            return this.unsetEmptyField ? "_Db" : string.Empty;
        }
    }
    public void PushUsingOriginal(FileFormatter ff)
    {
        if (!string.IsNullOrEmpty(this.originalProfileType))
        {
            ff.TabPushF("using {0} = Data.{1};\n", this.profileType, this.originalProfileType);
            if (this.unsetEmptyField)
            {
                ff.TabPushF("using {0}_Db = Data.{1}_Db;\n", this.profileType, this.originalProfileType);
            }
            ff.TabPush("\n");
        }
    }
    public bool createCollectionCs;
    public string fileName;
    public string className;
    public bool partial;
    public string collectionName;
    public bool unsetEmptyField;
    public string dbName;
    public string dbCodeName
    {
        get
        {
            if (dbName == c_dbPlayer)
            {
                return "dbData";
            }
            else
            {
                throw new Exception();
            }
        }
    }

    //
    public const string c_dbPlayer = "dbPlayer";
    public static readonly List<string> c_dbNames = new List<string>
    {
        c_dbPlayer,
    };

    public static Dictionary<string, DbFilesConfig> s_dbFilesConfigDict;

    public DbFilesConfig dbFilesConfig
    {
        get
        {
            if (s_dbFilesConfigDict == null)
            {
                s_dbFilesConfigDict = new Dictionary<string, DbFilesConfig>();

                s_dbFilesConfigDict[c_dbPlayer] = new DbFilesConfig
                {
                    server_var = "server",
                    server_path = "Script/Common/Server.cs",
                    serviceType = "Db",
                    serviceClassName = "DbService",
                    scriptFolder = "Db",
                    scriptOnStart = "Db_Start",
                    PersistenceTaskQueueHandler_class = "PersistenceTaskQueueHandler",
                    PersistenceTaskQueueHandler_path = "Script/Db/PersistenceTask/PersistenceTaskQueueHandler.cs",
                    PersistenceTaskQueueHandler_path2 = (profileType, postfix) => $"Script/Db/PersistenceTask/PersistenceTaskQueueHandler.{profileType}{postfix}.cs",
                };
            }
            return s_dbFilesConfigDict[this.dbName];
        }
    }

    public class Index
    {
        public bool isUnique;
        public string fieldName;
        public string fieldName2;
    }
    public List<Index> index;

    public class Field
    {
        public string type;
        public string typeModifier;
        public string name;
    }

    public class Query
    {
        public string cond; // eq, gte, all, in, range, max, eq2
        public Field condField;
        public Field condField2;
        public string output; // info, list, dict, dict2
        public Field outputField1;
        public Field outputField2;
        public Field outputField3;

        // runtime
        public string methodName;
        public string[] methodParamExps;
        public string returnExp;
    }
    public List<Query> query;

    public class Save
    {
        public string cond; // eq, gte, singleton
        public Field field;
        public Field field2;

        // runtime
        public string methodName;
    }
    public List<Save> save;

    public string redisDb;
    public string keyFunc;
    public List<Field> keyParam;

    System.Text.StringBuilder sb = new System.Text.StringBuilder();
    public string keyParamToString(
        bool withType, bool withName, string namePrefix,
        bool withType2, bool withName2, bool withDefaultValue)
    {
        sb.Clear();

        bool needComma = false;
        for (int j = 0; j < 2; j++)
        {
            if (j < this.keyParam.Count)
            {
                if (needComma)
                {
                    sb.Append(", ");
                    needComma = false;
                }
                if (withType)
                {
                    sb.Append(this.keyParam[j].type);
                    needComma = true;
                }
                if (withName)
                {
                    if (withType)
                        sb.Append(" ");
                    else
                        sb.Append(namePrefix);
                    sb.Append(this.keyParam[j].name);
                    needComma = true;
                }
            }
            else if (withType2 || withName2 || withDefaultValue)
            {
                if (needComma)
                {
                    sb.Append(", ");
                    needComma = false;
                }
                if (withType2)
                {
                    sb.Append("int");
                    needComma = true;
                }
                if (withName2)
                {
                    if (withType2)
                        sb.Append(" ");
                    sb.Append("_" + (j + 1));
                    needComma = true;
                }
                if (withDefaultValue)
                {
                    if (withName2)
                    {
                        sb.Append(" = ");
                    }
                    sb.Append("0");
                    needComma = true;
                }
            }
        }

        return sb.ToString();
    }

    public bool createPersistence;
    public bool createProxy;
    public int loadUseQueryIndex;
    public bool proxyIsPartial;
    public bool createPlaceholderWhenNull;
    public bool canExpire;

    public enum TaskQueueHash
    {
        NoKey,
        FirstKey,
        TwoKeys,
        SecondKey,
    }
    public TaskQueueHash taskQueueHash;

    public class Copy
    {
        public int index;
        public ServerDataCopyConfig config;
    }
    public Copy copy;
    public string postfix
    {
        get
        {
            if (this.copy == null || this.copy.index == 0)
            {
                return string.Empty;
            }
            else
            {
                return "_" + (this.copy.index + 1).ToString();
            }
        }
    }

    public string InsertRedisKeyParam(string keyParamString)
    {
        if (this.copy == null)
        {
            return keyParamString;
        }
        else
        {
            string s = this.copy.config.name + ".Id_" + (this.copy.index + 1);
            if (string.IsNullOrEmpty(keyParamString))
            {
                return s;
            }
            else
            {
                return s + ", " + keyParamString;
            }
        }
    }
}

