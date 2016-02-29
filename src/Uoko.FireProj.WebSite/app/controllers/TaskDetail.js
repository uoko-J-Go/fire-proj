
fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {};
    $scope.tasklogTotal = {};
    $scope.currLogTab = 0;
    $scope.GetTaskInfo = function () {
        var taskId = $("#taskIdParam").val();
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.taskInfo = data;
            $scope.GetLogTotal($scope.taskInfo.Id);
            $scope.GetAllUserDetail($scope.taskInfo.CheckUsers, 0);
            $scope.GetAllUserDetail($scope.taskInfo.NoticeUses, 0);
         
        });
    }
    //获取当前任务在各个环境中的任务记录数
    $scope.GetLogTotal = function (taskId) {
        TaskService.GetLogTotal(taskId, function (data) {
            $scope.tasklogTotal = data;
            $scope.currLogTab = $scope.taskInfo.DeployEnvironment;
            $scope.Query();
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
        bootbox.prompt("批示", function (result) {
            param = {
                "Id": $scope.taskInfo.Id,
                "LogsText": result
            };
            TaskService.TestFails(param, function (data) {
                formSubmitSuccessClick();
            });
        });

       
    }

    $scope.Tested = function () {
        bootbox.prompt("批示", function (result) {
            param = {
                "Id": $scope.taskInfo.Id,
                "LogsText": result
            };
            TaskService.Tested(param, function (data) {
                formSubmitSuccessClick();
            });
        });
    }

    $scope.Deploy = function () {
        CommonService.TriggerBuild($scope.taskInfo, function (data) {
            bootbox.alert("已经成功发起部署任务!", function () {
                TaskService.BeginDeploy($scope.taskInfo.Id, data.id, function (data) {
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

    /*任务记录相关*/
    $scope.pageSize = 0;
    $scope.currentPage = 1;
    $scope.items = [];
    $scope.totalItems = 0;//总数
    //查询项目
    $scope.Query = function () {
        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            taskId: $scope.taskInfo.Id,
            environment: $scope.currLogTab,
            sort: "CreateDate",
            order:"desc"
        }
        TaskService.GetTaskLogsByPage(params, function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        });

    }

    $scope.GotoGitLabBuildPage = function (buildId) {
        ProjectService.getByTaskId($scope.taskInfo.Id,function(data) {
            var url = data.ProjectRepo.replace(".git", "") + "/builds/" + buildId;
            window.open(url, '_blank');
        });
    }
    $scope.GetCurrLog = function (enviroment) {
        $scope.currLogTab = enviroment;
        $scope.currentPage = 1;
        $scope.items = [];
        $scope.totalItems = 0;//总数
        $scope.Query();
    }

    /*任务记录相关*/
    $scope.Init = function () {
        $scope.GetTaskInfo();
    }

    $scope.Init();
});