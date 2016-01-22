
fireproj.service("DictionaryService", function ($http) {
    //根据Id获取表单信息
    this.getById = function (id) {
        return $http.get("/api/DictionaryApi/" + id + "/ById");
    };

    //获取所有父级字典
    this.getParent = function () {
        return $http.get("/api/DictionaryApi/Parent");
    };

    //新增
    this.post = function (model) {
        return $http.post("/api/DictionaryApi", model);
    };

    //修改
    this.put = function (id, model) {
        return $http.put("/api/DictionaryApi/" + id + "", model);
    };
});



fireproj.controller("DictionaryController", function ($scope, $http, DictionaryService) {
    $scope.ParentList;
    $scope.formTile = "字典新增";
   
    
    //获取gitlab所有项目信息
    DictionaryService.getParent().success(function (data) {
        $scope.ParentList = data;
    }).error(function (data) {
        formSubmitFailClick(data);
    });

    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        if (typeof model.Id == "undefined") {
            DictionaryService.post(model).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
        else {
            DictionaryService.put(model.Id, model).success(function (data) {
                formSubmitSuccessClick();
            }).error(function (data) {
                formSubmitFailClick(data);
            });
        }
    };

});



