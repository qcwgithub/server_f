using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using MongoDB.Bson.Serialization.Serializers;
using System.Reflection;
using System.Collections.Concurrent;
using System.Numerics;

namespace Script
{
    public class MyBigIntegerSerializer : SerializerBase<BigInteger>
    {
        public override BigInteger Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            string val = context.Reader.ReadString();
            return BigInteger.Parse(val);
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, BigInteger value)
        {
            context.Writer.WriteString(value.ToString());
        }
    }
    class CustomBsonSerializationProvider : IBsonSerializationProvider
    {
        ConcurrentDictionary<Type, IBsonSerializer> cache = new ConcurrentDictionary<Type, IBsonSerializer>();
        IBsonSerializer CacheAndReturn(Type type, IBsonSerializer serializer)
        {
            this.cache[type] = serializer;
            return serializer;
        }

        // 此函数会从另一个线程调用过来
        public IBsonSerializer GetSerializer(Type type)
        {
            if (this.cache.TryGetValue(type, out IBsonSerializer serializer))
            {
                return serializer;
            }

            // 说明
            // 首先我们希望 Dictionary 序列化为 MongoDB 里的 Document
            // 而MongodDB 要求 Document 的 Key 必须序列化为 string

            // 枚举序列化为 string
            // 理由1：会做为 Dictionary 的 Key
            // 理由2：易于阅读
            if (type.IsEnum)
            {
                // make type 'EnumSerializer<type>'
                Type serializerType = typeof(EnumSerializer<>).MakeGenericType(type);

                // find constructor 'EnumSerilizer<type>(BsonType type)'
                ConstructorInfo[] constructors = serializerType.GetConstructors();
                ConstructorInfo constructor = constructors.First(ctor =>
                {
                    ParameterInfo[] parameters = ctor.GetParameters();
                    return parameters.Length == 1 && parameters[0].ParameterType == typeof(BsonType);
                });
                // call constructor
                serializer = (IBsonSerializer)constructor.Invoke(new object[] { BsonType.String });
                return this.CacheAndReturn(type, serializer);
            }

            if (type == typeof(BigInteger))
            {
                return this.CacheAndReturn(type, new MyBigIntegerSerializer());
            }

            if (type.IsGenericType &&
                type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
            {
                Type keyType = type.GetGenericArguments()[0];
                IBsonSerializer keySerializer = null;

                if (keyType == typeof(int))
                {
                    keySerializer = new Int32Serializer(BsonType.String);
                }
                else if (keyType == typeof(long))
                {
                    keySerializer = new Int64Serializer(BsonType.String);
                }
                else
                {
                    return this.CacheAndReturn(type, null);
                }

                Type valueType = type.GetGenericArguments()[1];
                IBsonSerializer valueSerializer = BsonSerializer.SerializerRegistry.GetSerializer(valueType);

                // make type
                // DictionaryInterfaceImplementerSerializer<type>
                Type serializerType = typeof(DictionaryInterfaceImplementerSerializer<>).MakeGenericType(type);

                // find constructor
                // DictionaryInterfaceImplementerSerializer(
                //      DictionaryRepresentation dictionaryRepresentation, 
                //      IBsonSerializer keySerializer, 
                //      IBsonSerializer valueSerializer
                // );
                ConstructorInfo[] constructors = serializerType.GetConstructors();
                ConstructorInfo constructor = constructors.First(ctor =>
                {
                    ParameterInfo[] parameters = ctor.GetParameters();
                    if (parameters.Length == 3 &&
                        parameters[0].ParameterType == typeof(DictionaryRepresentation) &&
                        parameters[1].ParameterType == typeof(IBsonSerializer) &&
                        parameters[2].ParameterType == typeof(IBsonSerializer))
                    {
                        return true;
                    }
                    return false;
                });

                // call constructor
                // DictionaryInterfaceImplementerSerializer(
                //     DictionaryRepresentation.Document,
                //     new Int32Serializer(BsonType.String),
                //     valueSerializer
                // );
                serializer = (IBsonSerializer)constructor.Invoke(new object[] {
                    DictionaryRepresentation.Document,
                    keySerializer,
                    valueSerializer
                });
                return this.CacheAndReturn(type, serializer);
            }

            return this.CacheAndReturn(type, null);
        }
    }
    public static class MongoRegister
    {
        static bool init = false;
        public static void Init()
        {
            if (init)
            {
                return;
            }

            // When a BSON document is deserialized, the name of each element is used to look up a matching member in the class map. 
            // Normally, if no matching member is found, an exception will be thrown.

            // 如果没有下面2句，会报错：
            // System.FormatException: Element '_id' does not match any field or property of class Data.Profile.
            //    at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.DeserializeClass(BsonDeserializationContext context)
            //    at MongoDB.Bson.Serialization.BsonClassMapSerializer`1.Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
            //    at MongoDB.Bson.Serialization.IBsonSerializerExtensions.Deserialize[TValue](IBsonSerializer`1 serializer, BsonDeserializationContext context)
            //    at MongoDB.Driver.Core.Operations.CursorBatchDeserializationHelper.DeserializeBatch[TDocument](RawBsonArray batch, IBsonSerializer`1 documentSerializer, MessageEncoderSettings messageEncoderSettings)
            //    at MongoDB.Driver.Core.Operations.FindCommandOperation`1.CreateFirstCursorBatch(BsonDocument cursorDocument)
            //    at MongoDB.Driver.Core.Operations.FindCommandOperation`1.CreateCursor(IChannelSourceHandle channelSource, IChannelHandle channel, BsonDocument commandResult)
            //    at MongoDB.Driver.Core.Operations.FindCommandOperation`1.ExecuteAsync(RetryableReadContext context, CancellationToken cancellationToken)
            //    at MongoDB.Driver.Core.Operations.FindOperation`1.ExecuteAsync(RetryableReadContext context, CancellationToken cancellationToken)
            //    at MongoDB.Driver.Core.Operations.FindOperation`1.ExecuteAsync(IReadBinding binding, CancellationToken cancellationToken)
            //    at MongoDB.Driver.OperationExecutor.ExecuteReadOperationAsync[TResult](IReadBinding binding, IReadOperation`1 operation, CancellationToken cancellationToken)
            //    at MongoDB.Driver.MongoCollectionImpl`1.ExecuteReadOperationAsync[TResult](IClientSessionHandle session, IReadOperation`1 operation, ReadPreference readPreference, CancellationToken cancellationToken)
            //    at MongoDB.Driver.MongoCollectionImpl`1.UsingImplicitSessionAsync[TResult](Func`2 funcAsync, CancellationToken cancellationToken)
            //    at collection_player.QueryById(Int32 playerId) in D:\Code\pkcastles_trunk\server\Script\DBPlayer\collection_player.cs:line 53
            //    at Script.DBQueryPlayerById.Handle(TcpClientData socket, Object _msg) in D:\Code\pkcastles_trunk\server\Script\DBPlayer\DBQueryPlayerById.cs:line 14
            //    at Script.MessageDispatcher.Dispatch(ProtocolClientData socket, MsgType type, Object msg, Action`2 reply) in D:\Code\pkcastles_trunk\server\Script\Common\MessageDispatcher.cs:line 107

            ConventionPack conventionPack = new ConventionPack { new IgnoreExtraElementsConvention(true) };
            ConventionRegistry.Register("IgnoreExtraElements", conventionPack, type => true);



            BsonSerializer.RegisterSerializationProvider(new CustomBsonSerializationProvider());

            var serializer_ii = BsonSerializer.SerializerRegistry.GetSerializer<Dictionary<long, int>>();
            init = true;
        }
    }
}