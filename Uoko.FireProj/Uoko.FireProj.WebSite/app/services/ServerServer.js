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
});