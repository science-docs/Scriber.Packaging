using System.Text.Json.Serialization;

namespace Scriber.Packaging
{
    public class Config
    {
        [JsonPropertyName("rewrite")]
        public RewriteConfig? Rewrite { get; set; }
        [JsonPropertyName("build")]
        public BuildConfig? Build { get; set; }
        [JsonPropertyName("documentation")]
        public DocumentationConfig? Documentation { get; set; }
    }

    public class DocumentationConfig
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; } = true;
        [JsonPropertyName("output")]
        public string? OutputFolder { get; set; }
    }

    public class BuildConfig
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; } = true;
        [JsonPropertyName("project")]
        public string? ProjectFile { get; set; }
    }

    public class RewriteConfig
    {
        [JsonPropertyName("enabled")]
        public bool? Enabled { get; set; } = true;
    }
}
