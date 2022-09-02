using System;
using System.Collections.Generic;
using System.Globalization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace GasaiYuno.Discord.Models;

public partial struct TranslationValue
{
    public Dictionary<string, TranslationValue> AnythingMap;
    public string String;

    public static implicit operator TranslationValue(Dictionary<string, TranslationValue> AnythingMap) => new TranslationValue { AnythingMap = AnythingMap };
    public static implicit operator TranslationValue(string String) => new TranslationValue { String = String };
}

/// <summary>
/// Json schema to be used with the Default Json Localization Manager of D.NET
/// </summary>
public partial struct TranslationUnion
{
    public Dictionary<string, Dictionary<string, TranslationValue>> AnythingMapMap;
    public string String;

    public static implicit operator TranslationUnion(Dictionary<string, Dictionary<string, TranslationValue>> AnythingMapMap) => new TranslationUnion { AnythingMapMap = AnythingMapMap };
    public static implicit operator TranslationUnion(string String) => new TranslationUnion { String = String };
}

public class Translation
{
    public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings);
}

public static class Serialize
{
    public static string ToJson(this object self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
        {
            TranslationUnionConverter.Singleton,
            TranslationValueConverter.Singleton,
            new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
        },
    };
}

internal class TranslationUnionConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(TranslationUnion) || t == typeof(TranslationUnion?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.String:
            case JsonToken.Date:
                var stringValue = serializer.Deserialize<string>(reader);
                return new TranslationUnion { String = stringValue };
            case JsonToken.StartObject:
                var objectValue = serializer.Deserialize<Dictionary<string, Dictionary<string, TranslationValue>>>(reader);
                return new TranslationUnion { AnythingMapMap = objectValue };
        }

        throw new Exception("Cannot unmarshal type TranslationUnion");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        var value = (TranslationUnion) untypedValue;
        if (value.String != null)
        {
            serializer.Serialize(writer, value.String);
            return;
        }

        if (value.AnythingMapMap != null)
        {
            serializer.Serialize(writer, value.AnythingMapMap);
            return;
        }

        throw new Exception("Cannot marshal type TranslationUnion");
    }

    public static readonly TranslationUnionConverter Singleton = new TranslationUnionConverter();
}

internal class TranslationValueConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(TranslationValue) || t == typeof(TranslationValue?);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        switch (reader.TokenType)
        {
            case JsonToken.String:
            case JsonToken.Date:
                var stringValue = serializer.Deserialize<string>(reader);
                if (stringValue.Length >= 1 && stringValue.Length <= 100)
                {
                    return new TranslationValue { String = stringValue };
                }

                break;
            case JsonToken.StartObject:
                var objectValue = serializer.Deserialize<Dictionary<string, TranslationValue>>(reader);
                return new TranslationValue { AnythingMap = objectValue };
        }

        throw new Exception("Cannot unmarshal type TranslationValue");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        var value = (TranslationValue) untypedValue;
        if (value.String != null)
        {
            if (value.String.Length >= 1 && value.String.Length <= 100)
            {
                serializer.Serialize(writer, value.String);
                return;
            }
        }

        if (value.AnythingMap != null)
        {
            serializer.Serialize(writer, value.AnythingMap);
            return;
        }

        throw new Exception("Cannot marshal type TranslationValue");
    }

    public static readonly TranslationValueConverter Singleton = new TranslationValueConverter();
}

internal class MinMaxLengthCheckConverter : JsonConverter
{
    public override bool CanConvert(Type t) => t == typeof(string);

    public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
    {
        var value = serializer.Deserialize<string>(reader);
        if (value.Length >= 1 && value.Length <= 100)
        {
            return value;
        }

        throw new Exception("Cannot unmarshal type string");
    }

    public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
    {
        var value = (string) untypedValue;
        if (value.Length >= 1 && value.Length <= 100)
        {
            serializer.Serialize(writer, value);
            return;
        }

        throw new Exception("Cannot marshal type string");
    }

    public static readonly MinMaxLengthCheckConverter Singleton = new MinMaxLengthCheckConverter();
}