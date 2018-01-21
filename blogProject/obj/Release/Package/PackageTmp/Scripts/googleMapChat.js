
var app = angular.module('mapsApp', []);

function map($scope, $http, $compile, $timeout) {
    var cities = [
    {
        city: 'Toronto',
        desc: 'This is the best city in the world!',
        lat: 43.7000,
        long: -79.4000
    },
    {
        city: 'New York',
        desc: 'This city is aiiiiite!',
        lat: 40.6700,
        long: -73.9400
    },
    {
        city: 'Chicago',
        desc: 'This is the second best city in the world!',
        lat: 41.8819,
        long: -87.6278
    },
    {
        city: 'Los Angeles',
        desc: 'This city is live!',
        lat: 34.0500,
        long: -118.2500
    },
    {
        city: 'Las Vegas',
        desc: 'Sin City...\'nuff said!',
        lat: 36.0800,
        long: -115.1522
    }
    ];


    var mapOptions = {

        zoom: 4,
        center: new google.maps.LatLng(40.0000, -98.0000),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }

    //--------------------------- Scope Leavel Declation of Varibles ---------------------
    $scope.user = [{ userId: 0, name: 'default',status:0, lat: 0, lng: 0 }];
    $scope.selectedUser = [{ userId: 0, name: 'default',status: 0, lat: 0, lng: 0 }];
    $scope.userStatus = null;
    $scope.users = [{ userId: 1, name: 'Admin', status: 0, lat: 0, lng: 0 }];
    $scope.selectUserMessages = [];
    var userMarker = [{lat:0,long:0,userId:0,name:'default',status:0}];

    //-------------------------------------------------------------------------------------
    
    
    // --------------------------------------------- Hub Related varibles List 
    $scope.hubconnection = null;
    $scope.hubconnection = $.connection.mapHub;
    
    //-------------------------------------------------------------------------
    // ------------------------------------- Map Related Varible List 
    $scope.map = new google.maps.Map(document.getElementById('map'), mapOptions);
    $scope.markers = [];
    var infoWindow = new google.maps.InfoWindow();
    // -------- This statment is html5 statment wich fetch the current location of user by obtaining the Lat and Lng --------------
    //navigator.geolocation.getCurrentPosition(function (position) {
    //    var pos = {
    //        lat: position.coords.latitude,
    //        lng: position.coords.longitude
    //    };
    //    $scope.user.lat = pos.lat;
    //    $scope.user.lng = pos.lng;
    //    //alert(pos.lat);
    //   // alert(pos.lng);
    //});
    //------------------------------------
    // Create Marker function
    var createMarker = function (info) {
        var icon = null;
        if (info.status!=null && $scope.user.userId !=info.userId)
        {
           
            icon = 'http://maps.google.com/mapfiles/ms/icons/green-dot.png';
        }
        else if ($scope.user.userId != info.userId)
        {
            
            icon = 'http://maps.google.com/mapfiles/ms/icons/red-dot.png';

        }
        else if ($scope.user.userId == info.userId) {

            icon = 'http://maps.google.com/mapfiles/ms/icons/yellow-dot.png';

        }
        var marker = new google.maps.Marker({
            map: $scope.map,
            icon: icon,
            position: new google.maps.LatLng(info.lat, info.long),
            title: info.name
        });
        if ($scope.user.userId == info.userId)
        {
            var content = '<button  ng-click="selectUser(' + info.userId + ')" class="btn btn-primary btn-lg" disabled>' + marker.title + '</button>';

        }
        else
        {
            var content = '<button ng-click="selectUser(' + info.userId + ')" class="btn btn-primary btn-lg" data-toggle="modal" data-target="#myModal">' + marker.title + '</button>';

        }
           var compiledContent = $compile(content)($scope);
           marker.content = compiledContent[0];

           google.maps.event.addListener(marker, 'click', function () {
            infoWindow.setContent(marker.content);
            infoWindow.open($scope.map, marker);


        });

        $scope.markers.push(marker);

    } // end of createMarker function which use to create marker
    //------------------------------- End of Map Related -------------------------------------------------------

    // ----------- Pull the current user which is signed ------------------
    $http.get('/Chat/getUserId').success(function (data) {
        $scope.user = angular.copy(data);
     
        ///alert($scope.user.userId);
    });
    //------------------- Hub Star Up and connecion with the Google chat Hub ------------
    $.connection.hub.start().done(function () {
      $scope.hubconnection.server.joinChatt($scope.user);

    });
    //----------------------------------------------------------------------------------
    //---------------------- Hub Client Function invoked by server --------------------------------------
    $scope.hubconnection.client.loadMembers = function (userLis) {
        
        $scope.users = userLis;
        for (i = 0; i < userLis.length; i++) {
            if ($scope.user.userId == userLis[i].userId)
            {
                $scope.user.lat=userLis[i].lat;
                $scope.user.lng = userLis[i].lng;
            }
           
            userMarker.lat = userLis[i].lat;
            userMarker.long = userLis[i].lng;
            userMarker.name = userLis[i].name;
            userMarker.userId = userLis[i].userId;
            userMarker.status = userLis[i].status;
            
            createMarker(userMarker);
        }
    
        $scope.$apply();

    };
    $scope.hubconnection.client.loadUserChat = function (chatMessages) {
        
        $scope.selectUserMessages = chatMessages;
        $scope.$apply();
    };
    //------- Show message to one to one chat window -------------
    $scope.hubconnection.client.displayUserMessage = function (message) {

        if (($scope.user.userId == message.uid) || (message.receiverId == $scope.user.userId && message.uid == $scope.selectedUser.userId)) // This statment making sure message delivered to right user screen by checking the sender and recevier id of users

        {
            $scope.selectUserMessages.push(message);
            
            //------------ The below code is to adjust the scroll of <UL> <li> to last message texted
            $timeout(function () {
                var scroller = document.getElementById("autoscroll");
                scroller.scrollTop = scroller.scrollHeight;
            }, 0, false);
            
            $scope.$apply();
            //---------------------
        }
       
    };
    //-------------- Client function to set the status of user to online ---------------
    $scope.hubconnection.client.setOnline = function (onlineUser) {
       // alert(onlineUser.userId);
        var singleUser = onlineUser;
        for (var i = 0; i < $scope.users.length; i++) // - This loop iterate through the users and set the use online which just joined the chat group
        {
            

            if ($scope.users[i].userId == singleUser.userId) {

                $scope.users[i] = angular.copy(onlineUser); // This statment set the current user status online ..
                $scope.changeMarkeronline(i);
            }
        }
        
        // The below display the message to chat window if some user is just join chat . like a greeting message..
        //$scope.singleMessage.uid = onlineUser.userId;
        //$scope.singleMessage.name = onlineUser.name;
        //$scope.singleMessage.msg = 'Just Signed Inn to Chat Room: Welcome ??';
        //var signedInnValue = angular.copy($scope.singleMessage);
        //$scope.messages.push(signedInnValue);
        $scope.$apply();
    };
    // -------------------------- Set it to Online -------------------------------
    $scope.hubconnection.client.setOfline = function (oflineUser) {
        var singleUser = oflineUser;

        for (var i = 0; i < $scope.users.length; i++) // - This loop iterate through the users and set the use online which just joined the chat group
        {
            if ($scope.users[i].userId == singleUser.userId) {

                $scope.users[i].status = null;
                $scope.changeMarkeroffline(i);
            }
        }

        //$scope.singleMessage.uid = oflineUser.userId;
        //$scope.singleMessage.name = oflineUser.name;
        //$scope.singleMessage.msg = 'Just Signed Out from Chat Room: Good Bye ??';
        //var signedOfValue = angular.copy($scope.singleMessage);
        //$scope.messages.push(signedOfValue);
        $scope.$apply();
    };
 //----------------------------------------------------------------------------------------
 // ------------------------ Client Page events -------------------------------------------
    $scope.messageUser = function (selectedUser) {
        
        google.maps.event.trigger($scope.markers[selectedUser], 'click');
    };

    $scope.selectUser = function (id) {
        
        for (i = 0; i < $scope.users.length; i++)
        {
            if (id==$scope.users[i].userId)
            {
                $scope.selectedUser = $scope.users[i];
            }
        }
        if ($scope.selectedUser.status == null)
        {
            $scope.userStatus = 'OFFLINE';
        }
        else
        {
            $scope.userStatus = 'ONLINE';
        }
        var flightPlanCoordinates = [
          { lat: $scope.user.lat, lng:$scope.user.lng },
          { lat: $scope.selectedUser.lat, lng:$scope.selectedUser.lng }
          
        ];
       

       
        var flightPath = new google.maps.Polyline({
            path: flightPlanCoordinates,
            geodesic: true,
            strokeColor: '#FF0000',
            strokeOpacity: 1.0,
            strokeWeight: 2
        });
        flightPath.setMap($scope.map);
        alert(298982);
        $scope.hubconnection.server.retriveUserChat($scope.selectedUser, $scope.user); // This method on server side sent request to retrive the chat of selected user and current online user


    };
    $scope.sendMessage = function () {
        
        $scope.hubconnection.server.sendMessage($scope.message, $scope.user, $scope.selectedUser);
    };
    //-------------------------------------------------------------------------------------
    $scope.removeMarker = function () {
        alert(33);
        $scope.markers[0].setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');
       // $scope.markers[0].setMap(null);
    };
    $scope.changeMarkeronline = function (index) {
        $scope.markers[index].setIcon('http://maps.google.com/mapfiles/ms/icons/green-dot.png');

    };
    $scope.changeMarkeroffline = function (index) {
        $scope.markers[index].setIcon('http://maps.google.com/mapfiles/ms/icons/red-dot.png');

    };
    $scope.opnInfoWindow = function (e, selectedMarker) {
        //e.preventDefault();
        google.maps.event.trigger(selectedMarker, 'click');
    }
    $scope.selectTab = function(i)
    {
        $scope.users.splice(1, 1);
    }
    
} // controll ending

app.controller('map', map);

// This function when a user is selected from list
