
fireproj.controller("TaskController", function ($scope, $http, TaskService) {
    var taskId = $("#id").val();
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.items = [];
    $scope.totalItems = 0;//总数
    //查询项目
    $scope.Query = function () {
        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            taskId: taskId
        }
        TaskService.GetTaskLogsByPage(params, function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        });

    }
    $scope.GetTaskInfo = function () {
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.taskInfo = data;
        });
    }

    $scope.Init = function () {
        $scope.Query();
        $scope.GetTaskInfo();
    }

    $scope.Init();
});