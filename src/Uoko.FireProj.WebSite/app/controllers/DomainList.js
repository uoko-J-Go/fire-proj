fireproj.controller("DomainController", function ($scope, $http, DomainService,TaskService) {
    
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
        DomainService.GetDomainByPage(params, function (data) {
            $scope.totalItems = data.total;
            $scope.items = data.rows;
        });

    }
    ///释放资源操作
    $scope.ReleaseDomain = function (task) {
        var userId = $("#userId").val();
        if (userId != task.TaskCreatorId) {
            bootbox.alert("只有该任务创建人才允许进行此操作!<a target='_blank' class='text-primary' href='/Task/Detail?taskId=" + task.TaskId + "'>查看</a>");
        }
        else {
            TaskService.ReleaseDomain(task.TaskId, function (data) {
                formSubmitSuccessClick("refresh");
            });
        }
    
    }
    $scope.Init = function () {
        $scope.Query();
        
    }

    $scope.Init();
});