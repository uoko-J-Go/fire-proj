fireproj.service("CommonService", function ($http) {

    this.getProjectBranch = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/{0}/repository/branches?private_token=D3MR_rnRZK4xWS-CtVho".Format(id)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        
        });
    };
    this.getAllUsers = function (successCallBack) {
        //$http.get("http://gitlab.uoko.ioc:12015/api/v3/users?private_token=D3MR_rnRZK4xWS-CtVho").success(function (data) {
        $http.get("/api/UserApi").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getSingleUser = function (userId,successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/users/{0}/?private_token=D3MR_rnRZK4xWS-CtVho".Format(userId)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.getProjectTriggers = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/{0}/triggers?private_token=D3MR_rnRZK4xWS-CtVho".Format(id)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    //触发项目
    this.TriggerBuild = function (buildInfo, successCallBack) {
        var params = {
            "slnFile": buildInfo.ProjectSlnName,
            "csProjFile": buildInfo.ProjectCsprojName,
            "iisSiteName": buildInfo.SiteName,
            "pkgDir": buildInfo.PackageDir,
            "msDeployUrl": "https://" + buildInfo.DeployIP + ":8172/msdeploy.axd",
            "useConfig": "Release", //taskInfo.DeployEnvironmentName,
            "Target": buildInfo.Target//"Deploy-To-IOC"
        };
        this.getProjectTriggers(buildInfo.RepoId, function (data) {
            var triggers = data;
            if (triggers.length <= 0) {
                console.error("无Trigger 请到GitLab中配置");
                return;
            }
            $http.post("http://gitlab.uoko.ioc:12015/api/v3/projects/{0}/trigger/builds?private_token=D3MR_rnRZK4xWS-CtVho".Format(buildInfo.RepoId ), { token: triggers[0].token, ref: buildInfo.Branch, variables: params }).success(function (data) {
                if (successCallBack != undefined) {
                    successCallBack(data);
                }
            }).error(function (data) {
                //错误处理
            });

        });
       
    };

    //根据选择环境,加载最新的通知,测试相关人
    this.loadCheckUserList = function (userInfo,successCallBack) {
        if (userInfo == undefined) {
            return;
        }
        var data = userInfo.split(',');
        var userData = new Array();
        for (var i = 0; i < data.length; i++) {
            var item = data[i].split('-');
            userData.push({
                UserId: item[0],
                NickName: GetUserName(item[0]),
            })
        }
        if (successCallBack != undefined) {
            successCallBack(data);
        }
        //$scope.taskInfo.CheckUsers = userData;
    }
    this.loadNoticeUserList = function (userInfo, successCallBack) {
        if (userInfo == undefined) {
            return;
        }
        var userData = new Array();
        var data = userInfo.split(',');

        for (var i = 0; i < data.length; i++) {
            userData.push({
                UserId: data[0],
                NickName: GetUserName(data[0]),
            })
        }
        if (successCallBack != undefined) {
            successCallBack(data);
        }
        //$scope.taskInfo.NoticeUsers = userData;
    }
    this.GetUserName = function (userId) {
        var userData = AllUsers();
        for (var i = 0; i < userData.length; i++) {
            if (userId == userData[i].UserId) {
                return userData[i].NickName;
            }
        }
    };
    
});