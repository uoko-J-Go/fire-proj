
//创建一个ProjectController
fireproj.controller("ServerController", function ($scope, $http, ServerService, TaskService, ProjectService) {
    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        model.IISData = IISData;
        ServerService.Create(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });


    ProjectService.getAllProject(function (data) {
        $scope.projectList = data;
    });

    

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }

    //////////////////////////////////////////////////////////////////
    var i = 0;
    var IISData = new Array();
    $scope.Append = function (model) {
        var project = JSON.parse(model.project);
        var info = {
            "Index":i++,
            "Name": model.Domain,
            "SiteName": model.SiteName,
            "ProjectId": project.ProjectId,
            "ProjectName": project.ProjectName
        }
        
        if (model.Index != null) {
            IISData[model.Index] = info;
        }
        else {
            IISData.push(info);
        }
        $scope.items = IISData;
    }
    $scope.edit = function (item) {
        $scope.model.Index = item.Index;
        $scope.model.Domain = item.Name;
        $scope.model.SiteName = item.SiteName;
        $scope.model.ProjectId = item.ProjectId;
    };
    $scope.delete = function (item) {
        IISData.remove(item.Index);
        $scope.items = IISData;

    };

    Array.prototype.remove = function (dx) {
        if (isNaN(dx) || dx > this.length) { return false; }
        for (var i = 0, n = 0; i < this.length; i++) {
            if (this[i] != this[dx]) {
                this[n++] = this[i]
            }
        }
        this.length -= 1
    }


   
});


