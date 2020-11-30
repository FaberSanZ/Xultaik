
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GltfLoader
{
    internal class ArrayConverter : JsonConverter<Array>
    {
        private static bool IsEnum(Type t)
        {
            return t.GetTypeInfo().IsEnum;
        }

        public override Array Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (typeToConvert == typeof(bool[]))
            {
                return this.ReadImpl<bool>(ref reader);
            }

            if (typeToConvert == typeof(int[]))
            {
                return this.ReadImpl<int>(ref reader);
            }

            if (typeToConvert == typeof(string[]))
            {
                return this.ReadImpl<string>(ref reader);
            }

            if (typeToConvert == typeof(float[]))
            {
                return this.ReadImpl<float>(ref reader);
            }

            Type type = typeToConvert.IsArray && ArrayConverter.IsEnum(typeToConvert.GetElementType()) ? typeToConvert.GetElementType() : throw new NotImplementedException();
            int[] numArray = this.ReadImpl<int>(ref reader);
            Array instance = Array.CreateInstance(type, numArray.Length);
            for (int index = 0; index < numArray.Length; ++index)
            {
                IEnumerator enumerator = Enum.GetValues(type).GetEnumerator();
                do
                {
                    enumerator.MoveNext();
                }
                while ((int)enumerator.Current != numArray[index]);
                instance.SetValue(enumerator.Current, index);
            }
            return instance;
        }

        private T[] ReadImpl<T>(ref Utf8JsonReader reader)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
            {
                return new T[1]
                {
                    (T) this.GetValue(typeof (T), ref reader)
                };
            }


            reader.Read();
            List<T> objList = new List<T>();
            while (reader.TokenType != JsonTokenType.EndArray)
            {
                objList.Add((T)this.GetValue(typeof(T), ref reader));
                reader.Read();
            }
            return objList.ToArray();
        }

        private object GetValue(Type type, ref Utf8JsonReader reader)
        {
            if (type == typeof(bool))
            {
                return reader.GetBoolean();
            }

            if (type == typeof(int))
            {
                return reader.GetInt32();
            }

            if (type == typeof(string))
            {
                return reader.GetString();
            }

            if (type == typeof(float))
            {
                return reader.GetSingle();
            }

            throw new NotSupportedException();
        }

        public virtual bool CanConvert(Type type)
        {
            return true;
        }


        public override void Write(Utf8JsonWriter writer, Array value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }


        //public ArrayConverter() => base.\u002Ector();
    }
}
