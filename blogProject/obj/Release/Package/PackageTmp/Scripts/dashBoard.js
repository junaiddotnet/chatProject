
var app = angular.module('chat-dashboard', []);

function dashBoard($scope, $window, $http) {

    // -------------- Scope Level Declaration ------------------
    $scope.user = 1;
    $scope.recentComments = [];
    $scope.recentChat = [];
    //----------------------------------------------------------
    $scope.hubconnection = null;
    $scope.hubconnection = $.connection.dashHub;

    $http.get('/Chat/getUserId').success(function (data) {
        //alert(data.userId);
        $scope.user = angular.copy(data);
    });

    $.connection.hub.start().done(function () {
       
        $scope.hubconnection.server.joinBoard();

    });

    $.connection.hub.error(function (err) {
        alert(err);
    });
    //------------------- Client fuction Invoked by Server --------------------
    $scope.refresh = function () {
        $window.location.reload();
    };
    $scope.hubconnection.client.loadRecentComments = function (comments) {
       
        $scope.recentComments = comments;
        $scope.$apply();

        //alert($scope.recentComments.length);
    };
    $scope.hubconnection.client.loadRecentChat = function (chatmessages) {
        //alert(chatmessages);
        $scope.recentChat = chatmessages;
        $scope.$apply();
    };
    $scope.hubconnection.client.displayMessage = function (message) {
        
        $scope.recentComments.splice(0, 0, message);
        //$scope.recentComments.push(message);
        $scope.$apply();
    };
    $scope.hubconnection.client.displayChatMessage = function (cmessage) {
        $scope.recentChat.splice(0, 0, cmessage);
        $scope.$apply();
    };
    //------------------------ Client Side function .............................
    $scope.chatWindow = function (name) {
        alert(name);
    };

} /// End of Controller


app.controller('dashBoard', dashBoard);
