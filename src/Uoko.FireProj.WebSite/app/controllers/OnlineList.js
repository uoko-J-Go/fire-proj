fireproj.controller("OnlineListController", function($scope, $http, $uibModal, TaskService, ProjectService) {

    $scope.projects = [];
    $scope.servers = [];
    $scope.domains = [];

    $scope.taskInfos = [];

    $scope.projectSelected = null;
    $scope.serverSelected = null;
    $scope.onlineVersion = null;


    function getProjectList() {
        ProjectService.getAllProject(function(data) {
            $scope.projects = data;
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
    $scope.Query = function() {

        var projectId = 0;
        if ($scope.projectSelected) {
            projectId = $scope.projectSelected.Id;
        }

        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            ProjectId: projectId,
        };

        TaskService.GetTasksNeedToBeOnline(params, function(data) {
            $scope.taskInfos = data;
        });
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


    $scope.$watch('projectSelected + serverSelected', function() {
        getDomain($scope.projectSelected, $scope.serverSelected);
    }, true);

    (function() {
        $scope.Query();

        getServerData();
        getProjectList();

    })();

});