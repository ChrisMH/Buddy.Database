(function() {
  db.createCollection('client');
  db.client.ensureIndex({ clientName: 1 }, { name: 'client_clientName', unique: true });

  db.createCollection('role');
  db.role.ensureIndex({ roleName: 1 }, { unique: true });

  db.createCollection('user');
  db.user.ensureIndex({ userName: 1 }, { name: 'user_userName', unique: true });
}());