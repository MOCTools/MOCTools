namespace MOCTools.Data.Entities;

public sealed class RawEpgSnapshot
{
    public long Id { get; set; }

    public DateOnly EpgDate { get; set; }

    public DateTimeOffset FetchedAtUtc { get; set; }

    public required string Sha256 { get; set; }

    public required string Payload { get; set; }
}
