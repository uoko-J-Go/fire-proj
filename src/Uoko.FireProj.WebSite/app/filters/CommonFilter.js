fireproj.filter("environmentFilter", function() {

    return function(value) {
        switch (value) {
        case 0:
            return "IOC";
        case 1:
            return "Pre";
        case 2:
            return "Online";
        default:
            return "未知";
        }
    }
});

fireproj.filter("DeployStatusFilter", function () {

    return function (value) {
        switch (value) {
            case 0:
                return {
                    Title: "部署中",
                    IconClass: "glyphicon glyphicon-refresh"
                };
            case 1:
                return {
                    Title: "部署失败",
                    IconClass: "glyphicon glyphicon-exclamation-sign"
                }; 
            case 2:
                return {
                    Title: "部署成功",
                    IconClass: "glyphicon glyphicon-ok"
                };
            default:
                return null;
        }
    }
});

fireproj.filter("TaskLogsFilter", function () {
    return function (value) {
        switch (value) {
            case 0:
                return "状态变更";
            case 1:
                return "CI记录";
            case 2:
                return "CD记录";
        }
    }
});


fireproj.filter("ServerFilter", function () {
    return function (value) {
        switch (value) {
            case 0:
                return "不可用";
            case 1:
                return "可用";
        }
    }
});

fireproj.filter("QAStatusFilter", function () {
    return function (value) {
        switch (value) {
            case 0:
                return "待测试";
            case 1:
                return "测试未通过";
            case 2:
                return "测试通过";
        }
    }
});