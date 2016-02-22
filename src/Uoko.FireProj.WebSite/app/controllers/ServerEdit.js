fireproj.controller("ServerController", function ($scope, $http, ServerService,TaskService) {
    
    $scope.LoadForm = function () {
        var serverId = $("#id").val();
        ServerService.getById(serverId, function (data) {
            $scope.model = data;
        });
    }

    $scope.GetEnvironment = function () {
        TaskService.GetEnvironment(function (data) {
            $scope.environmentList = data;
            $scope.LoadForm();
        });
    }
    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        

        ServerService.Update(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };
    $scope.Init = function () {
        $scope.GetEnvironment();
    }


    $scope.Init();
});