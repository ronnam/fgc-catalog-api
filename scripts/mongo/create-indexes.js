const db = db.getSiblingDB('fgc_catalog');
const collection = db.getCollection('game_reviews');

collection.createIndex(
    { game_id: 1, created_at: -1 },
    { name: 'idx_game_id_created_at', background: true }
);

print("Indices em game_reviews:");
printjson(collection.getIndexes());