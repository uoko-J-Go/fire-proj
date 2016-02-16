fireproj.controller("ServerController", function ($scope, $http, ServerService) {
    
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.items = [];
    $scope.totalItems = 0;//总数
    //查询项目
    $scope.Query = function () {
        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize
        }
        ServerService.GetServerByPage(params, function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        });

    }
    
    $scope.Init = function () {
        $scope.Query();
    }

    $scope.Init();
});