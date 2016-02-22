fireproj.service("ServerService", function ($http) {

    this.GetServerByPage = function (params, successCallBack) {
        $http.get("/api/ServerApi?Offset={0}&Limit={1}".Format(params.offset, params.limit)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };

    this.Create = function (model, successCallBack) {
        $http.post("/api/ServerApi/Create", model).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.Update = function (model, successCallBack) {
        $http.post("/api/ServerApi/Update", model).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.getById = function (serverId, successCallBack) {
        $http.get("/api/ServerApi/" + serverId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.delete = function (serverId, successCallBack) {
        $http.post("/api/ServerApi/Delete/" + serverId).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});