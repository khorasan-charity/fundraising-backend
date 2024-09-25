using Mahak.Main.Samples;
using Xunit;

namespace Mahak.Main.EntityFrameworkCore.Domains;

[Collection(MainTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<MainEntityFrameworkCoreTestModule>
{

}
