﻿fireproj.service("TaskService", function ($http) {
    this.CreateTask = function (task, successCallBack) {
        $http.post("/api/TaskApi/Create", task).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.UpdateTask = function (task, successCallBack) {
        $http.post("/api/TaskApi/Update", task).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    }; 
    this.GetTaskInfo = function (taskId,successCallBack) {
        $http.get("/api/TaskApi/"+taskId).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    

    this.BeginDeploy = function (taskId,triggerId,successCallBack) {
        $http.post("/api/TaskApi/BeginDeploy?taskId=" + taskId + "&triggerId=" + triggerId).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.GetEnvironment = function (successCallBack) {
        $http.get("/api/CommonApi/Environment").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.CommitToTest = function (taskId, successCallBack) {
        $http.post("/api/TaskApi/CommitToTest?taskId=" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.TestFails = function (taskId, successCallBack) {
        $http.post("/api/TaskApi/TestFails?taskId=" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.Tested = function (taskId, successCallBack) {
        $http.post("/api/TaskApi/Tested?taskId=" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
});