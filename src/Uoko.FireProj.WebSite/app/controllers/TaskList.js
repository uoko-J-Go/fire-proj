﻿
fireproj.controller("TaskController", function ($scope, $http, $uibModal,TaskService, ProjectService, CommonService) {
    $scope.projectList = [];
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };
    $scope.pageSize = 10;
    $scope.currentPage = 1;

    $scope.taskInfos = [];
    $scope.totalItems = 0;//总数
    $scope.queryType = 0; // 所有

    //查询项目
    $scope.Query = function (showType) {

        if (showType) {
            $scope.queryType = showType;
        }

        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            ShowType: $scope.queryType,
        }

        TaskService.GetTaskByPage(params,function (data) {
            $scope.totalItems = data.total;
            var tasks = data.rows;
            _.each(tasks, function(task, key) {
                if (task.TaskInfo.DeployInfoIocJson) {
                    task.DeployInfoIoc = JSON.parse(task.TaskInfo.DeployInfoIocJson);
                }
                if (task.TaskInfo.DeployInfoPreJson) {
                    task.DeployInfoPre = JSON.parse(task.TaskInfo.DeployInfoPreJson);
                }
                if (task.TaskInfo.DeployInfoOnlineJson) {
                    task.DeployInfoOnline = JSON.parse(task.TaskInfo.DeployInfoOnlineJson);
                }

                task.IocTestAllPassed = task.TaskInfo.IocCheckUserId && !(/-[01]/m.test(task.TaskInfo.IocCheckUserId));

                task.PreTestAllPassed = task.TaskInfo.PreCheckUserId && !(/-[01]/m.test(task.TaskInfo.PreCheckUserId));

                task.OnlineTestAllPassed = task.TaskInfo.OnlineCheckUserId && !(/-[01]/m.test(task.TaskInfo.OnlineCheckUserId));

            });
            
            $scope.taskInfos = tasks;
        });
    }

    $scope.Deploy = function (task) {
        $scope.param = {
            taskId: task.TaskInfo.Id,
        };
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/Deploy.html',
            controller: 'DeployController',
            resolve: {
                param: function () {
                    return $scope.param;
                }
            }
        });
    }
    $scope.Init = function () {
        $scope.Query();
        $scope.GetProjectList();
    }
    ///提交测试,状态改为8测试中
    $scope.CommitToTest = function (item) {
        bootbox.prompt("批示", function (result) {
            if (result != null) {
             paran = {
                "Id": item.Id,
                "LogsText": result
            }
            TaskService.CommitToTest(paran, function (data) {
                formSubmitSuccessClick("refresh");
            });
            }
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


