
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
                title: '操作', align: 'center', width: 200, formatter: function (value, row, index) {
                    return [
                        '<a class="btn btn-primary editor" ng-click="Deploy(' + row.Id + ')" title="编译部署">',
                            '编译部署',
                        '</a>',

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
            CommonService.TriggerBuild(taskInfo.Project.ProjectId, function (data) {
                bootbox.alert("已经成功发起部署任务，点击详细进行查看!");
            });
        });
       
    }

    $scope.ShowDetail = function (taskId) {
        location.href = "/Task/Detail?taskId=" + taskId;
    }
    $scope.Init = function () {
        $scope.GetProjectList();
    }

    $scope.Init();
});


