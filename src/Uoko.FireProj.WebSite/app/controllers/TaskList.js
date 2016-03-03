
fireproj.controller("TaskController", function ($scope, $http, TaskService, ProjectService, CommonService) {
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

    $scope.Deploy = function (item) {
        TaskService.GetTaskInfo(item.Id, function (data) {
            var taskInfo = data;
            CommonService.TriggerBuild(taskInfo, function (data) {
                bootbox.alert("已经成功发起部署任务，点击查看详细!", function () {              
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


