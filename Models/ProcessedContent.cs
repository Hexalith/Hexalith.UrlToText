namespace WebCrawler.Models;

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

[DataContract]
public record ProcessedContent
(
    [property: DataMember] string Url,
    [property: DataMember] string Title,
    [property: DataMember] string MainContent,
    [property: DataMember] IEnumerable<string> Headers,
    [property: DataMember] IDictionary<string, string> Metadata,
    [property: DataMember] IEnumerable<string> ImageAltTexts,
    [property: DataMember] DateTime Timestamp
);
