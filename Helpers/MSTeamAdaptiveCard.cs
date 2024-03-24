using System.Text.Json.Serialization;

public class MSAdaptiveCard
{
    [JsonPropertyName("type")]
    public string Type { get; set; }
    
    [JsonPropertyName("attachments")]
    public List<Attachment> Attachments { get; set; }
}

public class Attachment
{
    public string ContentType { get; set; }
    public AdaptiveCardContent Content { get; set; }
}

public class AdaptiveCardContent
{
    public string Type { get; set; }
    public List<CardBody> Body { get; set; }
    public string Schema { get; set; }
    public string Version { get; set; }
}

public class CardBody
{
    public string Type { get; set; }
    public string Text { get; set; }
    public string Size { get; set; }
    public string Weight { get; set; }
    public FactSet FactSet { get; set; }
}

public class FactSet
{
    public List<Fact> Facts { get; set; }
}

public class Fact
{
    public string Title { get; set; }
    public string Value { get; set; }
}