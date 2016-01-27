/*
  定义全局module
*/
var fireproj;
(function () {
    fireproj = angular.module("FireProj", ['ngMessages','ui.bootstrap']);
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
var formSubmitSuccessClick = function (operation) {
    bootbox.alert("提交成功", function (data) {
        if (operation == "refresh") {
            window.location.href = window.location.href
        }
        else {
            window.location.href = document.referrer;
        }
    });
}

//表单提交失败事件通用操作
var formSubmitFailClick = function (data) {
    var msgInfo = new Array();
    if (data.Message != null) {
        msgInfo.push(data.Message);
    } else {
        angular.forEach(data.ModelState, function (data) {
            msgInfo.push(data);
        });
    }
    for (var i = 0; i < msgInfo.length; i++) {
        bootbox.alert(msgInfo[i], function (data) {

        });
    }
}

function GetQueryString(name) {
    var reg = new RegExp("(^|&)" + name + "=([^&]*)(&|$)");
    var r = window.location.search.substr(1).match(reg);
    if (r != null) return unescape(r[2]); return null;
}

String.prototype.Format = function () {
    var args = arguments;
    return this.replace(/{(\d{1})}/g, function () {
        return args[arguments[1]];
    });
}