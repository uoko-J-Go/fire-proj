fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService,CommonService) {
    var taskId = $("#taskIdParam").val();
    $scope.currLogTab = 0;
    $scope.GetTaskInfo = function () {
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.model = data;
            $scope.param = {
                taskId: taskId,
                ProjectId: data.ProjectDto.Id,
                RepoId: data.ProjectDto.RepoId,
                Branch:data.Branch,
                DeployInfoIocJson: data.DeployInfoIocJson,
                DeployInfoPreJson: data.DeployInfoPreJson
            };
        });
    }
    $scope.GetTaskLogsByTaskId = function () {
        TaskService.GetTaskLogsByTaskId(taskId, function (data) {
            $scope.items = data;
        });
    }
    
    //编译部署
    $scope.Deploy = function () {
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
    $scope.TestPassed = function (stage) {
      
        var testresult= {
            TaskId: taskId,
            Stage: stage,
            QAStatus: 2,
            Comments:""
        }
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/TestDailog.html',
            controller: 'TestController',
            resolve: {
                param: function () {
                    return testresult;
                }
            }
        });
    }
    $scope.TestFails = function (stage) {
        var testresult = {
            TaskId: taskId,
            Stage: stage,
            QAStatus: 1,
            Comments: ""
        }
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/TestDailog.html',
            controller: 'TestController',
            resolve: {
                param: function () {
                    return testresult;
                }
            }
        });
    }
    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }

    $scope.Init = function () {
        $scope.GetTaskInfo();
        $scope.GetTaskLogsByTaskId();
    }

    $scope.Init();
});