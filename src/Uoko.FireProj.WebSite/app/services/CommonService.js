fireproj.service("CommonService", function ($http) {

    this.getProjectBranch = function (id, successCallBack) {
        $http.get("{0}/api/v3/projects/{1}/repository/branches?private_token={2}".Format(Global.GitLabUrl,id,Global.GitLabToken)).success(function (data) {
            if (successCallBack != undefined) {
            //排除pre和master分支
                data=data.filter(function (branch) {
                    return branch.name.toLowerCase() != "pre" && branch.name.toLowerCase() != "master";
                });
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        
        });
    };
    this.getAllUsers = function (successCallBack) {
        $http.get("/api/UserApi").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getSingleUser = function (userId,successCallBack) {
        $http.get("{0}/api/v3/users/{1}/?private_token={2}".Format(Global.GitLabUrl, userId, Global.GitLabToken)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getProjectTriggers = function (id, successCallBack) {
        $http.get("{0}/api/v3/projects/{1}/triggers?private_token={2}".Format(Global.GitLabUrl, id, Global.GitLabToken)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
});