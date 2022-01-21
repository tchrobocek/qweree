using Qweree.Mongo;

namespace Qweree.PiccStash.WebApi.Domain;

public class StashedPiccRepository : MongoRepositoryBase<StashedPicc>
{
    public StashedPiccRepository(MongoContext context) : base("stashed_piccs", context)
    {
    }
}