fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService,CommonService) {
    var taskId = $("#taskIdParam").val();
    var userId = $("#userId").val();
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;
            $scope.GetTaskInfo();
        });
    }
    $scope.TabChanged=function(index) {
        $scope.currLogTab = index;
    }
    $scope.GetTaskInfo = function () {
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.model = data;
            $scope.param = {
                taskId: taskId 
            };
            $scope.model.DeployInfoIocDto.CheckUser = AnalysisUser($scope.model.DeployInfoIocDto.CheckUser,$scope.AllUsers);
            $scope.model.DeployInfoIocDto.NoticeUser = AnalysisUser($scope.model.DeployInfoIocDto.NoticeUser, $scope.AllUsers);
            $scope.model.DeployInfoPreDto.CheckUser = AnalysisUser($scope.model.DeployInfoPreDto.CheckUser, $scope.AllUsers);
            $scope.model.DeployInfoPreDto.NoticeUser = AnalysisUser($scope.model.DeployInfoPreDto.NoticeUser, $scope.AllUsers);
            $scope.model.DeployInfoOnlineDto.CheckUser = AnalysisUser($scope.model.DeployInfoOnlineDto.CheckUser, $scope.AllUsers);
            $scope.model.DeployInfoOnlineDto.NoticeUser = AnalysisUser($scope.model.DeployInfoOnlineDto.NoticeUser, $scope.AllUsers);

            if ($scope.model.DeployInfoIocJson != null) {
                $scope.currLogTab = 0;
            }else if ($scope.model.DeployInfoPreJson != null) {
                $scope.currLogTab = 1;
            }
            //测试操作按钮控制
            $scope.IocTestShow = $scope.model.DeployInfoIocDto.DeployStatus==2&&$scope.model.DeployInfoIocDto.CheckUser.filter(function (user) {
                return user.Id == userId;
            }).length > 0;
            $scope.PreTestShow = $scope.model.DeployInfoPreDto.DeployStatus == 2&&$scope.model.DeployInfoPreDto.CheckUser.filter(function (user) {
                return user.Id == userId;
            }).length > 0;
            $scope.OnlineTestShow = $scope.model.DeployInfoOnlineDto.OnlineTaskId>0&&$scope.model.DeployInfoOnlineDto.CheckUser.filter(function (user) {
                return user.Id == userId;
            }).length > 0;
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
                    $scope.param.stage = $scope.currLogTab;
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