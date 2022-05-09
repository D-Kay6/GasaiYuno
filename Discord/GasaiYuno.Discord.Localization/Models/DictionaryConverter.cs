using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GasaiYuno.Discord.Localization.Models;

internal class DictionaryConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) { this.WriteValue(writer, value); }

    private void WriteValue(JsonWriter writer, object value)
    {
        var t = JToken.FromObject(value);
        switch (t.Type)
        {
            case JTokenType.Object:
                this.WriteObject(writer, value);
                break;
            case JTokenType.Array:
                this.WriteArray(writer, value);
                break;
            default:
                writer.WriteValue(value);
                break;
        }
    }

    private void WriteObject(JsonWriter writer, object value)
    {
        writer.WriteStartObject();
        if (value is IDictionary<string, object> obj)
            foreach (var kvp in obj)
            {
                writer.WritePropertyName(kvp.Key);
                this.WriteValue(writer, kvp.Value);
            }

        writer.WriteEndObject();
    }

    private void WriteArray(JsonWriter writer, object value)
    {
        writer.WriteStartArray();
        if (value is IEnumerable<object> array)
            foreach (var o in array)
            {
                this.WriteValue(writer, o);
            }

        writer.WriteEndArray();
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return ReadValue(reader);
    }

    private object ReadValue(JsonReader reader)
    {
        while (reader.TokenType == JsonToken.Comment)
        {
            if (!reader.Read()) throw new JsonSerializationException("Unexpected Token when converting IDictionary<string, object>");
        }

        switch (reader.TokenType)
        {
            case JsonToken.StartObject:
                return ReadObject(reader);
            case JsonToken.StartArray:
                return ReadArray(reader);
            case JsonToken.String:
            case JsonToken.Integer:
            case JsonToken.Float:
            case JsonToken.Boolean:
            case JsonToken.Undefined:
            case JsonToken.Null:
            case JsonToken.Date:
            case JsonToken.Bytes:
                return reader.Value;
            default:
                throw new JsonSerializationException($"Unexpected token when converting IDictionary<string, object>: {reader.TokenType}");
        }
    }

    private object ReadObject(JsonReader reader)
    {
        var obj = new Dictionary<string, object>();

        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonToken.PropertyName:
                    var propertyName = reader.Value?.ToString();
                    if (!reader.Read())
                    {
                        throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
                    }

                    var v = ReadValue(reader);
                    obj[propertyName ?? throw new JsonSerializationException("Unable to read value of IDictionary<string, object>")] = v;
                    break;
                case JsonToken.Comment:
                    break;
                case JsonToken.EndObject:
                    return obj;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
    }

    private object ReadArray(JsonReader reader)
    {
        IList<object> list = new List<object>();
        while (reader.Read())
        {
            switch (reader.TokenType)
            {
                case JsonToken.Comment:
                    break;
                case JsonToken.EndArray:
                    return list;
                default:
                    var v = ReadValue(reader);
                    list.Add(v);
                    break;
            }
        }

        throw new JsonSerializationException("Unexpected end when reading IDictionary<string, object>");
    }

    public override bool CanConvert(Type objectType) { return typeof(IDictionary<string, object>).IsAssignableFrom(objectType); }
}