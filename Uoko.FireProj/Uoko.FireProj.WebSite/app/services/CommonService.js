fireproj.service("CommonService", function ($http) {

    this.getProjectBranch = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/" + id + "/repository/branches?private_token=gQM5yK7r32iPsDcmNy8-").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    this.getAllUsers = function (successCallBack) {
        $http.get('http://gitlab.uoko.ioc:12015/api/v3/users?private_token=gQM5yK7r32iPsDcmNy8-').success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    this.getSingleUser = function (userId,successCallBack) {
        $http.get('http://gitlab.uoko.ioc:12015/api/v3/users/' + userId + '/?private_token=gQM5yK7r32iPsDcmNy8-').success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    this.TriggerBuild = function (id,successCallBack) {
        var params = {
            "Solution": "UOKO.SSOAA.sln",
            "CsprojName": "UOKO.SSO.Server\UOKO.SSO.Server.csproj"
        }
        $http.post('http://gitlab.uoko.ioc:12015/api/v3/projects/'+id+'/trigger/builds?private_token=gQM5yK7r32iPsDcmNy8-', { token: "f51b995894982acda6cc24accbb780", ref: "master", variables: params }).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});