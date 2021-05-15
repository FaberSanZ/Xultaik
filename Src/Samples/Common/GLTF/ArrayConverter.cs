// Copyright (c) 2019-2020 Faber Leonardo. All Rights Reserved. https://github.com/FaberSanZ
// This code is licensed under the MIT license (MIT) (http://opensource.org/licenses/MIT)

/*=============================================================================
	ArrayConverter.cs
=============================================================================*/


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Vultaik.GLTF
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
                return ReadImpl<bool>(ref reader);

            if (typeToConvert == typeof(int[]))
                return ReadImpl<int>(ref reader);

            if (typeToConvert == typeof(string[]))
                return ReadImpl<string>(ref reader);

            if (typeToConvert == typeof(float[]))
                return ReadImpl<float>(ref reader);

            Type type = typeToConvert.IsArray && IsEnum(typeToConvert.GetElementType()) ? typeToConvert.GetElementType() : throw new NotImplementedException();
            int[] numArray = ReadImpl<int>(ref reader);
            Array instance = Array.CreateInstance(type, numArray.Length);


            for (int i = 0; i < numArray.Length; ++i)
            {
                IEnumerator enumerator = Enum.GetValues(type).GetEnumerator();
                do
                {
                    enumerator.MoveNext();
                }
                while ((int)enumerator.Current != numArray[i]);
                instance.SetValue(enumerator.Current, i);
            }
            return instance;
        }

        private T[] ReadImpl<T>(ref Utf8JsonReader reader)
        {
            if (reader.TokenType is not JsonTokenType.StartArray)
            {
                return new T[1]
                {
                    (T)GetValue(typeof (T), ref reader)
                };
            }


            reader.Read();

            List<T> objList = new();

            while (reader.TokenType is not JsonTokenType.EndArray)
            {
                objList.Add((T)GetValue(typeof(T), ref reader));
                reader.Read();
            }
            return objList.ToArray();
        }

        private object GetValue(Type type, ref Utf8JsonReader reader)
        {
            if (type == typeof(bool))
                return reader.GetBoolean();

            if (type == typeof(int))
                return reader.GetInt32();

            if (type == typeof(string))
                return reader.GetString();

            if (type == typeof(float))
                return reader.GetSingle();

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
