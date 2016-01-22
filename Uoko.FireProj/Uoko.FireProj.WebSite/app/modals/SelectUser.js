fireproj.controller("SelectUserController", function ($scope, $uibModalInstance, selectedUsers) {
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

    $scope.Init=function() {
        $scope.userList = [
            { Id: 1, Name: "山姆", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182836_8MEhe.jpeg" },
            { Id: 2, Name: "庆攀", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182730_sNGAS.thumb.700_0.jpeg" },
            { Id: 3, Name: "志平", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182836_8MEhe.jpeg" },
            { Id: 4, Name: "佳君", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182730_sNGAS.thumb.700_0.jpeg" }
        ];

        for (var i = 0; i < $scope.userList.length; i++) {
            var user = $scope.userList[i];
            for (var j = 0; j < selectedUsers.length; j++) {
                if (user.Id === selectedUsers[j].Id) {
                    user.selected = true;
                }
            }
        }
    }
    $scope.Init();
});