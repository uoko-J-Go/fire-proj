fireproj.filter("environmentFilter", function() {

    return function(value) {
        switch (value) {
        case 0:
            return "IOC环境";
        case 1:
            return "Pre环境";
        case 2:
            return "正式环境";
        default:
            return "未知";
        }
    }
});

fireproj.filter("taskStatusFilter", function () {

    return function (value) {
        switch (value) {
            case 0:
                return "待部署";
            case 2:
                return "部署中";
            case 4:
                return "部署失败";
            case 6:
                return "部署成功";
            case 8:
                return "测试中";
            case 10:
                return "测试未通过";
            case 12:
                return "测试通过";
            default:
                return "未知";
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