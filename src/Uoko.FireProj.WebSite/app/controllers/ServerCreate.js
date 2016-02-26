
//创建一个ProjectController
fireproj.controller("ServerController", function ($scope, $http, ServerService, TaskService,ProjectService) {
    var IISData = new Array();

    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        model.IISData = IISData;
        ServerService.Create(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });

    
    ProjectService.getAllProject(function(data) {
        $scope.projectList = data;
    });

    $scope.Append = function (model) {
        var info = {
            "Name": model.Domain,
            "SiteName": model.SiteName,
            "ProjectId": model.ProjectId
        }
        IISData.push(info);
    }

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
});


