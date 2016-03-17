fireproj.controller("OnlineListController", function($scope, $http, $uibModal, TaskService, ProjectService) {
   
    $scope.projects = [];
    $scope.servers = [];
    $scope.domains = [];

    $scope.taskInfos = [];

    $scope.projectSelected = null;
    $scope.serverSelected = null;
    $scope.onlineVersion = null;
    $scope.showFire = true;

    function getProjectList() {
        ProjectService.getAllProject(function(data) {
            $scope.projects = data.filter(function(proj) {
                return proj.CreatorId == $("#userId").val();
            });
        });
    };

    function getServerData() {
        TaskService.GetResourceList(2, function(data) {
            $scope.servers = data;
        });
    }

    function getDomain(project, server) {
        if (project) {
            if (!server) {
                server = { Id: 0 };
            }

            TaskService.GetDomain(project.Id, server.Id, 0, function(data) {
                $scope.domains = data;
            });
        }
    }

    //查询项目
    $scope.Query = function () {

        var projectId = 0;
        if ($scope.projectSelected) {
            projectId = $scope.projectSelected.Id;
        }

        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            ProjectId: projectId,
        };

        TaskService.GetTasksNeedToBeOnline(params, function (data) {
            $scope.taskInfos = data;
        });
        if (projectId > 0) {
            TaskService.CheckOnlineByProjectId(projectId, function (data) {
                if (data.length > 0) {
                    var showInfo = "";
                    for (var i = 0; i < data.length; i++) {
                        showInfo += "<a target='_blank' class='text-primary' href='/Task/Detail?taskId=" + data[i].Id + "'>" + data[i].TaskName + "</a></br>";
                    }
                    $scope.showFire = false;
                    bootbox.alert("该项目下存在以下任务未部署成功:</br>" + showInfo + "");
                }
                else {
                    $scope.showFire = true;
                }
            });
        }
    }


    $scope.Fire = function (isValid) {
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }

        var onlineTaskInfo = {
            OnlineVersion: $scope.onlineVersion,
            ProjectId: $scope.projectSelected.Id,
            ProjectName: $scope.projectSelected.ProjectName,
            DeployServerId: $scope.serverSelected.Id,
            DeployServerIP: $scope.serverSelected.IP,
            DeployServerName: $scope.serverSelected.Name,
            Domain: $scope.domainSelected.Name,
            SiteName: $scope.domainSelected.SiteName,
        };

        TaskService.FireProject(onlineTaskInfo, function(data) {
            location.href = "/Task/Index";
        });
    }


    $scope.$watch('projectSelected.Id + serverSelected.Id', function () {
        getDomain($scope.projectSelected, $scope.serverSelected);
    }, true);

    (function() {
        $scope.Query();

        getServerData();
        getProjectList();

    })();

});