

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService,CommonService) {
    var taskId = $("#taskIdParam").val();
    $scope.taskInfo = {};
    $scope.projectList = [];
    $scope.branchList = [];
    $scope.isFirstLoad = true;
    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;
            $scope.GetProjectList();
          
        });
    }
    $scope.loadTags = function (query) {
        var result = $scope.AllUsers.filter(function(user) {
            return (user.name.toLowerCase().indexOf(query.toLowerCase()) != -1) || (user.username.toLowerCase().indexOf(query.toLowerCase()) != -1);
        });
        return result;
    }

    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
            $scope.GetTaskInfo();
        });
    };
    $scope.GetTaskInfo = function () {
        
        TaskService.GetTaskInfo(taskId, function (data) {

            $scope.GetAllUserDetail(data.CheckUsers, 0);
            $scope.GetAllUserDetail(data.NoticeUses, 0);
            $scope.taskInfo = data;
            $scope.getBranch($scope.taskInfo.Project);
            $scope.GetServerData(data.DeployEnvironment);
        });
    }
    $scope.GetAllUserDetail = function (userList, index) {
        if (index < userList.length) {
            var _user = userList[index];
            var result = $scope.AllUsers.filter(function (user) {
                return user.id == _user.Id;
            });
            userList.splice(index, 1, result[0]);
            if (index < userList.length) {
                ++index;
                $scope.GetAllUserDetail(userList, index);
            }
        }
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
        TaskService.UpdateTask($scope.taskInfo, function () {
            location.href = "/Task/Index";
        });
    }

    //发布环境change事件,获取IOC环境的服务器List
    $scope.GetServerData = function (environmentId) {
        TaskService.GetResourceList(environmentId, function (data) {
            $scope.ServerList = data;
            if ($scope.isFirstLoad) {
                var _server = $scope.ServerList.filter(function(server) {
                    return server.IP == $scope.taskInfo.DeployIP;
                })[0];
                $scope.taskInfo.Server = JSON.stringify(_server);
                $scope.isFirstLoad = false;
            }
           
            //$scope.$apply();
        });
    }

    //根据项目Id或者分支列表
    $scope.getBranch = function (project) {
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        CommonService.getProjectBranch(project.RepoId, function (data) {
            $scope.branchList = data;
        });
    }
    //部署服务器change事件
    $scope.GetDomain = function (project, server) {
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        if (typeof server == "string") {
            server = JSON.parse(server);
        }

        if (project != undefined && project != "") {
            if (server == undefined) {
                server = { Id: 0 };
            }
            TaskService.GetDomainByTaskId(project.Id, server.Id, taskId, function (data) {
                $scope.DomainList = data;
            });
        }
    }
    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }

    $scope.Init = function () {
        $scope.GetAllUser();
        $scope.$watch('taskInfo.Project + taskInfo.DeployEnvironment + taskInfo.Server', function () {
            $scope.GetDomain($scope.taskInfo.Project, $scope.taskInfo.Server);
        });
    }

    $scope.Init();
});