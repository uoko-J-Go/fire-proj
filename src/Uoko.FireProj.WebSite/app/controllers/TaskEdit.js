﻿

fireproj.controller("TaskController", function ($scope, $http, $uibModal, TaskService, ProjectService,CommonService) {
    $scope.taskInfo = {};
    $scope.projectList = [];
    $scope.branchList = [];
    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });
    $scope.GetProjectList = function () {
        ProjectService.getAllProject(function (data) {
            $scope.projectList = data;
        });
    };
    $scope.GetTaskInfo = function () {
        var taskId = $("#taskIdParam").val();
        TaskService.GetTaskInfo(taskId, function (data) {
            $scope.taskInfo = data;
            $scope.getBranch($scope.taskInfo.Project);
            $scope.GetAllUserDetail($scope.taskInfo.CheckUsers, 0);
            $scope.GetAllUserDetail($scope.taskInfo.NoticeUses, 0);

        });
    }
    $scope.GetAllUserDetail = function (userList, index) {
        if (index < userList.length) {
            var user = userList[index];
            CommonService.getSingleUser(user.Id, function (data) {
                userList.splice(index, 1, data);
                if (index < userList.length) {
                    ++index;
                    $scope.GetAllUserDetail(userList, index);
                }
            });
        }
    }
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
        if (!isValid) {
            bootbox.alert("表单验证未通过");
            return;
        }
        var project = $scope.taskInfo.Project;
        if (typeof project == "string") {
            $scope.taskInfo.Project = JSON.parse(project);
        }
        TaskService.UpdateTask($scope.taskInfo, function () {
            location.href = "/Task/Index";
        });
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
        $scope.GetTaskInfo();
        $scope.GetProjectList();
    }

    $scope.Init();
});