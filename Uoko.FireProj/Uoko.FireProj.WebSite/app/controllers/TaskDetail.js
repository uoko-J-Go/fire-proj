
fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {};
    $scope.GetTaskInfo = function () {
        var taskId = $("#taskIdParam").val();
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.taskInfo = data;
            $scope.GetAllUserDetail($scope.taskInfo.CheckUsers, 0);
            $scope.GetAllUserDetail($scope.taskInfo.NoticeUses, 0);
         
        });
    }
    $scope.GetAllUserDetail = function (userList, index) {
        if (index < userList.length) {
            var user = userList[index];
            CommonService.getSingleUser(user.Id, function (data) {
                userList.splice(index, 1, data);
                if (index < userList.length) {
                    ++index;
                    $scope.GetAllUserDetail(userList, index);
                }
            });
        }
    }

    
    $scope.Edit = function () {
        location.href = "/Task/Edit?taskId=" + $scope.taskInfo.Id;
    }

    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }

    $scope.TestFails = function () {
        var param = {
            id: $scope.taskInfo.Id,
            Status: 4
        };
        TaskService.UpdateTaskStatus(param, function (data) {
            formSubmitSuccessClick();
        });
    }

    $scope.Tested = function () {
        var param = {
            id: $scope.taskInfo.Id,
            Status: 5
        };
        TaskService.UpdateTaskStatus(param, function (data) {
            formSubmitSuccessClick();
        });
    }

    $scope.Init = function () {
        $scope.GetTaskInfo();
    }

    $scope.Init();
});