

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {
        TaskName: "",
        Project: null,
        Branch: "",
        DeployEnvironment: "",
        DeployIP: "",
        DeployAddress: "",
        SiteName: "",
        CheckUsers: [],
        NoticeUses: [],
        TaskDesc: "",
        Domain: "",
    };
    
    $scope.projectList = [];
    $scope.branchList = [];
    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });
    $scope.GetProjectList = function() {
        ProjectService.getAllProject(function(data) {
            $scope.projectList = data;
        });
    };
    //选择审核人
    $scope.selectCheckUser = function () {
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/SelectUser.html',
            controller: 'SelectUserController',
            resolve: {
                selectedUsers: function () {
                    return $scope.taskInfo.CheckUsers;
                }
            }
        });

        modalInstance.result.then(function (selectedUsers) {
            $scope.taskInfo.CheckUsers = selectedUsers;
        });
    }
    //选择相关人
    $scope.selectNoticeUser = function () {
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/SelectUser.html',
            controller: 'SelectUserController',
            resolve: {
                selectedUsers: function () {
                    return $scope.taskInfo.NoticeUses;
                }
            }
        });

        modalInstance.result.then(function (selectedUsers) {
            $scope.taskInfo.NoticeUses = selectedUsers;
        });
    }
    //移除审核人
    $scope.removeCheckUser = function (index) {
        $scope.taskInfo.CheckUsers.splice(index, 1);
        $scope.$evalAsync();
    }
    //移除相关人
    $scope.removeNoticeUser = function (index) {
        $scope.taskInfo.NoticeUses.splice(index, 1);
        $scope.$evalAsync();
    }
    $scope.Save = function (isValid) {
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }
        var project = $scope.taskInfo.Project;
        if (typeof project == "string") {
            $scope.taskInfo.Project = JSON.parse(project);
        }
        var server = $scope.taskInfo.Server;
        if (typeof project == "string") {
            server = JSON.parse(server);
            $scope.taskInfo.DeployIP = server.IP;
            $scope.taskInfo.ServerId = server.Id;
        }
        TaskService.CreateTask($scope.taskInfo, function() {
            location.href = "/Task/Index";
        });
    }

    //发布环境change事件,获取IOC环境的服务器List
    $scope.GetServerData = function (environmentId) {
        if (environmentId == 0) { //IOC环境
            TaskService.GetResourceList(environmentId, function (data) {
                $scope.ServerList = data;
            });
        }

    }
    //部署服务器change事件
    $scope.GetDomain = function (project, server) {
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        if (typeof server == "string") {
            server = JSON.parse(server);
        }

        if (project != undefined && project!= "") {
            if (server == undefined) {
                server = { Id: 0 };
            }
            TaskService.GetDomain(project.Id, server.Id, function (data) {
                $scope.DomainList = data;
            });
        }   
    }
    //根据项目Id或者分支列表
    $scope.getBranch = function (project) {
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        CommonService.getProjectBranch(project.ProjectId, function (data) {
            $scope.branchList = data;
        });
    }

    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }
    
    $scope.Init = function () {
        $scope.GetProjectList();
        $scope.$watch('taskInfo.Project + taskInfo.DeployEnvironment + taskInfo.Server', function () {
            $scope.GetDomain($scope.taskInfo.Project, $scope.taskInfo.Server);
        });
    }

    $scope.Init();
});