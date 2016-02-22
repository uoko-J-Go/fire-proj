
//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
    $scope.LoadForm = function () {
        var projectId = $("#id").val();
        ProjectService.getById(projectId).success(function (data) {
            $scope.model = data;
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    //获取gitlab所有项目信息
    $scope.getGitlbProject = function () {
        ProjectService.getGitlbProject().success(function (data) {
            $scope.ProjectList = data;
            $scope.LoadForm();
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
    $scope.init = function () {
        $scope.getGitlbProject();
    }
    $scope.init();
});


