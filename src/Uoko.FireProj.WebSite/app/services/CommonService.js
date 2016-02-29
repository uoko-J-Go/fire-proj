fireproj.service("CommonService", function ($http) {

    this.getProjectBranch = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/" + id + "/repository/branches?private_token=D3MR_rnRZK4xWS-CtVho").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getAllUsers = function (successCallBack) {
        $http.get('http://gitlab.uoko.ioc:12015/api/v3/users?private_token=D3MR_rnRZK4xWS-CtVho').success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getSingleUser = function (userId,successCallBack) {
        $http.get('http://gitlab.uoko.ioc:12015/api/v3/users/' + userId + '/?private_token=D3MR_rnRZK4xWS-CtVho').success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getProjectTriggers = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/" + id + "/triggers?private_token=D3MR_rnRZK4xWS-CtVho").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    //触发项目
    this.TriggerBuild = function (taskInfo, successCallBack) {
        var params = {
            "slnFile":taskInfo.ProjectSlnName,
            "csProjFile": taskInfo.Project.ProjectCsprojName,
            "iisSiteName": taskInfo.SiteName,
            "pkgDir": taskInfo.PackageDir,
            "msDeployUrl": "https://" + taskInfo.DeployIP + ":8172/msdeploy.axd",
            "publishConfiguration": "Release" //taskInfo.DeployEnvironmentName
        };
        this.getProjectTriggers(taskInfo.Project.ProjectId, function (data) {
            var triggers = data;
            if (triggers.length <= 0) {
                console.error("无Trigger 请到GitLab中配置");
                return;
            }
            $http.post('http://gitlab.uoko.ioc:12015/api/v3/projects/' + taskInfo.Project.ProjectId + '/trigger/builds?private_token=D3MR_rnRZK4xWS-CtVho', { token: triggers[0].token, ref: taskInfo.Branch, variables: params }).success(function (data) {
                if (successCallBack != undefined) {
                    successCallBack(data);
                }
            }).error(function (data) {
                //错误处理
            });

        });
       
    };
});