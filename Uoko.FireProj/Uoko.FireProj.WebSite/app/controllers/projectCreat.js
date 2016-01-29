
//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
  
    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        var gitlabInfo = model.Project;
        if (typeof gitlabInfo == 'string') {
            gitlabInfo = JSON.parse(gitlabInfo);
        }
        model.ProjectRepo = gitlabInfo.http_url_to_repo;
        model.ProjectId = gitlabInfo.id;
        model.ProjectGitlabName = gitlabInfo.name;
        ProjectService.post(model).success(function (data) {
            location.href = "/Project/Index";
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    };

    //获取gitlab所有项目信息
    ProjectService.getGitlbProject().success(function (data) {
        $scope.ProjectList = data;
    }).error(function (data) {
        formSubmitFailClick(data);
    });

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
});


