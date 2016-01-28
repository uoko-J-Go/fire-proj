
//创建一个ProjectService
fireproj.service("ProjectService", function ($http) {
    //列表分页
    this.getByPage = function (params) {
        return $http.get("/api/ProjectApi?Offset={0}&Limit={1}".Format(params.offset,params.limit));
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

    //编辑事件
    $scope.edit = function (item) {
        ProjectService.getById(item.Id).success(function (data) {
            $scope.formTile = "项目编辑";
            $scope.isShowForm = true;
            $scope.model = data;
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

    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        var gitlabInfo = JSON.parse(model.Project);
        model.ProjectRepo = gitlabInfo.http_url_to_repo;
        model.ProjectId = gitlabInfo.id;
        model.ProjectGitlabName = gitlabInfo.name;
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
    $scope.Init = function () {
        $scope.Query();
    }

    $scope.Init();
});


