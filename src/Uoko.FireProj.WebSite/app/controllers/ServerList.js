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
    
    //删除事件
    $scope.delete = function (item) {
        ServerService.delete(item.Id,function(data){
            formSubmitSuccessClick("refresh");
        });
    }

    $scope.Init = function () {
        $scope.Query();
    }

    $scope.Init();
});