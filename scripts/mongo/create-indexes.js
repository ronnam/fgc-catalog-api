const db = db.getSiblingDB('fgc_catalog');
const collection = db.getCollection('game_reviews');

// Índice de listagem: avaliações de um jogo, mais recentes primeiro
collection.createIndex(
    { game_id: 1, created_at: -1 },
    { name: 'idx_game_id_created_at', background: true }
);

// Garante 1 avaliação por usuário por jogo
collection.createIndex(
    { game_id: 1, user_id: 1 },
    { name: 'ux_game_user', unique: true }
);

print("Indices em game_reviews:");
printjson(collection.getIndexes());