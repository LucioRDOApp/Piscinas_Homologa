(function () {
    'use strict';
    angular.module('app').controller('DefaultController', DefaultController);
    DefaultController.$inject = ['$http', '$location'];
    function DefaultController($http, $location) {
        var controller = this;
        this.filter = { "email": "", "senha": "" }
    }
})();
