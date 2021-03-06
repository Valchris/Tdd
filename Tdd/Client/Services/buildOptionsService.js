﻿app.service('buildOptionsService', ['gameDataService', function (gameDataService) {
    var promise = gameDataService.getTowerTypes().then(function (success) {
        towers = success.data;
        return success;
    });

    this.getAllTowers = function () {
        return promise;
    }

    this.getTower = function (id) {
        var tower = towers.filter(function (tower) {
            return tower.id === parseInt(id);
        });
        if (tower && tower.length > 0) {
            return tower[0];
        }
        return null;
    }
}]);