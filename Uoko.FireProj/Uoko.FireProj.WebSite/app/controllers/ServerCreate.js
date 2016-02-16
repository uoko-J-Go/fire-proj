
//创建一个ProjectController
fireproj.controller("ServerController", function ($scope, $http, ServerService, TaskService) {

    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        var gitlabInfo = model.Project;
       
        ServerService.Create(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });

   

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
});


