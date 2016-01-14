
//创建一个ProjectService
fireproj.service("ProjectService", function ($http) {
    //列表分页
    this.getByPage = function (param) {
        return $http.get("/api/ProjectAPI/" + param + "");
    };

    //根据Id获取表单信息
    this.getById = function (id) {
        return $http.get("/api/ProjectAPI/" + id + "/ById");
    };

    //新增
    this.post = function (company) {
        return $http.post("/api/ProjectAPI", company);
    };

    //修改
    this.put = function (id, company) {
        return $http.put("/api/ProjectAPI/" + id + "", company);
    };

    //删除
    this.delete = function (id) {
        return $http.delete("/api/ProjectAPI/" + id + "");
    };
});



//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
    $scope.isShowForm = false;
    //分页配置
    $scope.tableOptions = {
        url: '/api/ProjectAPI',
        columns: [
            { field: 'Id', title: 'Id', align: 'center', width: 280 },
            { field: 'ProjectName', title: '项目名称', align: 'center' },
            { field: 'SiteNmae', title: '站点名称', align: 'center' },
            { field: 'ProjectRepo', title: 'Repo', align: 'center' },
            { field: 'ProjectDesc', title: '项目描述', align: 'center' },
            {
                title: '操作', align: 'center', width: 200, formatter: function (value, row, index) {
                    return [
                        '<a class="btn btn-primary editor" ng-click="edit(' + row.Id + ')" title="编辑">',
                            '编辑',
                        '</a>',

                        '<a class="btn btn-primary delete" ng-click="delete(' + row.Id + ')" title="删除">',
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

    //编辑事件
    $scope.edit = function (id) {
        ProjectService.getById(id).success(function (data) {
            $scope.formTile = "项目编辑";
            $scope.isShowForm = true;
            $scope.company = data;
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    //删除事件
    $scope.delete = function (id) {
        ProjectService.delete(id).success(function (data) {
            formSubmitSuccessClick();
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }

    //声明表单提交事件
    $scope.SubmitForm = function (company) {
        if (typeof company.Id == "undefined") {
            ProjectService.post(company).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
        else {
            ProjectService.put(company.Id, company).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
    };

    $scope.OpenForm = function () {
        $scope.formTile = "项目新增";
        $scope.isShowForm = true;
    }

    $scope.CancelForm = function () {
        $scope.isShowForm = false;
    }

});


