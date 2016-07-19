
fireproj.controller("TaskController", function ($scope, $http, $uibModal,TaskService, ProjectService, CommonService) {
    $scope.projectList = [];
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };
    $scope.pageSize = 10;
    $scope.currentPage = 1;

    $scope.taskInfos = [];
    $scope.totalItems = 0;//总数
    $scope.queryType = 0; // 所有
    

    //查询项目
    $scope.Query = function (showType) {

        if (showType) {
            $scope.queryType = showType;
        }

        var params = {
            offset: $scope.pageSize * ($scope.currentPage - 1),
            limit: $scope.pageSize,
            ShowType: $scope.queryType,
            Search: $scope.search,
        }

        TaskService.GetTaskByPage(params,function (data) {
            $scope.totalItems = data.total;
            var tasks = data.rows;
            $scope.taskInfos = tasks;
        });
    }

    $scope.Deploy = function (task) {
        $scope.param = {
            taskId: task.TaskInfo.Id,
        };
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/Deploy.html',
            controller: 'DeployController',
            resolve: {
                param: function () {
                    return $scope.param;
                }
            }
        });
    }

    $scope.Init = function () {
        $scope.Query();
        $scope.GetProjectList();
    }
    $scope.Init();
});


