/*
  定义全局module
*/
var fireproj;
(function() {
    fireproj = angular.module("FireProj", ['ngMessages', 'ui.bootstrap', 'ngTagsInput']);
    fireproj.config([
        '$provide', '$httpProvider','tagsInputConfigProvider', function($provide, $httpProvider, tagsInputConfigProvider) {
            tagsInputConfigProvider.setDefaults('tagsInput', { minLength: 1 }).setDefaults('autoComplete', { minLength: 0, selectFirstMatch: true });

            // alternatively, register the interceptor via an anonymous factory
            $httpProvider.interceptors.push(function($q) {
                return {
                    'responseError': function(rejection) {
                        if (rejection.data.Message) {
                            bootbox.alert(rejection.data.Message);
                        }
                        return $q.reject(rejection);
                    }
                };
            });

        }
    ]);

})();


//表单提交成功事件通用操作
var formSubmitSuccessClick = function (operation) {
    bootbox.alert("操作成功", function (data) {
        if (operation === "refresh") {
            window.location.reload();
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
//根据用户id获取用户名
var AnalysisUser = function (userInfo, UserAll) {
    $.each(userInfo, function (i, item) {
        var obj = UserAll.filter(function (user) {
            return item.Id == user.UserId;
        })[0];
        userInfo[i].NickName = obj.NickName;
    });
    return userInfo;
}