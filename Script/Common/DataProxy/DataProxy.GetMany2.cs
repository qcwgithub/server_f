using System;
using Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using System.Diagnostics;
using System.Linq;

namespace Script
{
    public abstract partial class DataProxy<DataType, P1, P2>
    {
        public async Task<List<DataType>> GetManyHelp(ConnectToDbService connectToDbService, P1 p1, List<P2> p2s)
        {
            RedisValue[] values = await this.GetDb().StringGetAsync(p2s.Select(p2 => this.Key(p1, p2)).ToArray());

            var list = new List<DataType>();

            List<Task<DataType>> tasks = null;
            List<int> indexes = null;
            for (int i = 0; i < values.Length; i++)
            {
                list.Add(null);
                if (!values[i].IsNullOrEmpty)
                {
                    var data = JsonUtils.parse<DataType>(values[i]);
                    if (!data.IsPlaceholder())
                    {
                        list[i] = data;
                    }
                }
                else
                {
                    if (tasks == null)
                    {
                        tasks = new List<Task<DataType>>();
                        indexes = new List<int>();
                    }

                    tasks.Add(this.InternalGet(connectToDbService, p1, p2s[i]));
                    indexes.Add(i);
                }
            }

            if (tasks != null)
            {
                await Task.WhenAll(tasks);

                for (int i = 0; i < tasks.Count; i++)
                {
                    int index = indexes[i];
                    list[index] = tasks[i].Result;
                }
            }

            return list;
        }

        public async Task<List<DataType>> GetMany(ConnectToDbService connectToDbService, P1 p1, List<P2> p2s, bool fillNullIfNotExist)
        {
            var list2 = await this.GetManyHelp(connectToDbService, p1, p2s);
            if (fillNullIfNotExist)
            {
                return list2;
            }

            var list = new List<DataType>();
            for (int i = 0; i < p2s.Count; i++)
            {
                if (list2[i] != null)
                {
                    list.Add(list2[i]);
                }
            }

            return list;
        }

        public async Task UnloadMany(P1 p1, IEnumerable<P2> p2s)
        {
            await this.GetDb().KeyDeleteAsync(p2s.Select(x => this.Key(p1, x)).ToArray());
        }
    }
}