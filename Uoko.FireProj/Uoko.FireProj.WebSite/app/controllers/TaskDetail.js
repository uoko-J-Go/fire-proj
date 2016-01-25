
fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {};
    $scope.GetTaskInfo = function () {
        var taskId = $("#taskIdParam").val();
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.taskInfo = data;
            for (var i = 0; i < $scope.taskInfo.CheckUsers.length; i++) {
                var user = $scope.taskInfo.CheckUsers[i];
                CommonService.getSingleUser(user.Id, function (data) {
                    user = data;
                    //$scope.$apply();
                });
            }
            for (var i = 0; i < $scope.taskInfo.NoticeUses.length; i++) {
                var user = $scope.taskInfo.NoticeUses[i];
                CommonService.getSingleUser(user.Id, function (data) {
                    user = data;
                });
            }
        });
    };

    
    $scope.Edit = function () {
        location.href = "/Task/Edit?taskId=" + $scope.taskInfo.Id;
    }
    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }
    
    $scope.Init = function () {
        $scope.GetTaskInfo();
    }

    $scope.Init();
});