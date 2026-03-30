using Facet;
using SSAReplacement.Api.Domain;

namespace SSAReplacement.Api.Features.Admin.Domain;

[Facet(typeof(User), exclude: [nameof(User.AuditEntries)])]
public partial record UserDto;

[Facet(typeof(AuditEntry), NestedFacets = [typeof(UserDto)])]
public partial record AuditEntryDto;

[Facet(typeof(User), NestedFacets = [typeof(AuditEntryDto)])]
public partial record UserDetailDto;
