﻿
fireproj.controller("TaskController", function ($scope, $http, TaskService, ProjectService, CommonService) {
    $scope.projectList = [];
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.items = [];
    $scope.totalItems = 0;//总数
    //查询项目
    $scope.Query = function () {
        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize
        }
        TaskService.GetTaskByPage(params,function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        });

    }
    $scope.Deploy = function (item) {
        TaskService.GetTaskInfo(item.Id, function (data) {
            var taskInfo = data;
            CommonService.TriggerBuild(taskInfo, function (data) {
                bootbox.alert("已经成功发起部署任务，点击详细进行查看!", function () {              
                    TaskService.BeginDeploy(taskInfo.Id, data.id, function (data) {
                        location.reload();
                    });
                });
                
            });
        });
    }
    $scope.Init = function () {
        $scope.Query();
        $scope.GetProjectList();
    }
    ///提交测试,状态改为8测试中
    $scope.CommitToTest = function (item) {
        TaskService.CommitToTest(item.Id, function (data) {
            formSubmitSuccessClick("refresh");
        });
    }
    ///释放资源操作
    $scope.ReleaseDomain = function (taskId) {
        TaskService.ReleaseDomain(taskId, function (data) {
            formSubmitSuccessClick("refresh");
        });
    }
    $scope.Init();
});


