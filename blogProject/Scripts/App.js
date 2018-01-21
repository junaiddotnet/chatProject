
var app = angular.module('chat-app', ['angular.filter']);
var edit = angular.module("textAngularTest", ['textAngular', 'ngRoute']);
// app cinfiguration setting
//app.config(['$tooltipProvider', function($tooltipProvider){
//    $tooltipProvider.setTriggers({'customEvent': 'customEvent'});
//}]);
//------------- Route configuration
edit.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.
    when('/Edit',
        {
            controller: 'wysiwygeditor',
            templateUrl: 'EditPost.cshtml'
        })
     .when('/admin',
        {
            controller: 'postItemsController',
            templateUrl: 'admin/show'
        })
        .when('/routeTwo/:postid',
        {
            templateUrl: function (params) { return 'admin/EditPost?postid=' + params.postid; }
        })
        .when('/routeTwo',
        {
            templateUrl: 'admin/EditPost'
        })
    .otherwise({
        redirectTo: '/admin'
    });

}]);
//--------------------------

function chatController($scope, $window, $http, $timeout, $filter, FileUploadService, focus) {
    $scope.inputMessageStatus = false;
    $scope.imagetext = "";
    //var userId;
    //var selectedGroup;
    $scope.user = { userId: null, name: null };
    $scope.register = { userName: null, password: null,confirmpassword:null,message:null };
    $scope.users = [{ userId: 1, name: 'Admin', groupId: 1, status: 0, unread: 0,seen:false, country: 'GB' }];
    $scope.SelectChatUsers = [];

    $scope.singleUser = { userId: 1, name: 'Admin', group: 1, status: 0 };
    $scope.activeItem = 1;
    $scope.activeTab = null;
    $scope.selectUser = null;//{ userId: 0, name: '0', group: 0, status: 0 };
    $scope.selectComment = null;
    $scope.messageWindow = false;
    $scope.tabsOptions = [{ id: 1, option: 'General', messagecount: 0, unread: 0 }, { id: 2, option: 'C#', messagecount: 0, unread: 0 }, { id: 3, option: 'SignalR', messagecount: 0, unread: 0 }, { id: 4, option: 'Entity Framework', messagecount: 0, unread: 0 },
        { id: 5, option: 'Anjular Js', messagecount: 0, unread: 0 }, { id: 6, option: 'J Query', messagecount: 0, unread: 0 }
    ];
    $scope.tabPageCount = 1;
    $scope.singleMessage = { groupChatId: 0, uid: 0, name: 'New', msg: 'Just Signed Inn to Chat Room: Welcome ??', group: 0, time: 0, receiverId: 0 };
    $scope.messages = [];
    $scope.selectUserMessages = [];
    //--------- File upload variables
    // Variables
    $scope.uploadme = "/Images/me.jpg";
    $scope.Message = "";
    $scope.FileInvalidMessage = "";
    $scope.SelectedFileForUpload = null;
    $scope.FileDescription = "";
    $scope.IsFormSubmitted = false;
    $scope.IsFileValid = false;
    $scope.IsFormValid = true;
    //-------------------------- Varibles used for google Map ---------------------------
    var mapOptions = {

        zoom: 4,
        center: new google.maps.LatLng(40.0000, -98.0000),
        mapTypeId: google.maps.MapTypeId.ROADMAP
    }
    //------------------------------------- varibles to control appreance of window -------------------------------
    $scope.chatBoxMain = "col-md-9 col-lg-9 col-sm-12";
    $scope.FileListToggle = false;
    $scope.MemberList = false;
    //--------------------------------------------------------------------------------------------------------------
    $scope.gimage = "nofile";
    //$scope.map = new google.maps.Map(document.getElementById('map'), mapOptions);

    //---------------------------------------------
    //----------------------------
    //$scope.messages = [
    //    { uid: 1,name:"Admin", msg: 'junaid mahmood',time:null },
    //                   { uid: 1,name:"Admin", msg: 'junaid mahmood', group: null, time: null },
    //                   { uid: 1,name:"Admin", msg: 'junaid mahmood', group: null, time: null },
    //                   { uid: 1,name:"Admin", msg: 'junaid mahmood', group: null, time: null },
    //];
    $scope.refresh = function () {
        $window.location.reload();
    };
    $scope.hubconnection = null;
    $scope.hubconnection = $.connection.planeHub;

    // $window.location.reload();
    $http.get('/Chat/getUserId').success(function (data) {
        // alert(data.userId);
        $scope.user = angular.copy(data);
    });

    $.connection.hub.start().done(function () {
        $http.get('/Chat/getUserId').success(function (data) {
            //alert(data.userId);
            // $scope.user = angular.copy(data);
            //alert($scope.user);


            $scope.hubconnection.server.joinChatt($scope.user);



        });


        $scope.postMessage = function () {
            $scope.hubconnection.server.broadcastMessage($scope.user, $scope.message, $scope.activeItem);
            $scope.message = null;
            $scope.inputMessageStatus = true;
        };

    });

    $.connection.hub.error(function (err) {
        alert(err);
    });
    //----------------------------- function of page to uplaod the image
    //the image


    // THIS IS REQUIRED AS File Control is not supported 2 way binding features of Angular
    // ------------------------------------------------------------------------------------
    //File Validation
    $scope.ChechFileValid = function (file) {
        var isValid = false;
        if ($scope.SelectedFileForUpload != null) {
            if ((file.type == 'image/png' || file.type == 'image/jpeg' || file.type == 'image/gif') && file.size <= (512 * 1024)) {
                $scope.FileInvalidMessage = "";
                isValid = true;
            }
            else {
                $scope.FileInvalidMessage = "Selected file is Invalid. (only file type png, jpeg and gif and 512 kb size allowed)";
            }
        }
        else {
            $scope.FileInvalidMessage = "Image required!";
        }
        // alert($scope.IsFileValid);
        $scope.IsFileValid = isValid;
    };

    //File Select event 
    $scope.selectFileforUpload = function (file) {
        $scope.SelectedFileForUpload = file[0];
    }
    //----------------------------------------------------------------------------------------

    //Save File
    $scope.SaveFile = function () {
        $scope.IsFormSubmitted = true;
        $scope.Message = "";
        $scope.ChechFileValid($scope.SelectedFileForUpload);
        if ($scope.IsFormValid && $scope.IsFileValid) {
            FileUploadService.UploadFile($scope.SelectedFileForUpload, $scope.FileDescription).then(function (d) {
                //alert(d.Message);
                angular.element('#myModal').modal('hide');
                // Sending image message to hub to distrubute to all the connected clients 
                $scope.hubconnection.server.broadcastImageMessage($scope.user, d.Message, $scope.activeItem);

                //ClearForm();
            }, function (e) {
                alert(e);
            });
        }
        else {
            $scope.Message = "All the fields are required.";
        }
    };
    //Clear form 
    function ClearForm() {
        $scope.FileDescription = "";
        //as 2 way binding not support for File input Type so we have to clear in this way
        //you can select based on your requirement
        angular.forEach(angular.element("input[type='file']"), function (inputElem) {
            angular.element(inputElem).val(null);
        });

        $scope.f1.$setPristine();
        $scope.IsFormSubmitted = false;
    }







    //-----------------------------------------------
    //-------------------------------------- Inner function called by page or invoked by page 
    //----------------------- Map Location function and methods
    $scope.myCurrentLocation = function () {
        // This method of geolocation will not work on azure until we purchase SSL Certificate
        //https://docs.microsoft.com/en-us/azure/app-service-web/web-sites-purchase-ssl-web-site
        //var basicLink = "https://maps.googleapis.com/maps/api/staticmap?";
        var locationLink = "";
        var cords = "center=";
        //----------------------------------- HTMl5 way of getting the current Position  --------------------------------
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(function (position) {
                var c = position.coords;
                //  center=40.714728,-73.998672
                //alert(c.latitude);

                //  basicLink.concat( + "&zoom=12&size=400x400&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyBEYkKE75U_1F_jGA6TgvAQ4Stv-5K0tF8";
                // var newlink = basicLink.concat("center=", c.latitude, ",", c.longitude, "&", "zoom=12&size=400x400", "&", "markers=color:red%7Clabel:C%7C", c.latitude, ",", c.longitude, "&key=", "AIzaSyBEYkKE75U_1F_jGA6TgvAQ4Stv-5K0tF8");
                // $scope.gimage = "https://maps.googleapis.com/maps/api/staticmap?center=40.714728,-73.998672&zoom=12&size=400x400&markers=color:red%7Clabel:C%7C40.718217,-73.998284&key=AIzaSyBEYkKE75U_1F_jGA6TgvAQ4Stv-5K0tF8";
                // $scope.gimage = newlink;
                var newlink = locationLink.concat(c.latitude, ",", c.longitude)
                //alert($scope.user);
                //alert(newlink);
                //alert($scope.activeItem);
                $scope.hubconnection.server.broadcastLocationMessage($scope.user, newlink, $scope.activeItem);

            });
        } else {
            x.innerHTML = "Geolocation is not supported by this browser.";
        }
    }
    $scope.getStaticMapurl = function (cords) {
        var basicLink = "https://maps.googleapis.com/maps/api/staticmap?";
        var newlink = basicLink.concat("center=", cords, "&", "zoom=12&size=400x400", "&", "markers=color:red%7Clabel:C%7C", cords, "&key=", "AIzaSyBEYkKE75U_1F_jGA6TgvAQ4Stv-5K0tF8");
        return newlink;
    }
    $scope.getDynamicMapUrl = function (cords) {
        var basicLink = "https://www.google.com/maps?";
        var newlink = basicLink.concat("daddr=", cords);
        return newlink;
    }
    //--------------------------------------------------------
    $scope.formateDate = function (dateValue) {
        var d = $filter('date')(dateValue, 'yyyy-MM-dd');
        alert(d);
        return dateValue;
    }
    $scope.closeChatUser = function (ind) {
        $scope.SelectChatUsers.splice(ind, 1);
        if (ind == 1 && $scope.SelectChatUsers.length == 1) {
            alert(ind);
            ind = ind - 1;
        }


        if ($scope.SelectChatUsers.length != 0) {

            $scope.messageUser($scope.SelectChatUsers[ind]);

        }
        else if ($scope.SelectChatUsers.length == 0) {
            $scope.returnToMain();
        }


    };
    $scope.tabSelect = function (id) {
        $scope.messages = null;
        $scope.activeItem = id.id;
        $scope.activeTab = id;
       // $scope.messages = null;
        $scope.tabPageCount = 1;
        $scope.hubconnection.server.join($scope.activeItem, $scope.user);
        if ($scope.FileListToggle == true) {
            FileDisplayList();
        }

    };
    //---------------- Inner Page Text Box Enter Event to  ----------------
    $scope.getEnter = function (keyEvent) {
        if (keyEvent.which === 13 && keyEvent.shiftKey==false) {
            
            if ($scope.message.length > 0) {
                $scope.postMessage();

            }
        }
        else if (keyEvent.which === 13 && keyEvent.shiftKey == true)
        {
            
            ///-------- This below code is just to adjus the scroll bar to an end 
            $timeout(function () {
                var scroller = document.getElementById("autoscrlone");
                // alert(scroller.scrollTop);
                // alert(scroller.scrollHeight);
                scroller.scrollTop = scroller.scrollHeight;
            }, 0, false);
            //-------------------

            
        }
    };

    $scope.getEnterUserChat=function(keyEvent)
    {
        if (keyEvent.which === 13 && $scope.message.length > 0)
        {
            $scope.sendMessage();

        }
    }
    $scope.navselectUser = function (id) {

        $scope.selectUser = id;
        $scope.selectComment = id;
    };
    $scope.CommentPick = function (id) {
        $scope.selectComment = id;
        $scope.selectUser = id;
    };
    // This function when a user is selected from list
    $scope.messageUser = function (id) {
        var flag = 0;
        
        
        //----------------------------------------
        if ($scope.SelectChatUsers != null)
        {
            for (var i = 0 ; i < $scope.SelectChatUsers.length; i++) {
                if (id.userId == $scope.SelectChatUsers[i].userId) {
                    $scope.SelectChatUsers[i].seen = true;
                    flag = 1;

                }
                if ($scope.SelectChatUsers[i].userId == $scope.user.userId) {
                    $scope.SelectChatUsers.splice(0, 1);
                }


            }
        }
      
        if (flag == 0) {
           // id.unread = 13;
            var copyArray = angular.copy(id);
            
            $scope.SelectChatUsers.push(copyArray);
           // alert($scope.SelectChatUsers[1].unread);
            
        }
        //-----------------------------------------
        $scope.selectUser = id;

        $scope.messageWindow = true;
        $scope.hubconnection.server.retriveUserChat($scope.selectUser, $scope.user);
        focus("focusMeUser");

    };
    $scope.sendMessage = function () {
        $scope.hubconnection.server.sendMessageTo($scope.message, $scope.user, $scope.selectUser);
        $scope.message = null;
        focus('focusMeUser');


    };
    $scope.returnToMain = function () {
        $scope.selectUser = null;
        $scope.messageWindow = false;
        $scope.selectUserMessages = null;
    };
    //----------------------------------------- Client Function invoked by Server ---------------------------------------------------
    $scope.hubconnection.client.ping = function () {

        $scope.hubconnection.server.pingback($scope.user.userId);
    };
    $scope.hubconnection.client.displayMessage = function (message) {
        $scope.inputMessageStatus = false;

        focus('focusMe');
        var relevantTabSlot = 0;

        $scope.hubconnection.state.current = 333; //Temporary Checking only 
        // -------------------- Get the Tab from Top List whoes Messages Is broadCasted and incremnet its message Count
        for (var tabCount = 0; tabCount < $scope.tabsOptions.length; tabCount++) {
            if ($scope.tabsOptions[tabCount].id == message.group) {

                $scope.tabsOptions[tabCount].messagecount++;
                relevantTabSlot = tabCount;
            }

        }
        //----------------------------------------------------------------------------------------------------

        if (message.group == $scope.activeItem) {
            $scope.messages.push(message);


        }
        else {

            $scope.tabsOptions[relevantTabSlot].unread = 1;
        }
        ///-------- This below code is just to adjus the scroll bar to an end 
        $timeout(function () {
            var scroller = document.getElementById("autoscrlone");
            // alert(scroller.scrollTop);
            // alert(scroller.scrollHeight);
            scroller.scrollTop = scroller.scrollHeight;
        }, 0, false);
        //-------------------

        $scope.$apply();

    };
   
    $scope.hubconnection.client.setOnline = function (onlineUser) {

        $scope.singleUser = onlineUser;
        for (var i = 0; i < $scope.users.length; i++) // - This loop iterate through the users and set the use online which just joined the chat group
        {
            if ($scope.users[i].userId == $scope.singleUser.userId) {

                // $scope.users[i] = angular.copy(onlineUser);
                $scope.users[i].status = onlineUser.status;

            }
        }
        // The below display the message to chat window if some user is just join chat . like a greeting message..
        $scope.singleMessage.uid = onlineUser.userId;
        $scope.singleMessage.name = onlineUser.name;
        $scope.singleMessage.msg = 'Just Signed Inn to Chat Room: Welcome ??';
        $scope.singleMessage.time = new Date();
        var signedInnValue = angular.copy($scope.singleMessage);
        $scope.messages.push(signedInnValue);
        $scope.$apply();
    };

    $scope.hubconnection.client.setOfline = function (oflineUser) {
        $scope.singleUser = oflineUser;

        for (var i = 0; i < $scope.users.length; i++) // - This loop iterate through the users and set the use online which just joined the chat group
        {
            if ($scope.users[i].userId == $scope.singleUser.userId) {

                $scope.users[i].status = null;

            }
        }
        $scope.singleMessage.uid = oflineUser.userId;
        $scope.singleMessage.name = oflineUser.name;
        $scope.singleMessage.msg = 'Just Signed Out from Chat Room: Good Bye ??';
        $scope.singleMessage.time = new Date();
        var signedOfValue = angular.copy($scope.singleMessage);
        $scope.messages.push(signedOfValue);
        $scope.$apply();
    };

    $scope.hubconnection.client.displayUserMessage = function (message) {
        var flagMessageCount = 0;
        for (var count = 0; count < $scope.users.length; count++) {
            //if ($scope.users[count].userId==message.receiverId || $scope.users[count].userId==message.uid )
            //{
            //    // Now incremnet the number of messages count Badge in The Members List Panel

            //    $scope.users[count].unread++;
            //}
            if ($scope.users[count].userId == message.uid && $scope.userId != message.uid) {

                //alert(count);
                
                $scope.users[count].unread++;
                flagMessageCount = $scope.users[count].unread;
            }
            $scope.$apply();
        }
       
        if (($scope.user.userId == message.uid) || (message.receiverId == $scope.user.userId && message.uid == $scope.selectUser.userId)) // This statment making sure message delivered to right user screen by checking the sender and recevier id of users

        {
            
            $scope.selectUserMessages.push(message);
            //------------ The below code is to adjust the scroll of <UL> <li> to last message texted
            $timeout(function () {
                var scroller = document.getElementById("autoscroll");
                scroller.scrollTop = scroller.scrollHeight;
            }, 0, false);
            
            //---------------------
        }
        else
        {
            for (var i =0;i < $scope.SelectChatUsers.length;i++)
            {
                if (message.uid==$scope.SelectChatUsers[i].userId && message.receiverId==$scope.user.userId && message.uid!=$scope.selectUser.userId)
                {
                    
                   // alert($scope.SelectChatUsers.length);
                    $scope.SelectChatUsers[i].seen = false;
                }
            }
        }

        $scope.$apply();
    };
    $scope.hubconnection.client.loadMessages = function (messageList) {
       // $scope.messages = null;
        $scope.messages = messageList;
        
        var list = null;
        //for (var i = 0; i < $scope.messages.length; i++)
        //{
        //    alert($scope.messages[i].txtStatus);
        //}
        //alert(list);
        // -------------------- Get the Tab from Top List whoes Messages Is broadCasted and incremnet its message Count
        for (var tabCount = 0; tabCount < $scope.tabsOptions.length; tabCount++) {
            if ($scope.tabsOptions[tabCount].id == $scope.activeItem) {

                $scope.tabsOptions[tabCount].unread = 0;
            }

        }
        //----------------------------------------------------------------------------------------------------
        ///-------- This below code is just to adjus the scroll bar to an end 
        $timeout(function () {
            var scroller = document.getElementById("autoscrlone");
            scroller.scrollTop = scroller.scrollHeight;

        }, 0, false);
        //-------------------
        $scope.inputMessageStatus = false;
        focus('focusMe');

        $scope.$apply();
    };

    $scope.hubconnection.client.loadUserChat = function (chatMessages) {
        $scope.selectUserMessages = chatMessages;
       // This littile piece of code is to adjust the scroll bar in to the bottom
        $timeout(function () {
            var scroller = document.getElementById("autoscroll");
            scroller.scrollTop = scroller.scrollHeight;
        }, 0, false);
        //---------------------

        $scope.$apply();
    };
    $scope.hubconnection.client.loadMembers = function (userLis) {

        $scope.users = userLis;
        $scope.$apply();
    };
    $scope.hubconnection.client.loadTabOptions = function (options) {
        $scope.tabsOptions = options;
        $scope.$apply();
    };
    $scope.testing = function () {
        $scope.imagetext = "aa.jpg";
    }
    //------------------------------ Page function to control the Layout of Page and interface ----------------------------
    $scope.getChatWindowLength = function () {
        //var d = "col-lg-11 col-md-11 col-sm-11";

        if ($scope.user.userId != null) {
            return $scope.chatBoxMain;
        }
        else {
            return "col-md-11 col-lg-11 col-sm-11";
        }
        return d;
    }
    $scope.Info = function () {
        $scope.chatBoxMain = "col-md-7 col-lg-7 col-sm-12";

        $scope.FileListToggle = true;
        $scope.MemberList = false;

        FileDisplayList();
    }
    $scope.UserList = function () {
        $scope.chatBoxMain = "col-md-7 col-lg-7 col-sm-12";
        $scope.MemberList = true;
        $scope.FileListToggle = false;

    }
    var FileDisplayList = function () {
        $http.get('/Chat/getFiles', { params: { groupId: $scope.activeItem } }).success(function (data) {
            $scope.FileList = data;
            //$scope.user = angular.copy(data);
        });
    }
    $scope.CloseFileModal = function () {
        $scope.chatBoxMain = "col-md-9 col-lg-9 col-sm-12";
        $scope.FileListToggle = false;
    }
    $scope.CloseMemberModal = function () {
        alert(33);
        $scope.chatBoxMain = "col-md-9 col-lg-9 col-sm-12";
        $scope.MemberList = false;

    }
    //--------------------------------------------
    ///---------------------------- Function to control the scrool effect of chat window
    $scope.images = [1, 2, 3, 4, 5, 6, 7, 8];

    $scope.loadMore = function () {
        var last = $scope.images[$scope.images.length - 1];
        for (var i = 1; i <= 8; i++) {
            $scope.images.push(last + i);
        }
    };
    $scope.showMore = function () {
        // Loading the spinner
        var load = document.getElementById("loadmore");
       
        $timeout(function () {
             load.style.display = 'block';
        load.className = 'loader';
        },5000);
        console.log('show more triggered');
        console.log($scope.tabPageCount);
        console.log($scope.activeTab.messagecount);
        if (($scope.tabPageCount * 30) < $scope.activeTab.messagecount) {
            $http.get('/Chat/getChat', { params: { groupId: $scope.activeItem, pageCount: $scope.tabPageCount } }).success(function (data) {

                $scope.messages = data;


            });
            $scope.tabPageCount = $scope.tabPageCount + 1;
            ///-------- This below code is just to adjus the scroll bar to an end 
            $timeout(function () {
                var scroller = document.getElementById("autoscrlone");
                scroller.scrollTop = scroller.scrollTop + 200;

            }, 0, false);
            //-------------------
            // $scope.$apply();
            // Hide the Loading Spinner ....
           

        }
        else {
            // alert("Messages Over ...........")
        }
        load.className = null;
        load.style.display = 'none';



    };
    //--------------------------------------------------------------------------------
    $scope.ImageModel = function (imgId) {
        var modal = document.getElementById('myImageModel');
        
        // Get the image and insert it inside the modal - use its "alt" text as a caption
        modal.style.display = "block";

        var img = document.getElementById(imgId);
        var modalImg = document.getElementById("img01");
        var captionText = document.getElementById("caption");
        //alert(img.src);
        modalImg.src = img.src;
        captionText.innerHTML = img.alt;

    }
    $scope.closeImageModel = function () {
        var modal = document.getElementById('myImageModel');

        modal.style.display = "none";
    };
    $scope.closeLoginModel = function () {
        var modal = document.getElementById("myLoginModel");
        modal.style.display = "none";
    };
    $scope.ImageButton=function(id)
    {
        
        var className = document.getElementById(id + "Img").className;
        //alert(className);
        if (className == "glyphicon glyphicon-arrow-down")
        {
            document.getElementById(id+"Img").className = "glyphicon glyphicon-arrow-right";

        }
        else
        {
            document.getElementById(id+"Img").className = "glyphicon glyphicon-arrow-down";
        }
    }
    $scope.deleteMessage = function (id) {

        if (document.getElementById(id + "msg").style.textDecoration == 'line-through') {
            document.getElementById(id + "msg").style.textDecoration = "none";
        }
        else {
            document.getElementById(id + "msg").style.textDecoration = "line-through";
            // Send Message to hub That user Delete his message ...
            $scope.hubconnection.server.deleteMessage($scope.user, $scope.activeItem, id);


        }

    };
    $scope.undoMessage = function (id) {
        $scope.hubconnection.server.undoMarkDelete($scope.user, $scope.activeItem, id);
    };
    $scope.hubconnection.client.markAsDelete=function(dmessage)
    {
        
        if (dmessage.group == $scope.activeItem)
        {
            // Find the selected detelted in Group Message List 
            for (var i=0;i<$scope.messages.length;i++ )
            {
                
                if ($scope.messages[i].groupChatId==dmessage.groupChatId )
                {
                    
                    $scope.messages[i].txtStatus = "delete";
                }
            }
        }
        $scope.$apply();
    };
    $scope.hubconnection.client.markAsUndo = function (dmessage) {
        
        if (dmessage.group == $scope.activeItem) {
            // Find the selected detelted in Group Message List 
            for (var i = 0; i < $scope.messages.length; i++) {

                if ($scope.messages[i].groupChatId == dmessage.groupChatId) {

                    $scope.messages[i].txtStatus = null;
                }
            }
        }
        $scope.$apply();
    };
    $scope.hubconnection.client.clientStatusChange = function (selectUser) {

        for (var i = 0; i < $scope.users.length; i++) // - This loop iterate through the users and set the use online which just joined the chat group
        {
            if ($scope.users[i].userId == selectUser.userId) {

                $scope.users[i].activeStatus = selectUser.activeStatus;

            }
        }
        $scope.singleMessage.uid = selectUser.userId;
        $scope.singleMessage.name = selectUser.name;
        $scope.singleMessage.msg = 'Just Changed His Status To (...' + selectUser.activeStatus+" ....)";
        $scope.singleMessage.time = new Date();
        var statusMessage = angular.copy($scope.singleMessage);
        $scope.messages.push(statusMessage);
        ///-------- This below code is just to adjus the scroll bar to an end 
        $timeout(function () {
            var scroller = document.getElementById("autoscrlone");
            // alert(scroller.scrollTop);
            // alert(scroller.scrollHeight);
            scroller.scrollTop = scroller.scrollHeight;
        }, 0, false);
        //-------------------
        $scope.$apply();

    };
    $scope.changeStatus = function (state) {
        
        if (state=='online')
        {
            $scope.hubconnection.server.statusChange(state,$scope.user);
        }
        else if (state=='busy')
        {
            $scope.hubconnection.server.statusChange(state, $scope.user);

        }
        else if (state == 'disturb')
        {
            $scope.hubconnection.server.statusChange(state, $scope.user);

        }
    };
    $scope.logUserInn = function () {
        $scope.register.message = null;
        $http.get('/chat/verifyUserAndGetUser', { params: { userName: $scope.register.userName, password: $scope.register.password } }).success(function (datauser) {
           
            if (datauser!=null && datauser!="")
            {
                //$scope.user = datauser;
                //$scope.hubconnection.server.joinChatt($scope.user);
                $scope.refresh();
            }
            else
            {
                $scope.register.message = "Wrong User Name or Password ??";
            }

        });
        //alert($scope.register.userName);

    };

    $scope.RegisterUserInn = function () {
        $scope.register.message = null;
        if ($scope.register.password !=$scope.register.confirmpassword)
        {
            
            $scope.register.message = " Password and Confirm Password Must Match";
        }
        else
        {
            $http.get('/chat/registerNewUser', { params: { name: $scope.register.userName, password: $scope.register.password } }).success(function (datauser) {
                if (datauser.userId==0)
                {
                    $scope.register.message = "User Name Already Exist , Look Alternative Name ";
                }
                else
                {
                    $scope.refresh();
                }
            });
        }
    };
} // Controller Ending

