
fireproj.controller("TaskController", function ($scope, $http, TaskService, ProjectService, CommonService) {
    $scope.projectList = [];
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };

    $scope.tableOptions = {
        url: '/api/TaskApi',
        columns: [
            { field: 'Id', title: 'Id', align: 'center', width: 50, visible: false, cardVisible: false, switchable: false },
            { field: 'TaskName', title: '任务名称', align: 'center' },
            { field: 'DeployEnvironment', title: '环境', align: 'center' },
            { field: 'Branch', title: '分支', align: 'center' },
            { field: 'Status', title: '状态', align: 'center' },
            
            { field: 'TaskDesc', title: '任务描述', align: 'center' },
            {
                title: '操作', align: 'center', width: 400, formatter: function (value, row, index) {
                    return [
                        '<a class="btn btn-primary editor" ng-click="Deploy(' + row.Id + ')" title="编译部署">',
                            '编译部署',
                        '</a>',

                        '<a class="btn btn-primary delete" ng-click="CommitToTest(' + row.Id + ')" title="提交测试">',
                            '提交测试',
                        '</a>',

                        '<a class="btn btn-primary delete" href="/Task/Logs/' + row.Id + '" title="任务记录">',
                            '任务记录',
                        '<a class="btn btn-primary delete" ng-click="ShowDetail(' + row.Id + ')" title="详细">',
                            '详细',
                        '</a>'].join('');
                }
            }
        ],
        search: true,
        showRefresh: true,
        showToggle: true,
        showColumns: true,
        showExport: true,
        minimumCountColumns: 2,
        showPaginationSwitch: true,
        pagination: true,
        idField: true,
        pageList: [10, 25, 50, 100],
        sidePagination: 'server'
    };
    $scope.Deploy = function (taskId) {
        TaskService.GetTaskInfo(taskId, function (data) {
            var taskInfo = data;
            CommonService.TriggerBuild(taskInfo, function (data) {
                bootbox.alert("已经成功发起部署任务，点击详细进行查看!", function () {
                    //todo 添加记录 状态更改,这里判断是否部署成功
                    var param = {
                        id:taskId,
                        Status: "Deployment"
                    };
                    TaskService.UpdateTaskStatus(param, function (data) {

                    });
                    // 保存部署记录

                });
                
            });
        });
    }

    $scope.ShowDetail = function (taskId) {
        location.href = "/Task/Detail?taskId=" + taskId;
    }
    $scope.Init = function () {
        $scope.GetProjectList();
    }
    ///提交测试,状态改为3测试中
    $scope.CommitToTest = function (taskId) {
        var param = {
            id: taskId,
            Status: 3
        };
        TaskService.UpdateTaskStatus(param, function (data) {

        });
    }

    $scope.Init();
});


