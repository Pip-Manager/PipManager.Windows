using Newtonsoft.Json;

namespace PipManager.Models;

public class PypiPackageInfo
{
    [JsonProperty("releases")]
    public Dictionary<string, List<PypiPackageRelease>> Releases;
}

public class PypiPackageRelease
{
    [JsonProperty("filename")]
    public string Filename;
    [JsonProperty("upload_time")]
    public string UploadTime;
    [JsonProperty("digests")]
    public PypiPackageDigest Digests;
}

public class PypiPackageDigest
{
    [JsonProperty("blake2b_256")]
    public string Blake2B256;
    [JsonProperty("md5")]
    public string Md5;
    [JsonProperty("sha256")]
    public string Sha256;
}