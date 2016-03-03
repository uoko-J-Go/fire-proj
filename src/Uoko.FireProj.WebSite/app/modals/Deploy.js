fireproj.controller("DeployController", function ($scope, $http, $uibModalInstance, TaskService, ProjectService, CommonService, param) {
    
    $scope.Project = null;//当前项目对象
    $scope.Server = null;//当前服务器对象
    $scope.Server = null;//当前服务器对象
    $scope.taskInfo = {
        TaskName: "",
        Project: null,
        Branch: "",
        DeployStage: "",
        Server: null,
        DomainInfo: null,
        CheckUsers: [],
        NoticeUsers: [],
        DeployInfo: {}
    };

    $scope.projectList = [];
    $scope.branchList = [];

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data.filter(function (env) {
            return env.Id != 2;
        });
    });
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;
        });
    }
    $scope.loadTags = function (query) {
        var result = $scope.AllUsers.filter(function (user) {
            return (user.name.toLowerCase().indexOf(query.toLowerCase()) != -1) || (user.username.toLowerCase().indexOf(query.toLowerCase()) != -1);
        });
        return result;
    }
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };
    $scope.Save = function (isValid) {
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }
        var project = param;//任务详情页传递过来的参数
       
        var server = $scope.taskInfo.Server;
        if (typeof server == "string") {
            server = JSON.parse(server);
        }
        var domainInfo = $scope.taskInfo.DomainInfo;
        if (typeof domainInfo == "string") {
            domainInfo = JSON.parse(domainInfo);
        }
        var checkUserIds = [];
        var noticeUserIds = [];
        if ($scope.taskInfo.CheckUsers != null && $scope.taskInfo.CheckUsers.length > 0) {
            $.each($scope.taskInfo.CheckUsers, function (i, item) {
                checkUserIds.push(item.id);
            });
        }
        if ($scope.taskInfo.NoticeUsers != null && $scope.taskInfo.NoticeUsers.length > 0) {
            $.each($scope.taskInfo.NoticeUsers, function (i, item) {
                noticeUserIds.push(item.id);
            });
        }

        var taskForSave = {
            TaskName: $scope.taskInfo.TaskName,
            Branch: $scope.taskInfo.Branch,
            ProjectId: project.ProjectId,
            DeployStage: $scope.taskInfo.DeployStage,
            Id:param.taskId,
        }
        if (taskForSave.DeployStage == 0) {
            taskForSave.IocDeployInfo = $scope.taskInfo.DeployInfo;
            taskForSave.IocDeployInfo.DeployIP = server.IP;
            taskForSave.IocDeployInfo.Domain = domainInfo.Name;
            taskForSave.IocDeployInfo.SiteName = domainInfo.SiteName;
            taskForSave.IocDeployInfo.CheckUserId = checkUserIds.join(",");
            taskForSave.IocDeployInfo.NoticeUserId = noticeUserIds.join(",");
        } else if (taskForSave.DeployStage == 1) {
            taskForSave.PreDeployInfo = $scope.taskInfo.DeployInfo;
            taskForSave.PreDeployInfo.DeployIP = server.IP;
            taskForSave.PreDeployInfo.Domain = domainInfo.Name;
            taskForSave.PreDeployInfo.SiteName = domainInfo.SiteName;
            taskForSave.PreDeployInfo.CheckUserId = checkUserIds.join(",");
            taskForSave.PreDeployInfo.NoticeUserId = noticeUserIds.join(",");
        }

        TaskService.UpdateTask(taskForSave, function (data) {
            bootbox.alert("成功发起部署...", function (data) {
                $uibModalInstance.close();
                window.location.reload();
            });
        });
    }

    //发布环境change事件,获取IOC环境的服务器List
    $scope.GetServerData = function (environmentId) {
        TaskService.GetResourceList(environmentId, function (data) {
            $scope.ServerList = data;
        });
    }
    //部署服务器change事件
    $scope.GetDomain = function (server) {
        if (typeof server == "string") {
            server = JSON.parse(server);
        }
        if (server == undefined) {
            server = { Id: 0 };
        }
        TaskService.GetDomain(param.ProjectId, server.Id,null, function (data) {
            $scope.DomainList = data;
        });
    }
    //根据项目Id或者分支列表
    $scope.getBranch = function (RepoId) {
        CommonService.getProjectBranch(RepoId, function (data) {
            $scope.branchList = data;
        });
    }


    $scope.Init = function () {
        $scope.GetProjectList();
        $scope.GetAllUser();
        $scope.getBranch(param.RepoId);
        $scope.$watch('taskInfo.Project + taskInfo.DeployEnvironment + taskInfo.Server', function () {
            $scope.GetDomain($scope.taskInfo.Server);
        });
    }

    $scope.Init();

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    }


   
});