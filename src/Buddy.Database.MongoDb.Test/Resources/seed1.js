(function() {

  db.role.insert({ 'roleName': 'OptimusPrime' });
  db.role.insert({ 'roleName': 'Administrator' });
  db.role.insert({ 'roleName': 'SuperUser' });
  db.role.insert({ 'roleName': 'Installer' });
  db.role.insert({ 'roleName': 'User' });

  var roles = db.role.find({ }, { _id: 0, roleName: 1 }).map(function(role) { return { roleName: role.roleName }; });

  db.user.insert({
      'userName': 'logikos',
      'roles': roles
    });
  } ());