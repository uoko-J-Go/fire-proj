fireproj.controller("SelectUserController", function ($scope, $uibModalInstance,CommonService, selectedUsers) {
    $scope.userList = [];
    $scope.selectedUsers=[];
    $scope.Ok = function () {

        for (var i = 0; i < $scope.userList.length; i++) {
            var user = $scope.userList[i];
            if (user.selected) {
                $scope.selectedUsers.push(user);
            }
        }
        $uibModalInstance.close($scope.selectedUsers);
    }
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    }
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function(data) {
            $scope.userList = data;
            for (var i = 0; i < $scope.userList.length; i++) {
                var user = $scope.userList[i];
                for (var j = 0; j < selectedUsers.length; j++) {
                    if (user.id === selectedUsers[j].id) {
                        user.selected = true;
                    }
                }
            }
        });


        
    }
    $scope.Init = function () {
        $scope.GetAllUser();
       
    }
    $scope.Init();
});