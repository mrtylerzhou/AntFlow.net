using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json.Serialization;

namespace AntFlowCore.Vo
{
    public class MailInfo
    {
        [JsonPropertyName("receiver")]
        public string Receiver { get; set; }

        [JsonPropertyName("receivers")]
        public List<string> Receivers { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("cc")]
        public string[] Cc { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("file")]
        public FileInfo File { get; set; }

        [JsonPropertyName("fileInputStream")]
        public Stream FileInputStream { get; set; }
    }
}