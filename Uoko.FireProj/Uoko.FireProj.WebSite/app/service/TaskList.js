
fireproj.service("TaskService", function ($http) {
    
});

fireproj.controller("TaskController", function ($scope, $http, TaskService) {
    $scope.tableOptions = {
        url: '/api/TaskApi',
        columns: [
            { field: 'Id', title: 'Id', align: 'center', width: 50 },
            { field: 'TaskName', title: '任务名称', align: 'center' },
            { field: 'DeployEnvironment', title: '环境', align: 'center' },
            { field: 'Branch', title: '分支', align: 'center' },
            { field: 'Status', title: '状态', align: 'center' },
            
            { field: 'TaskDesc', title: '任务描述', align: 'center' },
            {
                title: '操作', align: 'center', width: 200, formatter: function (value, row, index) {
                    return [
                        '<a class=" editor" ng-click="edit(' + row.Id + ')" title="编辑">',
                            '编辑',
                        '</a>',

                        '<a class=" delete" ng-click="delete(' + row.Id + ')" title="删除">',
                            '删除',
                        '</a>'].join('');
                }
            },
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
    };

});


