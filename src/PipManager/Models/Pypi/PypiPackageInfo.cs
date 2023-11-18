using Newtonsoft.Json;

namespace PipManager.Models.Pypi;

public class PypiPackageInfo
{
    [JsonProperty("releases")]
    public required Dictionary<string, List<PypiPackageRelease>> Releases;
}

public class PypiPackageRelease
{
    [JsonProperty("filename")]
    public required string Filename;

    [JsonProperty("upload_time")]
    public required string UploadTime;

    [JsonProperty("digests")]
    public required PypiPackageDigest Digests;
}

public class PypiPackageDigest
{
    [JsonProperty("blake2b_256")]
    public required string Blake2B256;

    [JsonProperty("md5")]
    public required string Md5;

    [JsonProperty("sha256")]
    public required string Sha256;
}