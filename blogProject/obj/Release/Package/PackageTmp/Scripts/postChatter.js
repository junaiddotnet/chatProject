var app = angular.module('post-app', []);

function postController($scope,$http) {
    
    $scope.hubconnection = null;
    $scope.hubconnection = $.connection.postHub;

   

    $scope.chatCall = function () {
        $.connection.hub.start().done(function () {
           
        });

        $.connection.hub.error(function (err) {
            alert(err);
        });

        //$http.get('/Chat/getUserId').success(function (data) {
        //    alert(data.userId);
        //    $scope.hubconnection.server.joinChatt();
        //});

        $http.get('/Chat/getId').success(function (data) {
            alert(222);
        }).error(function (error) {
            console.log(error);
        });

       

        // ---------- Get the Selected Post Chat of Current User and Admin ----------------------//

    };
} //----------- end of post controller
app.controller('postController', postController);

