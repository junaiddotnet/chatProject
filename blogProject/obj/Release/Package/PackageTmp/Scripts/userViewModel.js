var app = angular.module('userManage', []);
function manageUserController($scope,$http)
{
    $scope.newUser = {
        userId: 0,
        userName: null,
        password:null
    };
    $scope.userList = null;
    $scope.messageShow = null;
    $scope.temp = 222;
    $http.get('/Account/jsGetUser').success(function (data) {
        $scope.userList = data;
        
    });
    $scope.deleteUser=function (user,index)
    {
        $http.get('/Account/delUser', { params: { userId: user.userId } }).success(function (result)
        {
            $scope.userList.splice(index, 1);
            $scope.messageShow = user.userId + ' (' + user.userName + ') Deleted Successfully ...';

        });

    };
    $scope.addUser = function () {
        var add = angular.copy( $scope.newUser);
        $http.post('/Account/addNewUser', $scope.newUser).success(function (data) {
            if (data.userId==0)
            {
                $scope.messageShow = data.name;
            }
            else
            {
                $scope.messageShow = data.name;
                add.userId = data.userId;
                $scope.userList.push(add);
            }
        });
            
    };  // End of functions ...
}
app.controller('manageUserController', manageUserController);

