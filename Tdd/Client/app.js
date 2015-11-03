var app = angular.module('tdd', ['ngRoute', 'ngCookies']);

app.config(['$routeProvider', '$locationProvider',
  function($routeProvider, $locationProvider) {
    $locationProvider.html5Mode(true);

    $routeProvider.when("/login", {
        controller: "loginController",
        templateUrl: "/Partial/Account/Login.html"
    }).when("/register", {
        controller: "registerController",
        templateUrl: "/Partial/Account/Register.html"
    }).when('/', {
        templateUrl: '/Partial/Home.html',
        controller: 'HomeController'
    });
}]);

app.run(['authService', function (authService) {
    authService.fillAuthData();
}])

app.config(['$httpProvider', function($httpProvider) {
    $httpProvider.interceptors.push('httpRequestInterceptor');
    $httpProvider.interceptors.push('authInterceptor');
}]);