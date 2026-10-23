using Fgc.Catalog.Domain.Entities;
using MongoDB.Bson.Serialization;

namespace Fgc.Catalog.Infrastructure.Persistence.Mappings
{
    public static class GameReviewClassMap
    {
        public static void Register()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(GameReview)))
                return;

            BsonClassMap.RegisterClassMap<GameReview>(map =>
            {
                map.AutoMap();
                map.SetIdMember(map.GetMemberMap(r => r.Id).SetElementName("_id"));
                map.GetMemberMap(r => r.GameId).SetElementName("game_id");
                map.GetMemberMap(r => r.UserId).SetElementName("user_id");
                map.GetMemberMap(r => r.Rating).SetElementName("rating");
                map.GetMemberMap(r => r.Comment).SetElementName("comment");
                map.GetMemberMap(r => r.CreatedAt).SetElementName("created_at");
            });
        }
    }
}