//------------------------- new controller for Post ---------------------------
function wysiwygeditor($scope, $http, $location) {



}// --- controller ending
function postItemsController($scope, $http, $location) {
    $scope.editPannel = false;
    $scope.categoryList = null;
    $scope.selectCategory = { categoryId: 0, categoryName: 'a' };
    $scope.newCategory = { categoryId: 0, categoryName: null };

    $scope.edit = true;
    $scope.messageShow = null;
    $scope.newKey = {
        postId: 1002,
        postText: null,
        postName: null,
        categoryId: 0

    };
    $scope.htmlcontent = null;
    $scope.disabled = false;
    $scope.postList = null;
    $scope.commentList = null;
    $scope.commentMessage = null;
    $http.get('/Admin/jsGetCategory').success(function (data) {
        $scope.categoryList = data;

    });
    $http.get('/Admin/jsViewPostList').success(function (data) {
        $scope.postList = data;

        //$scope.newKey.staffid = data[0].StaffId // saving the fetch staff id in to anjular view model for ???
    })
   .error(function (data, status) {
       alert(data);
   });


    //--------------------------- Innter Function called by page or events ---------------
    $scope.postSelect = function (postid) {
        $scope.editPannel = true;
        $scope.edit = true;

        $http.get('/Admin/jsViewPost', { params: { postId: postid } }).success(function (data) {
            $scope.htmlcontent = data.posttext;
            $scope.newKey.postId = postid;
            $scope.newKey.postName = data.postname;
            $scope.newKey.postText = data.posttext;
            $scope.newKey.categoryId = data.categoryId;
            // ----- Loop strat 
            for (var c = 0; c < $scope.categoryList.length; c++) {
                if ($scope.categoryList[c].categoryId == $scope.newKey.categoryId) {

                    $scope.selectCategory = $scope.categoryList[c];
                    break;
                }
            }
            //-------- loop ending
        })
       .error(function (data, status) {
           alert(data);
       });
        /// now traverse loop


    }; //-- End of PostSelect
    $scope.savePost = function () {
        if ($scope.edit == true) {
            $scope.newKey.postText = $scope.htmlcontent;
            $scope.newKey.categoryId = $scope.selectCategory.categoryId;

            $http.post('/Admin/EditPosts', $scope.newKey).success(function (data) {

            });
            //$scope.editPannel = false;

            $scope.messageShow = 'Post Updated Successfully ....';
        }
        else {
            $scope.newKey.postText = $scope.htmlcontent;
            $scope.newKey.categoryId = $scope.selectCategory.categoryId;
            $http.post('/Admin/EditPosts', $scope.newKey).success(function (data) {
                $scope.postList.push(data);
            });
            //$scope.editPannel = false;
            $scope.messageShow = 'Post Addedd Successfully ....';
        }

    };

    $scope.addNew = function () {
        $scope.editPannel = true;
        $scope.edit = false;
        $scope.messageShow = null;
        $scope.newKey.postId = 0;
        $scope.htmlcontent = "Start writing here...";
    };

    // ---------- ---------------
    $scope.selectComments = function (postId) {
        $scope.commentList = null;
        $http.get('/Admin/jsViewComments', { params: { postId: postId } }).success(function (data) {
            $scope.commentList = data;

        })
      .error(function (data, status) {
          alert(data);
      });


    };
    //--------------------
    //----------- deleteComment event ---------
    $scope.deleteComment = function (index, commentid) {
        $http.get('/Admin/deleteComments', { params: { commentId: commentid } }).success(function (data) {
            $scope.commentList.splice(index, 1);
            $scope.commentMessage = commentid + ' deleted Successfully';

        })
     .error(function (data, status) {
         alert(data);
     });
    };
    //-------------- Post Category Model events ---------------
    $scope.saveCategory = function () {
        $http.post('/Admin/saveCategory', $scope.newCategory).success(function (data) {
            $scope.categoryList.push(data);

        });
    };
    //--------------------------------------------------------
    $scope.editCategory = function (category) {
        $http.post('/Admin/saveCategory', category).success(function (data) {


        });
    };
    //----------- Refresh Category Page event function
    $scope.loadCategories = function () {
        $http.get('/Admin/jsGetCategory').success(function (data) {
            $scope.categoryList = data;

        });
    };
    //----------------
} // End of Controller

