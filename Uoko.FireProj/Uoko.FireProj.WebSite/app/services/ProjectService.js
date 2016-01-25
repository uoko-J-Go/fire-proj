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
 
});