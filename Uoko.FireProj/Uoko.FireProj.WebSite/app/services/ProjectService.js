fireproj.service("ProjectService", function ($http) {
    this.getAllProject = function (successCallBack) {
        $http.get("/api/ProjectApi/GetAll").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    //获取任务的项目
    this.getByTaskId = function (taskId, successCallBack) {
        return $http.get("/api/ProjectApi/GetByTaskId/" + taskId).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    //列表分页
    this.getByPage = function (params) {
        return $http.get("/api/ProjectApi?Offset={0}&Limit={1}".Format(params.offset, params.limit));
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

    this.getGitLabSln = function (projectId,successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/" + projectId + "/repository/tree?private_token=JX4Gb7W_gfp7PdzpBjpG").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

});