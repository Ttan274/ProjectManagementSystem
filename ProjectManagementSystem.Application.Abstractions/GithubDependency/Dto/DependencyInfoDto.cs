using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Application.Abstractions.GithubDependency.Dto;

public class DependencyInfoDto
{
    public string? Name { get; set; }
    public string? Version { get; set; }
    public string? PackageUrl { get; set; }
    public string? PackageManager { get; set; }
    public string? License { get; set; }

    public string GetLicenseDescription()
    {
        return License switch
        {
            "MIT" => "Permissive open-source license: free for personal and commercial use, modification, and distribution. No warranty or liability. No cost.",
            "Apache-2.0" => "Open-source license with patent protection: free for commercial use, modification, and distribution. Requires preservation of license and NOTICE file. No cost.",
            "GPL" => "Strong copyleft license: allows commercial use but requires derivative works to also be open-source under GPL. Cannot be used in proprietary software without open-sourcing. No cost.",
            "LGPL" => "Lesser GPL: suitable for libraries; allows linking in proprietary software, but modifications to the LGPL-covered parts must be open-sourced. Free for commercial use.",
            "BSD" => "Permissive license with minimal restrictions: free for commercial and non-commercial use. Requires attribution. No cost.",
            "ISC" => "Very permissive license similar to MIT: allows free use, modification, and distribution, including commercially. No cost.",
            _ => "License information not provided or not recognized."
        };
    }

    public string GetLicenseBadgeClass()
    {
        return License switch
        {
            "MIT" => "mit-badge",
            "Apache-2.0" => "apache-badge",
            "GPL" => "gpl-badge",
            "PostgreSQL" => "postgresql-badge",
            "BSD" => "bsd-badge",
            "ISC" => "isc-badge",
            "LGPL" => "lgpl-badge",
            _ => "unknown-badge"
        };
    }

    public string GetPackageIcon()
    {
        return PackageManager switch
        {
            "npm" => "<i class='fab fa-npm text-danger' style='font-size: 1.5rem'></i>",
            "nuget" => $"<img src=\"https://api.nuget.org/v3-flatcontainer/{Name?.ToLowerInvariant()}/{Version}/icon\" alt=\"\" aria-hidden=\"true\" class=\"w-100\" \">",
            "github" => "<i class='fab fa-github' style='font-size: 1.5rem'></i>",
            _ => "<i class='fas fa-cube text-secondary' style='font-size: 1.5rem'></i>"
        };
    }

    public string GetPackageUrl()
    {
        if (string.IsNullOrEmpty(PackageUrl)) return "#";

        if (PackageUrl.StartsWith("pkg:npm/"))
        {
            var package = PackageUrl.Replace("pkg:npm/", "").Split('@')[0];
            return $"https://www.npmjs.com/package/{package}";
        }
        else if (PackageUrl.StartsWith("pkg:nuget/"))
        {
            var package = PackageUrl.Replace("pkg:nuget/", "").Split('@')[0];
            return $"https://www.nuget.org/packages/{package}";
        }
        else if (PackageUrl.StartsWith("pkg:github/"))
        {
            var parts = PackageUrl.Replace("pkg:github/", "").Split('@');
            return $"https://github.com/{parts[0]}";
        }
        return "#";
    }
}


public class SbomPackage
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("versionInfo")]
    public string Version { get; set; }

    [JsonPropertyName("licenseConcluded")]
    public string LicenseConcluded { get; set; }

    [JsonPropertyName("externalRefs")]
    public List<SbomExternalRef> ExternalRefs { get; set; }
}
public class GitHubSbomResponse
{
    [JsonPropertyName("sbom")]
    public SbomData Sbom { get; set; }
}

public class SbomData
{
    [JsonPropertyName("SPDXID")]
    public string SpdxId { get; set; }

    [JsonPropertyName("packages")]
    public List<SbomPackage> Packages { get; set; }

    [JsonPropertyName("relationships")]
    public List<SbomRelationship> Relationships { get; set; }
}

public class SbomExternalRef
{
    [JsonPropertyName("referenceType")]
    public string ReferenceType { get; set; }

    [JsonPropertyName("referenceLocator")]
    public string ReferenceLocator { get; set; }
}

public class SbomRelationship
{
    [JsonPropertyName("spdxElementId")]
    public string SourceElementId { get; set; }

    [JsonPropertyName("relatedSpdxElement")]
    public string TargetElementId { get; set; }

    [JsonPropertyName("relationshipType")]
    public string RelationshipType { get; set; }
}


