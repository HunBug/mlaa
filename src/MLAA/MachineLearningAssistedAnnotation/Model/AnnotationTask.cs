using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mlaa.Model
{
    internal class AnnotationTask
    {
        public AnnotationTask(IFrameImageSource frameImageSource)
        {
            FrameImageSource = frameImageSource;
        }
        public IFrameImageSource FrameImageSource { get; private set; }
        public List<Sample> Samples { get; private set; } = new List<Sample>();

        internal void Save(string path)
        {
            //save to json
            JsonSerializerOptions defaultJsonSerializerOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };
            string jsonString = JsonSerializer.Serialize(this, defaultJsonSerializerOptions);
            File.WriteAllText(path, jsonString);

        }
    }
}
