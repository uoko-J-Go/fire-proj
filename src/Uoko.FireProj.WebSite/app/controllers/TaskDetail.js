fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService,CommonService) {
    var taskId = $("#taskIdParam").val();
    $scope.currLogTab = 0;
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;
            $scope.GetTaskInfo();
        });
    }
    $scope.GetTaskInfo = function () {
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.model = data;
            $scope.param = {
                taskId: taskId,
            };
            $scope.model.DeployInfoIocDto.CheckUser = AnalysisUser($scope.model.DeployInfoIocDto.CheckUser,$scope.AllUsers);
            $scope.model.DeployInfoIocDto.NoticeUser = AnalysisUser($scope.model.DeployInfoIocDto.NoticeUser, $scope.AllUsers);
            $scope.model.DeployInfoPreDto.CheckUser = AnalysisUser($scope.model.DeployInfoPreDto.CheckUser, $scope.AllUsers);
            $scope.model.DeployInfoPreDto.NoticeUser = AnalysisUser($scope.model.DeployInfoPreDto.NoticeUser, $scope.AllUsers);
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
    $scope.GotoGitLabBuildPage=function(buildId) {
        var url = $scope.model.ProjectDto.ProjectRepo.replace(".git","") + "/builds/" + buildId;
        window.open(url, "_blank");
    }
    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }

    $scope.Init = function () {
        $scope.GetAllUser();
        $scope.GetTaskLogsByTaskId();
    }

    $scope.Init();
});