function dashBoard($scope) {

}

app.directive('autoFocus', function ($timeout) {
    return {
        restrict: 'AC',
        link: function (_scope, _element) {
            $timeout(function () {
                _element[0].focus();
            }, 0);
        }
    };
});
app.controller('chatController', chatController);
//app.controller('postController', postController);
edit.controller('wysiwygeditor', wysiwygeditor);
edit.controller('postItemsController', postItemsController);

app.factory('FileUploadService', function ($http, $q) { // explained abour controller and service in part 2

    var fac = {};
    fac.UploadFile = function (file, description) {
        var formData = new FormData();
        formData.append("file", file);
        //We can send more data to server using append         
        formData.append("description", description);

        var defer = $q.defer();
        $http.post("/Chat/SaveFiles", formData,
            {
                withCredentials: true,
                headers: { 'Content-Type': undefined },
                transformRequest: angular.identity
            })
        .success(function (d) {
            defer.resolve(d);
        })
        .error(function () {
            defer.reject("File Upload Failed!");
        });

        return defer.promise;

    }
    return fac;

});

//your directive
app.directive("fileread", [
  function () {
      return {
          scope: {
              fileread: "="
          },
          link: function (scope, element, attributes) {
              element.bind("change", function (changeEvent) {
                  var reader = new FileReader();
                  reader.onload = function (loadEvent) {
                      scope.$apply(function () {
                          scope.fileread = loadEvent.target.result;
                      });
                  }
                  reader.readAsDataURL(changeEvent.target.files[0]);
              });
          }
      }
  }
]);
//---------------- directive for scrolly
app.directive('scrolly', function () {
    return {
        restrict: 'A',
        link: function (scope, element, attrs) {
            var raw = element[0];
            console.log('loading directive');

            element.bind('scroll', function () {
                console.log('in scroll');
                console.log(raw.scrollTop);
                console.log(raw.scrollHeight);
                console.log(raw.offsetHeight);
                if (raw.scrollTop + raw.offsetHeight > raw.scrollHeight) {
                    console.log("I am at the bottom");

                }
                else if (raw.scrollTop < 50) {
                    scope.$apply(attrs.scrolly);
                    console.log("I am at the Top");
                    // scope.$apply(attrs.scrolly);
                }
            });
        }
    };
});
app.directive('focusOn', function () {
    return function (scope, elem, attr) {
        scope.$on('focusOn', function (e, name) {
            if (name === attr.focusOn) {
                elem[0].focus();
            }
        });
    };
});

app.factory('focus', function ($rootScope, $timeout) {
    return function (name) {
        $timeout(function () {
            $rootScope.$broadcast('focusOn', name);
        });
    }
});