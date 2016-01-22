
fireproj.service("TaskService", function ($http) {

    this.CreateTask = function (task,successCallBack) {
        $http.post("/api/TaskApi/Create",task).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
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
    this.getProjectBranch = function (id, successCallBack) {
        $http.get("http://gitlab.uoko.ioc:12015/api/v3/projects/" + id + "/repository/branches?private_token=JX4Gb7W_gfp7PdzpBjpG").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
});

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService) {
    $scope.taskInfo = {
        TaskName: "",
        Project: {},
        Branch: "",
        DeployEnvironment: "",
        DeployIP: "",
        DeployAddress: "",
        SiteName: "",
        CheckUsers: [],
        NoticeUses: [],
        TaskDesc:""
    };
    $scope.projectList = [];
    $scope.branchList = ["Dev", "Master"];
    $scope.environmentList = [{ Id: 1, Name: "IOC环境" }, { Id: 2, Name: "Pre环境" }, { Id: 3, Name: "生产环境" }];
    $scope.serverList = ["192.168.200.26", "192.168.200.28", "192.168.200.29"];
    $scope.siteList = ["ids.uoko.ioc","sso.uoko.ioc", "etadmin.uoko.ioc"];
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
        var project = $scope.taskInfo.Project;
        if (typeof project == "string") {
            $scope.taskInfo.Project = JSON.parse(project);
        } 
        TaskService.CreateTask($scope.taskInfo, function() {
            location.href = "/Task/Index";
        });
    }

    //根据项目Id或者分支列表
    $scope.getBranch = function (project) {
        project = JSON.parse(project)
        ProjectService.getProjectBranch(project.ProjectId, function (data) {
            $scope.branchList = data;
        });
    }

    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }
    
    $scope.Init = function () {
        $scope.GetProjectList();
    }

    $scope.Init();
});