
//创建一个ProjectService
fireproj.service("ProjectService", function ($http) {
    //列表分页
    this.getByPage = function (param) {
        return $http.get("/api/ProjectApi/" + param + "");
    };

    //根据Id获取表单信息
    this.getById = function (id) {
        return $http.get("/api/ProjectApi/" + id + "/ById");
    };

    //新增
    this.post = function (model) {
        return $http.post("/api/ProjectApi", model);
    };

    //修改
    this.put = function (id, model) {
        return $http.put("/api/ProjectApi/" + id + "", model);
    };

    //删除
    this.delete = function (id) {
        return $http.delete("/api/ProjectApi/" + id + "");
    };

    //获取gitlab所有项目信息
    this.getGitlbProject = function () {
        return $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects?private_token=JX4Gb7W_gfp7PdzpBjpG");
    };
});



//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
    $scope.isShowForm = false;
    //分页配置
    $scope.tableOptions = {
        url: '/api/ProjectApi',
        columns: [
            { field: 'selected', checkbox: true, align: 'center', width: 200 },
            { field: 'Id', title: 'Id', align: 'center', width: 50 },
            { field: 'ProjectName', title: '项目名称', align: 'center' },
            { field: 'ProjectFileName', title: '项目文件', align: 'center' },
            { field: 'ProjectRepo', title: '项目地址', align: 'center' },
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
            $scope.model = data;
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
    $scope.SubmitFrom = function (model) {
        if (typeof model.Id == "undefined") {
            ProjectService.post(model).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
        else {
            ProjectService.put(model.Id, model).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
    };

    //获取gitlab所有项目信息
    ProjectService.getGitlbProject().success(function (data) {
        $scope.ProjectList = data;
    }).error(function (data) {
        formSubmitFailClick(data);
    });

    $scope.OpenForm = function () {
        $scope.formTile = "项目新增";
        $scope.isShowForm = true;
        $scope.model = null;
    }

    $scope.CancelForm = function () {
        $scope.isShowForm = false;
    }

});


