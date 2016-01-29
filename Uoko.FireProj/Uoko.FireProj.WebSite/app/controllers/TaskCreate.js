﻿

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService, CommonService) {
    $scope.taskInfo = {
        TaskName: "",
        Project: null,
        Branch: "",
        DeployEnvironment: "",
        DeployIP: "",
        DeployAddress: "",
        SiteName: "",
        CheckUsers: [],
        NoticeUses: [],
        TaskDesc:""
    };
    ///环境服务器IP List,后面优化
    $scope.DeployIPList = [
      { Id: 1, IP: "192.168.1.100" },
      { Id: 2, IP: "192.168.1.101" },
    ];
    $scope.projectList = [];
    $scope.branchList = [];
    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });
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
    $scope.Save = function (isValid) {
        //if (!isValid) {
        //    bootbox.alert("表单验证未通过");
        //    return;
        //}
        var project = $scope.taskInfo.Project;
        if (typeof project == "string") {
            $scope.taskInfo.Project = JSON.parse(project);
        } 
        TaskService.CreateTask($scope.taskInfo, function() {
            location.href = "/Task/Index";
        });
    }

    //选择环境change事件
    $scope.EnvironmentChange = function (project,environmentId) {
        if (environmentId == 0) {
            if (typeof project == "string") {
                project = JSON.parse(project);
            }
            TaskService.GetResourceList(project.Id, environmentId, function (data) {
                $scope.resourceList = data;
            });
        }
    }
    //根据项目Id或者分支列表
    $scope.getBranch = function (project) {
        if (typeof project == "string") {
            project = JSON.parse(project);
        }
        CommonService.getProjectBranch(project.ProjectId, function (data) {
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