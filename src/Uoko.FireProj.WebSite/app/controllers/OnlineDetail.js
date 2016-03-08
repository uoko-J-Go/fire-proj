fireproj.controller("OnlineDetailController", function($scope, $http, $uibModal, TaskService, ProjectService) {

    $scope.onlineTaskId = $("#onlineTaskId").val();

    function getProjectInfo(projectId, buildId) {
        if (projectId && buildId) {
            ProjectService.getById(projectId).success(function(project) {
                if (project) {
                    $scope.buildInfoLink = project.ProjectRepo.replace(".git", "/builds/" + buildId);
                }
            });
        }
    }

    //查询项目
    function queryDetails() {

        var params = {
            onlineTaskId: $scope.onlineTaskId
        };

        TaskService.GetOnlineDetail(params, function(data) {
            $scope.onlineDetails = data;
            getProjectInfo(data.OnlineTask.ProjectId, data.OnlineTask.BuildId);
        });
    };


    $scope.retryDeploy = function() {
        var params = {
            onlineTaskId: $scope.onlineTaskId
        };
        TaskService.RetryDeployOnline(params, function(data) {
            location.reload();
        });
    };

    (function() {
        queryDetails();
    })();
});