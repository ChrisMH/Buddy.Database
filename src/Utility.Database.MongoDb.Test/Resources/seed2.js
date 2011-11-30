(function () {
  var roles = db.role.find({ roleName: { $ne: 'OptimusPrime'} }, { _id: 0, roleName: 1 }).map(function (role) { return { roleName: role.roleName }; });

  db.user.insert({
    'userName': 'chogan',
    'roles': roles
  });
} ());