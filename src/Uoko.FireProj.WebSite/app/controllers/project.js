//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
    $scope.isShowForm = false;
    $scope.pageSize = 10;
    $scope.currentPage = 1;
    $scope.items = [];
    $scope.totalItems = 0;//总数
    //查询项目
    $scope.Query = function () {
        var params= {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize
        }
        ProjectService.getByPage(params).success(function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    //删除事件
    $scope.delete = function (item) {
        ProjectService.delete(item.Id).success(function (data) {
            formSubmitSuccessClick("refresh");
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    $scope.Init = function () {
        $scope.Query();
    }

    $scope.Init();
});


