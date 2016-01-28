
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
        TaskService.TestFails($scope.taskInfo.Id, function (data) {
            formSubmitSuccessClick();
        });
    }

    $scope.Tested = function () {
        TaskService.Tested($scope.taskInfo.Id, function (data) {
            formSubmitSuccessClick();
        });
    }
    $scope.Deploy = function () {
        CommonService.TriggerBuild(taskInfo, function (data) {
            bootbox.alert("已经成功发起部署任务!", function () {
                TaskService.BeginDeploy(taskId, data.id, function (data) {
                    location.reload();
                });
            });

        });
    }

    $scope.CommitToTest = function () {
        TaskService.CommitToTest($scope.taskInfo.Id, function (data) {
            bootbox.alert("已经成功提交测试!", function () {
                location.reload();
            });
        });
    }
    $scope.Init = function () {
        $scope.GetTaskInfo();
    }

    $scope.Init();
});