fireproj.controller("TestController", function ($scope, $uibModalInstance, TaskService, param) {
    $scope.testResult = param;
    $scope.Ok = function () {
        if ($scope.testResult.QAStatus == 1 && $scope.testResult.Comments.length < 1) {
            bootbox.alert("请填写备注...", null);
        } else {
            TaskService.UpdateTestStatus($scope.testResult,function() {
                $uibModalInstance.close();
                window.location.reload();
            });
           
        }
        
    }
    $scope.Cancel = function () {
        $uibModalInstance.dismiss('cancel');
    }        

});