
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
fireproj.service("TaskService", function ($http) {

});
fireproj.service("ProjectService", function ($http) {
    this.getAllProject = function (successCallBack) {
        $http.get("/api/ProjectApi/GetAll").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {
        TaskName: "",
        Project: { Id: 1, ProjectName: "单点登录" },
        Branch: "",
        DeployEnvironment: "",
        DeployIP: "",
        SiteName: "",
        CheckUsers: [{ Id: 1, Name: "山姆", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182836_8MEhe.jpeg" }],
        NoticeUses: [{ Id: 2, Name: "庆攀", Profile: "http://img5.duitang.com/uploads/item/201406/07/20140607182730_sNGAS.thumb.700_0.jpeg" }],
        TaskDesc:""
    };
    $scope.projectList = [];
    $scope.branchList = ["Dev", "Master"];
    $scope.environmentList = [{ Id: 1, Name: "IOC环境" }, { Id: 2, Name: "Pre环境" }, { Id: 3, Name: "生产环境" }];
    $scope.serverList = ["192.168.200.26", "192.168.200.28", "192.168.200.29"];
    $scope.siteList = ["sso.uoko.ioc", "etadmin.uoko.ioc"];
    $scope.GetProjectList = function() {
        ProjectService.getAllProject(function(data) {
            $scope.projectList = data;
        });
    };
    //选择审核人
    $scope.selectCheckUser = function () {
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/SelectUser.html',
            controller: 'SelectUserController',
            resolve: {
                selectedUsers: function () {
                    return $scope.taskInfo.CheckUsers;
                }
            }
        });

        modalInstance.result.then(function (selectedUsers) {
            $scope.taskInfo.CheckUsers = selectedUsers;
        });
    }
    //选择相关人
    $scope.selectNoticeUser = function () {
        var modalInstance = $uibModal.open({
            templateUrl: '/app/modals/SelectUser.html',
            controller: 'SelectUserController',
            resolve: {
                selectedUsers: function () {
                    return $scope.taskInfo.NoticeUses;
                }
            }
        });

        modalInstance.result.then(function (selectedUsers) {
            $scope.taskInfo.NoticeUses = selectedUsers;
        });
    }
    //移除审核人
    $scope.removeCheckUser = function (index) {
        $scope.taskInfo.CheckUsers.splice(index, 1);
        $scope.$evalAsync();
    }
    //移除相关人
    $scope.removeNoticeUser = function (index) {
        $scope.taskInfo.NoticeUses.splice(index, 1);
        $scope.$evalAsync();
    }
    $scope.Save = function () {
        alert(JSON.stringify($scope.taskInfo));
    }
    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }
    
    $scope.Init = function () {
        $scope.GetProjectList();
    }

    $scope.Init();
});