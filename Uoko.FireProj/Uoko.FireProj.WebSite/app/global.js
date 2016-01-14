/*
  定义全局module
*/
var fireproj;
(function () {
    fireproj = angular.module("FireProj", ['ngMessages']);
})();

//bootstrap-table的angular指令
fireproj.directive('initTable', ['$compile', function ($compile) {
    return {
        restrict: 'A',
        link: function (scope, el, attrs) {
            var opts = scope.$eval(attrs.initTable);

            opts.onLoadSuccess = function () {
                $compile(el.contents())(scope);
            };

            el.bootstrapTable(opts);
        }

    };
}]);

//表单提交成功事件通用操作
var formSubmitSuccessClick = function () {
    bootbox.alert("提交成功", function (data) {
        location.reload();//刷新分页
    });
}

//表单提交失败事件通用操作
var formSubmitFailClick = function (data) {
    var msgInfo = new Array();
    if (data.ErrorInfo == undefined) {
        angular.forEach(data.ModelState, function (data) {
            msgInfo.push(data);
        });
    } else {
        msgInfo.push(data.Message);
    }
    for (var i = 0; i < msgInfo.length; i++) {
        Metronic.alert({
            container: "#bootstrap_alerts_demo",
            message: msgInfo[i],
            icon: "warning",
            type: "warning",
            reset: false,
        });
    }
}