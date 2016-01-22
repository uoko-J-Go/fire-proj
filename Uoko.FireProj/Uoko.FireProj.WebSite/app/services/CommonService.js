fireproj.service("CommonService", function ($http) {

    this.getAllUsers = function (successCallBack) {
        $http.get('http://gitlab.uoko.ioc:12015/api/v3/users?private_token=JX4Gb7W_gfp7PdzpBjpG').success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});