using System;
using System.IO;
public class Create_collection_x_info
{
    public static void Create(ServerDataConfig config)
    {
        string content = GetContent(config);

        string path = $"Script/{config.dbFilesConfig.scriptFolder}/collections/{config.fileName}{config.postfix}.cs";

        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, content);
    }

    public static string GetContent(ServerDataConfig config)
    {
        FileFormatter ff = new FileFormatter();
        ff.TabPush("//// AUTO CREATED ////\n");

        ff.TabPush("using System.Collections.Generic;\n");
        ff.TabPush("using System.Threading.Tasks;\n");
        ff.TabPush("using Data;\n");
        ff.TabPush("using MongoDB.Driver;\n");
        ff.TabPush("using Script;\n");
        ff.TabPush("using System.Linq;\n");

        config.PushUsingOriginal(ff);

        ff.Push("\n");
        ff.TabPush("//// AUTO CREATED ////\n");
        // ff.TabPush("public ", config.partial ? "partial " : "", "class ", config.className, config.postfix, " : IServiceScript<NormalServer, DBPlayerService>\n");
        ff.TabPushF("public {0} class {1}{2} : ServiceScript<{3}>\n",
            config.partial ? "partial " : "",
            config.className, config.postfix,
            config.dbFilesConfig.serviceClassName);

        ff.BlockStart();
        {
            ff.TabPush("public const string COLLECTION = \"", config.collectionName, config.postfix, "\";\n");
            // ff.TabPushF("public {0} server {{ get; set; }}\n", config.dbFilesConfig.server_class);
            // ff.TabPushF("public {0} service {{ get; set; }}\n", config.dbFilesConfig.serviceClassName);
            ff.TabPush("MongoClient mongoClient => this.server.data.mongoClient;\n");
            ff.TabPush("string dbName => this.server.data.mongoDBConfig.", config.dbCodeName, ";\n");

            ff.Push("\n");
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPushF("public {0}{1}(Server server, {2} service) : base(server, service)\n", config.className, config.postfix, config.dbFilesConfig.serviceClassName);
            ff.BlockStart();
            ff.BlockEnd();

            ff.Push("\n");
            ff.TabPush("//// AUTO CREATED ////\n");
            ff.TabPush("public IMongoCollection<", config.xinfoType, "> GetCollection()\n");
            ff.BlockStart();
            {
                ff.TabPush("// It’s ok if the database doesn’t yet exist. It will be created upon first use.\n");
                ff.TabPush("IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);\n");
                ff.TabPush("IMongoCollection<", config.xinfoType, "> collection = database.GetCollection<", config.xinfoType, ">(COLLECTION);\n");
                ff.TabPush("return collection;\n");
            }
            ff.BlockEnd();

            if (config.unsetEmptyField)
            {
                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPush("public IMongoCollection<", config.xinfoType_dbPostfix, "> GetCollection_Db()\n");
                ff.BlockStart();
                {
                    ff.TabPush("// It’s ok if the database doesn’t yet exist. It will be created upon first use.\n");
                    ff.TabPush("IMongoDatabase database = this.mongoClient.GetDatabase(this.dbName);\n");
                    ff.TabPush("IMongoCollection<", config.xinfoType_dbPostfix, "> collection = database.GetCollection<", config.xinfoType_dbPostfix, ">(COLLECTION);\n");
                    ff.TabPush("return collection;\n");
                }
                ff.BlockEnd();
            }

            if (config.index.Count > 0)
            {
                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPush("public async Task CreateIndex()\n");
                ff.BlockStart();
                {
                    foreach (var item in config.index)
                    {
                        if (!string.IsNullOrEmpty(item.fieldName2))
                        {
                            ff.TabPushF("await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, new List<string> {{ nameof({0}.{1}), nameof({0}.{2}) }}, true, {3}, this.service.logger);\n",
                                config.xinfoType, item.fieldName, item.fieldName2, item.isUnique ? "true" : "false");
                        }
                        else
                            ff.TabPush("await MongoHelper.CreateIndex(this.mongoClient, this.dbName, COLLECTION, nameof(", config.xinfoType, ".", item.fieldName, "), true, ", item.isUnique ? "true" : "false", ", this.service.logger);\n");
                    }
                }
                ff.BlockEnd();
            }

            foreach (var query in config.query)
            {
                query.methodName = string.Empty;
                // query.methodParamExps = string.Empty;


                string p = "Query";
                if (query.cond == "iter" || query.cond == "iter_nd")
                {
                    p = "Iterate";
                }

                string methodNamePrefix = string.Empty;
                switch (query.output)
                {
                    case "one":
                        {
                            methodNamePrefix = p + "_" + config.xinfoType;
                            if (query.outputField1.type != config.xinfoType)
                            {
                                methodNamePrefix += "_" + query.outputField1.name;
                            }
                        }
                        break;

                    case "list":
                        {
                            methodNamePrefix = p + "_listOf_" + config.xinfoType;
                            if (query.outputField1.type != config.xinfoType)
                            {
                                methodNamePrefix += "_" + query.outputField1.name;
                            }
                        }
                        break;

                    case "dict":
                        {
                            methodNamePrefix = p + "_dictOf_" + config.xinfoType;
                            methodNamePrefix += "_" + query.outputField1.name + "_" + query.outputField2.name;
                        }
                        break;

                    case "dict2":
                        {
                            methodNamePrefix = p + "_dictOf_" + config.xinfoType;
                            methodNamePrefix += "_" + query.outputField1.name + "_" + query.outputField2.name + "_" + query.outputField3.name;
                        }
                        break;

                    default:
                        throw new Exception();
                }

                bool hasProjection = false;
                switch (query.cond)
                {
                    case "eq":
                        // case "gte":
                        query.methodName = methodNamePrefix + "_by_" + query.condField.name;
                        query.methodParamExps = new string[] { query.condField.type + " " + query.condField.name };
                        break;
                    case "eq2":
                        query.methodName = methodNamePrefix + "_by_" + query.condField.name + "_" + query.condField2.name;
                        query.methodParamExps = new string[] { query.condField.type + " " + query.condField.name, query.condField2.type + " " + query.condField2.name };
                        break;
                    case "all":
                        query.methodName = methodNamePrefix + "_all";
                        query.methodParamExps = new string[0];
                        break;
                    case "in":
                        query.methodName = methodNamePrefix + "_byListOf_" + query.condField.name;
                        // methodName = "QueryBy_" + query.condField.name + "List";
                        query.methodParamExps = new string[] { string.Format("List<{0}> {1}List", query.condField.type, query.condField.name) };
                        break;
                    case "range":
                        query.methodName = methodNamePrefix + "_byRangeOf_" + query.condField.name;
                        // methodName = "QueryBy_" + query.condField.name + "Range";
                        query.methodParamExps = new string[]
                        {
                            string.Format("{0} min_{1}", query.condField.type, query.condField.name),
                            string.Format("{0} max_{1}", query.condField.type, query.condField.name)
                        };
                        break;
                    case "max":
                        query.methodName = string.Format("Query_{0}_maxOf_{1}", config.xinfoType, query.condField.name);
                        query.methodParamExps = new string[0];
                        break;
                    case "max_by_serverId":
                        query.methodName = string.Format("Query_{0}_maxOf_{1}_by_serverId", config.xinfoType, query.condField.name);
                        query.methodParamExps = new string[] { "int serverId" };
                        break;
                    case "iter":
                        query.methodName = methodNamePrefix + "_by_" + query.condField.name;
                        query.methodParamExps = new string[]
                        {
                            string.Format("{0} start_{1}", query.condField.type, query.condField.name),
                            string.Format("{0} end_{1}", query.condField.type, query.condField.name),
                        };
                        break;
                    case "iter_nd":
                        query.methodName = methodNamePrefix + "_by_" + query.condField.name + "_nd";
                        query.methodParamExps = new string[]
                        {
                            string.Format("{0} start_{1}", query.condField.type, query.condField.name),
                            string.Format("{0} end_{1}", query.condField.type, query.condField.name),
                        };
                        break;
                    case "ele":
                        query.methodName = methodNamePrefix + "_byElementOf_" + query.condField.name;
                        query.methodParamExps = new string[]
                        {
                            string.Format("{0} ele_{1}", query.condField.type, query.condField.name),
                        };
                        break;

                    default:
                        throw new Exception();
                }

                query.returnExp = string.Empty;
                switch (query.output)
                {
                    case "one":
                        query.returnExp = query.outputField1.type;
                        break;
                    case "list":
                        query.returnExp = string.Format("List<{0}>", query.outputField1.type);
                        break;
                    case "dict":
                        query.returnExp = string.Format("Dictionary<{0}, {1}>", query.outputField1.type, query.outputField2.type);
                        break;
                    case "dict2":
                        query.returnExp = string.Format("Dictionary<{0}, {1}_{2}_{3}>", query.outputField1.type, config.xinfoType, query.outputField2.name, query.outputField3.name);
                        break;

                    default:
                        throw new Exception();
                }

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPush(string.Format("public async Task<{0}> {1}({2})\n", query.returnExp, query.methodName, string.Join(',', query.methodParamExps)));
                ff.BlockStart();
                {
                    ff.TabPush("var collection = this.GetCollection();\n");

                    switch (query.cond)
                    {
                        case "eq":
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Eq(nameof({0}.{1}), {1});\n", config.xinfoType, query.condField.name));
                            break;
                        case "eq2":
                            ff.TabPushF("var eq1 = Builders<{0}>.Filter.Eq(nameof({0}.{1}), {1});\n", config.xinfoType, query.condField.name);
                            ff.TabPushF("var eq2 = Builders<{0}>.Filter.Eq(nameof({0}.{1}), {1});\n", config.xinfoType, query.condField2.name);
                            ff.TabPushF("var filter = Builders<{0}>.Filter.And(eq1, eq2);\n", config.xinfoType);
                            break;
                        // case "gte":
                        // ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Gte(nameof({0}.{1}), {1});\n", config.xinfoType, query.condField.name));
                        // break;
                        case "all":
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Empty;\n", config.xinfoType));
                            break;

                        case "in":
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.In(nameof({0}.{1}), {1}List);\n", config.xinfoType, query.condField.name));
                            break;

                        case "range":
                            ff.TabPush(string.Format("var gte = Builders<{0}>.Filter.Gte(nameof({0}.{1}), min_{1});\n", config.xinfoType, query.condField.name));
                            ff.TabPush(string.Format("var lte = Builders<{0}>.Filter.Lte(nameof({0}.{1}), max_{1});\n", config.xinfoType, query.condField.name));
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.And(gte, lte);\n", config.xinfoType));
                            break;

                        case "max":
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Gt(nameof({0}.{1}), 0);\n", config.xinfoType, query.condField.name));
                            break;

                        case "iter":
                            ff.TabPushF("MyDebug.Assert(start_{0} < end_{0});\n", query.condField.name);
                            ff.TabPushF("var gte = Builders<{0}>.Filter.Gte(nameof({0}.{1}), start_{1});\n", config.xinfoType, query.condField.name);
                            ff.TabPushF("var lt = Builders<{0}>.Filter.Lt(nameof({0}.{1}), end_{1});\n", config.xinfoType, query.condField.name);
                            ff.TabPushF("var filter = Builders<{0}>.Filter.And(gte, lt);\n", config.xinfoType, query.condField.name);
                            break;

                        case "iter_nd":
                            ff.TabPushF("MyDebug.Assert(start_{0} < end_{0});\n", query.condField.name);
                            ff.TabPushF("var gte = Builders<{0}>.Filter.Gte(nameof({0}.{1}), start_{1});\n", config.xinfoType, query.condField.name);
                            ff.TabPushF("var lt = Builders<{0}>.Filter.Lt(nameof({0}.{1}), end_{1});\n", config.xinfoType, query.condField.name);
                            ff.TabPushF("var nd = Builders<{0}>.Filter.Ne(nameof({0}.{1}), {2});\n", config.xinfoType, "deleted", "true");
                            ff.TabPushF("var filter = Builders<{0}>.Filter.And(gte, lt, nd);\n", config.xinfoType);
                            break;

                        case "ele":
                            ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Eq(nameof({0}.{1}), ele_{1});\n", config.xinfoType, query.condField.name));
                            break;

                        default:
                            throw new Exception();
                    }

                    switch (query.output)
                    {
                        case "one":
                        case "list":
                            {
                                if (query.outputField1.type != config.xinfoType)
                                {
                                    hasProjection = true;
                                    ff.TabPush(string.Format("var projection = Builders<{0}>.Projection.Include(nameof({0}.{1}));\n", config.xinfoType, query.outputField1.name));
                                }
                            }
                            break;

                        case "dict":
                            if (query.outputField2.type != config.xinfoType)
                            {
                                hasProjection = true;
                                ff.TabPush(string.Format("var projection = Builders<{0}>.Projection.Include(nameof({0}.{1})).Include(nameof({0}.{2}));\n", config.xinfoType, query.outputField1.name, query.outputField2.name));
                            }
                            break;

                        case "dict2":
                            if (query.outputField2.type == config.xinfoType || query.outputField2.type == config.xinfoType)
                            {
                                throw new NotImplementedException();
                            }

                            hasProjection = true;
                            ff.TabPush(string.Format("var projection = Builders<{0}>.Projection.Include(nameof({0}.{1})).Include(nameof({0}.{2})).Include(nameof({0}.{3}));\n",
                                config.xinfoType, query.outputField1.name, query.outputField2.name, query.outputField3.name));
                            break;

                        default:
                            throw new Exception();
                    }

                    ff.TabPush("var find = collection.Find(filter)");

                    switch (query.cond)
                    {
                        case "max":
                        case "max_by_serverId":
                            ff.Push("\n").AddTab(1).TabPush(string.Format(".SortByDescending(x => x.{0})", query.condField.name)).AddTab(-1);
                            ff.Push("\n").AddTab(1).TabPush(string.Format(".Skip(0)")).AddTab(-1);
                            ff.Push("\n").AddTab(1).TabPush(string.Format(".Limit(1)")).AddTab(-1);
                            break;

                        case "iter":
                        case "iter_nd":
                            ff.Push("\n").AddTab(1).TabPush(".Limit(1000)").AddTab(-1);
                            break;
                    }

                    if (hasProjection)
                    {
                        ff.Push("\n").AddTab(1).TabPush(".Project(projection)").AddTab(-1);
                    }

                    ff.Push(";\n");

                    ff.Push("\n");

                    bool isToList = false;
                    switch (query.cond)
                    {
                        case "max":
                        case "max_by_serverId":
                            isToList = true;
                            break;
                    }
                    switch (query.output)
                    {
                        case "list":
                        case "dict":
                        case "dict2":
                            isToList = true;
                            break;
                    }
                    if (isToList)
                    {
                        ff.TabPush("var result = await find.ToListAsync();\n");
                    }
                    else
                    {
                        ff.TabPush("var result = await find.FirstOrDefaultAsync();\n");
                    }

                    switch (query.output)
                    {
                        case "one":
                            if (query.outputField1.type == config.xinfoType)
                            {
                                if (!isToList)
                                {
                                    ff.TabPush("return result;\n");
                                }
                                else
                                {
                                    ff.TabPush(string.Format("return result.Count > 0 ? result[0] : default({0});\n", config.xinfoType));
                                }
                            }
                            else
                            {
                                if (!isToList)
                                {
                                    ff.TabPush(string.Format("return result != null ? ({0})result[nameof({1}.{2})] : default({0});\n",
                                        query.outputField1.type, config.xinfoType, query.outputField1.name));
                                }
                                else
                                {
                                    ff.TabPush(string.Format("return result.Count > 0 ? ({0})result[0][nameof({1}.{2})] : default({0});\n",
                                        query.outputField1.type, config.xinfoType, query.outputField1.name));
                                }
                            }
                            break;

                        case "list":
                            if (query.outputField1.type == config.xinfoType)
                            {
                                ff.TabPush("return result;\n");
                            }
                            else
                            {
                                ff.TabPush(string.Format("return result.Select(_ => ({0})_[nameof({1}.{2})]).ToList();\n",
                                    query.outputField1.type, config.xinfoType, query.outputField1.name));
                            }
                            break;

                        case "dict":
                            ff.TabPush(string.Format("var dict = new Dictionary<{0}, {1}>();\n",
                                query.outputField1.type, query.outputField2.type));
                            ff.TabPush("foreach (var item in result)\n");
                            ff.BlockStart();
                            if (query.outputField2.type == config.xinfoType)
                            {
                                ff.TabPush(string.Format("dict[item.{0}] = item;\n", query.outputField1.name));
                            }
                            else
                            {
                                ff.TabPush(string.Format("dict[({0})item[nameof({1}.{2})]] = ({3})item[nameof({1}.{4})];\n",
                                    query.outputField1.type, config.xinfoType, query.outputField1.name,
                                    query.outputField2.type, query.outputField2.name));
                            }
                            ff.BlockEnd();
                            ff.TabPush("return dict;\n");
                            break;

                        case "dict2":
                            ff.TabPush(string.Format("var dict = new Dictionary<{0}, {1}_{2}_{3}>();\n",
                                query.outputField1.type, config.xinfoType, query.outputField2.name, query.outputField3.name));
                            ff.TabPush("foreach (var item in result)\n");
                            ff.BlockStart();
                            ff.TabPushF("{0} {2} = ({0})item[nameof({1}.{2})];\n", query.outputField1.type, config.xinfoType, query.outputField1.name);
                            ff.TabPushF("{0} {2} = ({0})item[nameof({1}.{2})];\n", query.outputField2.type, config.xinfoType, query.outputField2.name);
                            ff.TabPushF("{0} {2} = ({0})item[nameof({1}.{2})];\n", query.outputField3.type, config.xinfoType, query.outputField3.name);

                            ff.TabPushF("dict[{0}] = new {3}_{1}_{2} {{ {1} = {1}, {2} = {2} }};\n",
                                query.outputField1.name, query.outputField2.name, query.outputField3.name, config.xinfoType);
                            ff.BlockEnd();
                            ff.TabPush("return dict;\n");
                            break;

                        default:
                            throw new Exception();
                    }
                }
                ff.BlockEnd();
            }

            foreach (var item in config.save)
            {
                item.methodName = "Save";

                ff.Push("\n");
                ff.TabPush("//// AUTO CREATED ////\n");
                ff.TabPushF("public async Task<ECode> {0}({1} info)\n", item.methodName, config.xinfoType);
                ff.BlockStart();
                {
                    ff.TabPushF("var collection = this.GetCollection{0}();\n", config.dbPostfix);

                    switch (item.cond)
                    {
                        case SaveCond.eq:
                            {
                                ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.{2}(nameof({0}.{1}), info.{1});\n",
                                    config.xinfoType_dbPostfix, item.field.name, "Eq"));
                            }
                            break;

                        case SaveCond.eq2:
                            {
                                ff.TabPushF("var eq1 = Builders<{0}>.Filter.Eq(nameof({0}.{1}), info.{1});\n", config.xinfoType_dbPostfix, item.field.name);
                                ff.TabPushF("var eq2 = Builders<{0}>.Filter.Eq(nameof({0}.{1}), info.{1});\n", config.xinfoType_dbPostfix, item.field2.name);
                                ff.TabPushF("var filter = Builders<{0}>.Filter.And(eq1, eq2);\n", config.xinfoType_dbPostfix);
                            }
                            break;

                        case SaveCond.gte:
                            {
                                ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.{2}(nameof({0}.{1}), info.{1});\n",
                                    config.xinfoType_dbPostfix, item.field.name, "Gte"));
                            }
                            break;

                        case SaveCond.singleton:
                            {
                                ff.TabPush(string.Format("var filter = Builders<{0}>.Filter.Empty;\n", config.xinfoType_dbPostfix));
                            }
                            break;

                        case SaveCond.multiple:
                            {

                            }
                            break;

                        default:
                            throw new Exception();
                    }

                    switch (item.cond)
                    {
                        case SaveCond.eq:
                        case SaveCond.eq2:
                        case SaveCond.gte:
                        case SaveCond.singleton:
                            {
                                if (config.unsetEmptyField)
                                {
                                    ff.TabPush("var info_Db = XInfoHelper_Db.Copy_Class<{0}, {1}>(info);\n".Format(config.xinfoType_dbPostfix, config.xinfoType));
                                    ff.TabPush("await collection.ReplaceOneAsync(filter, info_Db, new ReplaceOptions { IsUpsert = true });\n");
                                }
                                else
                                {
                                    ff.TabPush("await collection.ReplaceOneAsync(filter, info, new ReplaceOptions { IsUpsert = true });\n");
                                }
                            }
                            break;
                        case SaveCond.multiple:
                            {
                                ff.TabPush("await collection.InsertOneAsync(info);\n");
                            }
                            break;
                        default:
                            throw new Exception();

                    }
                    ff.TabPush("return ECode.Success;\n");
                }
                ff.BlockEnd();
            }
        }
        ff.BlockEnd();
        ff.Push("\n");
        ff.TabPush("//// AUTO CREATED ////\n");
        return ff.GetString();
    }
}