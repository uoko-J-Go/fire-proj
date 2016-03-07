fireproj.controller("DeployController", function ($scope, $http, $uibModalInstance, TaskService, ProjectService, CommonService, param) {
    $scope.projectList = [];
    $scope.branchList = [];
    $scope.GetDeployStage=function() {
        TaskService.GetEnvironment(function (data) {
            $scope.environmentList = data.filter(function (env) {
                return env.Id != 2;
            });
            $scope.GetProjectList();
        });
    }
    $scope.AllUsers = [];
    $scope.GetAllUser = function () {
        CommonService.getAllUsers(function (data) {
            $scope.AllUsers = data;

            $scope.GetDeployStage();
           
        });
    }
    $scope.loadTags = function (query) {
        var result = $scope.AllUsers.filter(function (user) {
            return (user.NickName.toLowerCase().indexOf(query.toLowerCase()) != -1) || (user.LoginName.toLowerCase().indexOf(query.toLowerCase()) != -1);
        });
        return result;
    }
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;

            $scope.GetTaskInfo();
        });
    };
    $scope.Save = function (isValid) {
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }
        var project = $scope.taskInfo.Project;
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
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
                checkUserIds.push(item.UserId);
            });
        }
        if ($scope.taskInfo.NoticeUsers != null && $scope.taskInfo.NoticeUsers.length > 0) {
            $.each($scope.taskInfo.NoticeUsers, function (i, item) {
                noticeUserIds.push(item.UserId);
            });
        }

        var taskForSave = {
            Id: param.taskId,
            TaskName: $scope.taskInfo.TaskName,
            Branch: $scope.taskInfo.Branch,
            ProjectId: project.Id,
            DeployStage: $scope.taskInfo.DeployStage
        };
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

        TaskService.UpdateTask(taskForSave, function (data) {
            $uibModalInstance.close();
            window.location.reload();
        });
    }

    //发布环境change事件,获取IOC环境的服务器List
    $scope.GetServerData = function (environmentId) {
        TaskService.GetResourceList(environmentId, function (data) {
            $scope.ServerList = data;

            $scope.taskInfo.Branch = $scope.taskInfo.Branch;
            var obj
            if (environmentId == 0) {
                obj = $scope.taskInfo.DeployInfoIocDto;
            }
            else {
                obj = $scope.taskInfo.DeployInfoPreDto;

                //绑定上线信息相关人
                //绑定测试,通知人
                $scope.taskInfo.OnlineCheckUsers = AnalysisUser($scope.taskInfo.DeployInfoOnlineDto.CheckUser, $scope.AllUsers);
                $scope.taskInfo.OnlineNoticeUsers = AnalysisUser($scope.taskInfo.DeployInfoOnlineDto.NoticeUser, $scope.AllUsers);
            }
            //绑定服务器
            $scope.taskInfo.DeployIP = obj.DeployIP;
            var _server = $scope.ServerList.filter(function (server) {
                return server.IP == $scope.taskInfo.DeployIP;
            })[0];
            $scope.taskInfo.Server = JSON.stringify(_server);
            //绑定域名
            $scope.taskInfo.DeployIP = obj.Domain;
            //绑定测试,通知人
            $scope.taskInfo.CheckUsers = AnalysisUser(obj.CheckUser, $scope.AllUsers);
            $scope.taskInfo.NoticeUsers = AnalysisUser(obj.NoticeUser, $scope.AllUsers);
            //备注
            $scope.taskInfo.DeployInfo = {
                TaskDesc: obj.TaskDesc
            };


            $scope.$watch('taskInfo.DeployStage + taskInfo.Server', function () {
                $scope.GetDomain($scope.taskInfo.Project, $scope.taskInfo.Server);
            });
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
            TaskService.GetDomain(project.Id, server.Id, param.taskId, function (data) {
                $scope.DomainList = data;
                //域名ng-select
                var _domain = $scope.DomainList.filter(function (server) {
                    return server.Name == $scope.taskInfo.DeployIP;
                })[0];
                $scope.taskInfo.DomainInfo = JSON.stringify(_domain);

            });
        }
    }
    //根据项目Id或者分支列表
    $scope.getBranch = function (RepoId) {
        CommonService.getProjectBranch(RepoId, function (data) {
            $scope.branchList = data;
        });
    }

    //获得任务详情
    $scope.GetTaskInfo = function () {
        TaskService.GetTaskInfo(param.taskId, function (data) {
            $scope.taskInfo = data;  
            $scope.getBranch(data.ProjectDto.RepoId);//加载分支list
            $scope.taskInfo.Project = data.ProjectDto;

            if (param.stage != undefined) {
                $scope.taskInfo.DeployStage = param.stage;
            } else {
                $scope.taskInfo.DeployStage = 0;
            }
            $scope.GetServerData($scope.taskInfo.DeployStage);
        });
    }

    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    }

    $scope.Init = function () {      
        $scope.GetAllUser(); 
       
    }

    $scope.Init();


   
});