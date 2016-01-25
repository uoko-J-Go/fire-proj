
fireproj.service("TaskService", function ($http) {
    
});
fireproj.service("ProjectService", function ($http) {
    //获取gitlab所有项目信息
    this.getAllProject = function (successCallBack) {
        $http.get("/api/ProjectApi/GetAll").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
});
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
                       '<a class="btn btn-primary delete" href="/Task/Detail/' + row.Id + '" title="详情">',
                            '详情',
                        '</a>',

                        '<a class="btn btn-primary editor" ng-click="Deploy(' + row.Id + ')" title="编译部署">',
                            '编译部署',
                        '</a>',

                        '<a class="btn btn-primary delete" ng-click="CommitToTest(' + row.Id + ')" title="提交测试">',
                            '提交测试',
                        '</a>',

                        '<a class="btn btn-primary delete" href="/Task/Logs/' + row.Id + '" title="任务记录">',
                            '任务记录',
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
    $scope.Deploy=function(id) {
        CommonService.TriggerBuild(function() {
            

        });
    }

    $scope.CommitToTest = function () {

    }
    $scope.Init = function () {
        $scope.GetProjectList();
    }

    $scope.Init();
});


