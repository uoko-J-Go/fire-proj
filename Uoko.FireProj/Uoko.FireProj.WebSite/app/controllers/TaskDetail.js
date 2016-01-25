
fireproj.service("TaskService", function ($http) {
    //根据Id获取表单信息
    this.getById = function (id) {
        return $http.get("/api/TaskApi/" + id + "/ById");
    };
});

fireproj.controller("TaskController", function ($scope, $http, TaskService) {

    var id = $("#id").val();
    if (typeof id != "") {
        TaskService.getById(id).success(function (data) {
            $scope.taskInfo = data;
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    }


    $scope.Cancel = function () {
        location.href = "/Task/Index";
    }


});