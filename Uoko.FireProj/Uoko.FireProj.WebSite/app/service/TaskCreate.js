
fireproj.service("TaskService", function ($http) {

});
fireproj.service("ProjectService", function ($http) {
    //获取gitlab所有项目信息
    this.getAllProject = function (successCallBack) {
        $http.get("api/ProjectApi/GetAll").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});

fireproj.controller("TaskController", function ($scope, $http, TaskService, ProjectService) {
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
            $scope.ProjectList = data;
        });
    };

    //选择审核人
    $scope.selectCheckUser = function () {
        $("#selectUserModal").modal('show');
    }
    //选择相关人
    $scope.selectNoticeUser = function () {
        $("#selectUserModal").modal('show');
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