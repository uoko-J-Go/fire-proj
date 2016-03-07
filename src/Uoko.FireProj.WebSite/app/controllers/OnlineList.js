fireproj.controller("OnlineListController", function($scope, $http, $uibModal, TaskService, ProjectService) {

    $scope.projects = [];
    $scope.servers = [];
    $scope.domains = [];

    $scope.onlineTaskInfo = {};

    $scope.pageSize = 10;
    $scope.currentPage = 1;

    $scope.taskInfos = [];
    $scope.totalItems = 0; //总数
    $scope.queryType = 0; // 所有
    $scope.projectSelected = null;
    $scope.serverSelected = null;


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
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        if (typeof server == "string") {
            server = JSON.parse(server);
        }
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
            $scope.totalItems = data.total;
            var tasks = data.rows;

            $scope.taskInfos = tasks;
        });
    }


    $scope.Deploy = function(task) {
        $scope.param = {
            taskId: task.TaskInfo.Id,
        };
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/Deploy.html',
            controller: 'DeployController',
            resolve: {
                param: function() {
                    return $scope.param;
                }
            }
        });
    }

    $scope.$watch('projectSelected + serverSelected', function() {
        getDomain($scope.onlineTaskInfo.Project, $scope.onlineTaskInfo.Server);
    });

    (function() {
        $scope.Query();

        getServerData();
        getProjectList();

    })();

});