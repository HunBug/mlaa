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
        private JsonSerializerOptions defaultJsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
        };
        public AnnotationTask(IFrameImageSource frameImageSource)
        {
            FrameImageSource = frameImageSource;
        }
        public IFrameImageSource FrameImageSource { get; private set; }
        public List<Sample> Samples { get; private set; } = new List<Sample>();

        internal void Save(string path)
        {
            //save to json
            string jsonString = JsonSerializer.Serialize(Samples, defaultJsonSerializerOptions);
            File.WriteAllText(path, jsonString);
        }
        
        internal void Load(string path)
        {
            //load from json
            string jsonString = File.ReadAllText(path);
            List<Sample>? samples = JsonSerializer.Deserialize<List<Sample>>(jsonString, defaultJsonSerializerOptions);
            if (samples != null)
            {
                Samples = samples;
            }
            else
            {
                throw new Exception($"Failed to samples from {path}");
            }
        }
    }
}
