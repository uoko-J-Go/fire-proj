﻿

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.IsSubmiting = false;
    $scope.Project = null;//当前项目对象
    $scope.Server = null;//当前服务器对象
    $scope.Server = null;//当前服务器对象
    $scope.taskInfo = {
        TaskName: "",
        Project: null,
        Branch: null,
        DeployStage: null,
        Server: null,
        DomainInfo: null,
        CheckUsers:[],
        NoticeUsers: [],
        OnlineCheckUsers: [],
        OnlineNoticeUsers: [],
        DeployInfo: {}
    };

    $scope.projectList = [];
    $scope.branchList = [];

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data.filter(function (env) {
            return env.Id !=2;
        });
    });
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;
        });
    }
    $scope.loadTags = function (query) {
        var result = $scope.AllUsers.filter(function(user) {
            return (user.NickName.toLowerCase().indexOf(query.toLowerCase()) != -1) || (user.LoginName.toLowerCase().indexOf(query.toLowerCase()) != -1);
        });
        return result;
    }
    $scope.GetProjectList = function() {
        ProjectService.getAllProject(function(data) {
            $scope.projectList = data;
        });
    };
    $scope.Save = function (isValid) {
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }
        if ($scope.IsSubmiting) {
            return;
        }
        $scope.IsSubmiting = true;
        var project = $scope.taskInfo.Project;
        var server = $scope.taskInfo.Server;
        var domainInfo = $scope.taskInfo.DomainInfo;

        var checkUserIds = [];
        var noticeUserIds = [];
        if ($scope.taskInfo.CheckUsers != null && $scope.taskInfo.CheckUsers.length > 0) {
            $.each($scope.taskInfo.CheckUsers, function (i, item) {
                checkUserIds.push(item.UserId);
            });
        }
        if ($scope.taskInfo.NoticeUsers != null && $scope.taskInfo.NoticeUsers.length > 0) {
            $.each($scope.taskInfo.NoticeUsers, function (i, item) {
                noticeUserIds.push(item.UserId);
            });
        }
        
        var taskForSave= {
            TaskName:$scope.taskInfo.TaskName,
            Branch:$scope.taskInfo.Branch.name,
            ProjectId: project.Id,
            DeployStage: $scope.taskInfo.DeployStage.Id
        }
        if (taskForSave.DeployStage == 0) {
            taskForSave.IocDeployInfo = $scope.taskInfo.DeployInfo;
            taskForSave.IocDeployInfo.DeployIP =server.IP;
            taskForSave.IocDeployInfo.Domain =domainInfo.Name;
            taskForSave.IocDeployInfo.SiteName =domainInfo.SiteName;
            taskForSave.IocDeployInfo.CheckUserId = checkUserIds.join(",");
            taskForSave.IocDeployInfo.NoticeUserId = noticeUserIds.join(",");
        } else if (taskForSave.DeployStage == 1) {
            taskForSave.PreDeployInfo = $scope.taskInfo.DeployInfo;
            taskForSave.PreDeployInfo.DeployIP = server.IP;
            taskForSave.PreDeployInfo.Domain =domainInfo.Name;
            taskForSave.PreDeployInfo.SiteName =domainInfo.SiteName;
            taskForSave.PreDeployInfo.CheckUserId = checkUserIds.join(",");
            taskForSave.PreDeployInfo.NoticeUserId = noticeUserIds.join(",");

            /*添加上线相关信息*/
            var onlineCheckUserIds = [];
            var onlineNoticeUserIds = [];
            if ($scope.taskInfo.OnlineCheckUsers != null && $scope.taskInfo.OnlineCheckUsers.length > 0) {
                $.each($scope.taskInfo.OnlineCheckUsers, function (i, item) {
                    onlineCheckUserIds.push(item.UserId);
                });
            }
            if ($scope.taskInfo.OnlineNoticeUsers != null && $scope.taskInfo.OnlineNoticeUsers.length > 0) {
                $.each($scope.taskInfo.OnlineNoticeUsers, function (i, item) {
                    onlineNoticeUserIds.push(item.UserId);
                });
            }
            taskForSave.OnlineDeployInfo = {
                CheckUserId: onlineCheckUserIds.join(","),
                NoticeUserId: onlineNoticeUserIds.join(",")
            }
            /*添加上线相关信息*/   
        }

        TaskService.CreateTask(taskForSave, function (data) {
            $scope.IsSubmiting = false;
            location.href = "/Task/Index";
            
        },function(data) {
            $scope.IsSubmiting = false;
        });
    }

    //发布环境change事件,获取TEST环境的服务器List
    $scope.GetServerData = function () {
        TaskService.GetResourceList($scope.taskInfo.DeployStage.Id, function (data) {
            $scope.ServerList = data;
        });
    }
    //部署服务器change事件
    $scope.GetDomain = function (project, server) {
       
        if (!project || !server) {
            return;
        }
        TaskService.GetDomain(project.Id, server.Id, 0, function (data) {
            $scope.DomainList = data;
        });
    }
    //根据项目Id或者分支列表
    $scope.getBranch = function () {
        CommonService.getProjectBranch($scope.taskInfo.Project.RepoId, function (data) {
            $scope.branchList = data;
        });
    }

    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }

    $scope.Init = function () {
        $scope.GetProjectList();
        $scope.GetAllUser();
        $scope.$watch('taskInfo.Project.Id + taskInfo.DeployStage.Id + taskInfo.Server.Id', function () {
            $scope.GetDomain($scope.taskInfo.Project, $scope.taskInfo.Server);
        });
    }

    $scope.Init();
});