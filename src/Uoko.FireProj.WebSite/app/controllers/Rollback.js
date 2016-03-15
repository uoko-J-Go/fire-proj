fireproj.controller("RollbackController", function($scope, $http, $uibModal, TaskService, ProjectService) {
    $scope.IsSubmiting = false;
    $scope.projects = [];
    $scope.onlineTasks = [];
    $scope.getProjectList = function () {

        ProjectService.getAllProject(function(data) {
            $scope.projects = data.filter(function(proj) {
                return proj.CreatorId == $("#userId").val();
            });
        });

    };

    $scope.GetOnlineTaskRollbackAble= function () {
        var projectId = 0;
        if ($scope.projectSelected) {
            projectId = $scope.projectSelected.Id;
        }
        TaskService.GetOnlineTaskRollbackAble(projectId,function (data) {
            $scope.onlineTasks = data;
        });

    };
    $scope.Rollback = function (isValid) {

         if(!isValid) {
            bootbox.alert("表单验证未通过");
            return;
            }
            if ($scope.IsSubmiting) {
                return;
            }
        $scope.IsSubmiting = true;
        bootbox.confirm({
            title:"回滚确认",
            buttons: {
                confirm: {
                    label: '确认'
                },
                cancel: {
                    label: '取消'
                }
            },
            message: '你确认要回滚当前本版么？',
            callback: function (result) {
                if (result) {
                    var rollbackTaskInfo = {
                        ProjectId: $scope.projectSelected.Id,
                        ProjectName: $scope.projectSelected.ProjectName,
                        FromVersion: $scope.taskSelected.OnlineVersion,
                        ToVersion: "before-" + $scope.taskSelected.OnlineVersion
                    };

                    TaskService.Rollback(rollbackTaskInfo, function (data) {
                        $scope.IsSubmiting = false;
                        window.location.reload();

                    }, function (data) {
                        $scope.IsSubmiting = false;
                    });
                } else {
                    $scope.IsSubmiting = false;
                }
            }
        });
       

        
       

    };

    $scope.getProjectList();
});