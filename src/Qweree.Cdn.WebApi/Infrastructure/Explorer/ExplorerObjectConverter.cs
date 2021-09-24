using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Qweree.Cdn.Sdk.Explorer;

namespace Qweree.Cdn.WebApi.Infrastructure.Explorer
{
    public class ExplorerObjectConverter : JsonConverter<IExplorerObjectDto>
    {
        public override IExplorerObjectDto Read(ref Utf8JsonReader reader, Type typeToConvert,
            JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(
            Utf8JsonWriter writer,
            IExplorerObjectDto value,
            JsonSerializerOptions options)
        {
            switch (value)
            {
                case null:
                    JsonSerializer.Serialize(writer, (IExplorerObjectDto?) null, options);
                    break;
                case ExplorerFileDto file:
                    JsonSerializer.Serialize(writer, file, options);
                    break;
                case ExplorerDirectoryDto directory:
                    JsonSerializer.Serialize(writer, directory, options);
                    break;
                default:
                {
                    var type = value.GetType();
                    JsonSerializer.Serialize(writer, value, type, options);
                    break;
                }
            }
        }
    }
}