fireproj.service("DomainService", function ($http) {

    this.GetDomainByPage = function (params, successCallBack) {
        $http.get("/api/DomainResourceApi?Offset={0}&Limit={1}".Format(params.offset, params.limit)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    
    
    
});