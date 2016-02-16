
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

    //选择项目change事件
    $scope.GetProjectInfo = function (project) {
        if (typeof project == 'string') {
            project = JSON.parse(project);
        }
        ProjectService.getGitLabSln(project.id, function (data) {
            var fileList = new Array();
            for (var i = 0; i < data.length; i++) {
                if ((/\.[^\.]+$/.exec(data[i].name)) == ".sln") {
                    $scope.model.ProjectSlnName = data[i].name;//项目sln名称
                }
                if (data[i].type == "tree") {
                    fileList.push(data[i]);
                }
            }
            $scope.menuList = fileList;
          
        });
    }

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
});


