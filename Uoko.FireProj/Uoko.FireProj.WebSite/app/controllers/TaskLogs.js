
fireproj.controller("TaskController", function ($scope, $http) {
    var taskId = $("#id").val();
    //分页配置
    $scope.tableOptions = {
        url: '/api/TaskLogsApi',
        columns: [
            { field: 'Id', title: 'Id', align: 'center', width: 50 },
            { field: 'Branch', title: '代码分支', align: 'center' },
            { field: 'DeployEnvironment', title: '部署环境', align: 'center' },
            { field: 'DeployIP', title: '服务器IP', align: 'center' },
            { field: 'Status', title: '部署状态', align: 'center' },
            { field: 'CreateBy', title: '操作人', align: 'center' },
            { field: 'CreateDate', title: '操作时间', align: 'center' },
            
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
        sidePagination: 'server',
        queryParams: function (params) {
            params.taskId = taskId;
            return params;
        }
    };
});