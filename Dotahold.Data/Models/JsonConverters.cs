using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dotahold.Data.Models
{
    public class SafeIntConverter : JsonConverter<int>
    {
        public override int Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    if (int.TryParse(stringValue, out int value))
                    {
                        return value;
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    else if (reader.TryGetInt64(out long longValue))
                    {
                        if (longValue > int.MaxValue || longValue < int.MinValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)longValue;
                        }
                    }
                    else if (reader.TryGetDouble(out double doubleValue))
                    {
                        if (doubleValue > int.MaxValue || doubleValue < int.MinValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)doubleValue;
                        }
                    }
                    else if (reader.TryGetSingle(out float floatValue))
                    {
                        if (floatValue > int.MaxValue || floatValue < int.MinValue)
                        {
                            return 0;
                        }
                        else
                        {
                            return (int)floatValue;
                        }
                    }
                    else
                    {
                        return 0;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Read error in SafeIntConverter: {ex.Message}");
            }

            return 0;
        }

        public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class SafeDoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.String)
                {
                    var stringValue = reader.GetString();
                    if (double.TryParse(stringValue, out double value))
                    {
                        return value;
                    }
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetDouble(out double value))
                    {
                        return value;
                    }
                    else if (reader.TryGetSingle(out float floatValue))
                    {
                        return floatValue;
                    }
                    else if (reader.TryGetInt32(out int intValue))
                    {
                        return intValue;
                    }
                    else if (reader.TryGetInt64(out long longValue))
                    {
                        return longValue;
                    }
                    else
                    {
                        return 0.0;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Read error in SafeDoubleConverter: {ex.Message}");
            }

            return 0;
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            writer.WriteNumberValue(value);
        }
    }

    public class SafeBoolConverter : JsonConverter<bool>
    {
        public override bool Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.True)
                {
                    return true;
                }
                else if (reader.TokenType == JsonTokenType.False)
                {
                    return false;
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    if (bool.TryParse(reader.GetString(), out bool boolean))
                    {
                        return boolean;
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Read error in SafeBoolConverter: {ex.Message}");
            }

            return false;
        }

        public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        {
            writer.WriteBooleanValue(value);
        }
    }

    public class SafeStringConverter : JsonConverter<string>
    {
        public override string Read(ref Utf8JsonReader reader, System.Type typeToConvert, JsonSerializerOptions options)
        {
            try
            {
                if (reader.TokenType == JsonTokenType.StartArray)
                {
                    StringBuilder stringBuilder = new();
                    bool first = true;

                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonTokenType.EndArray)
                        {
                            break;
                        }

                        if (!first)
                        {
                            stringBuilder.Append(", ");
                        }
                        else
                        {
                            first = false;
                        }

                        string append = string.Empty;

                        if (reader.TokenType == JsonTokenType.String)
                        {
                            append = reader.GetString() ?? string.Empty;
                        }
                        else if (reader.TokenType == JsonTokenType.Number)
                        {
                            if (reader.TryGetDouble(out double doubleValue))
                            {
                                append = doubleValue.ToString();
                            }
                            else if (reader.TryGetInt32(out int intValue))
                            {
                                append = intValue.ToString();
                            }
                        }

                        stringBuilder.Append(append);
                    }

                    return stringBuilder.ToString();
                }
                else if (reader.TokenType == JsonTokenType.String)
                {
                    return reader.GetString() ?? string.Empty;
                }
                else if (reader.TokenType == JsonTokenType.Number)
                {
                    if (reader.TryGetDouble(out double doubleValue))
                    {
                        return doubleValue.ToString();
                    }
                    else if (reader.TryGetSingle(out float floatValue))
                    {
                        return floatValue.ToString();
                    }
                    else if (reader.TryGetInt32(out int intValue))
                    {
                        return intValue.ToString();
                    }
                    else if (reader.TryGetInt64(out long longValue))
                    {
                        return longValue.ToString();
                    }
                }
                else
                {
                    reader.Skip();
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine($"Read error in SafeStringConverter: {ex.Message}");
            }

            return string.Empty;
        }

        public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value);
        }
    }
}
