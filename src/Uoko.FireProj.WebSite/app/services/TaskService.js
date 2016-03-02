﻿fireproj.service("TaskService", function ($http) {

    this.GetTaskByPage = function (params, successCallBack) {
        $http.get("/api/TaskApi?Offset={0}&Limit={1}".Format(params.offset, params.limit)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    this.GetTaskLogsByPage = function (params, successCallBack) {
        $http.get("/api/TaskLogsApi?Offset={0}&Limit={1}&taskId={2}&environment={3}&sort={4}&order={5}".Format(params.offset, params.limit, params.taskId, params.environment, params.sort, params.order)).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };
    
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
    

    this.BeginDeploy = function (taskId,deployStage,triggerId,successCallBack) {
        $http.post("/api/TaskApi/BeginDeploy?taskId=" + taskId + "&deployStage=" + deployStage + "&triggerId=" + triggerId).success(function (data) {
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

    this.CommitToTest = function (param, successCallBack) {
        $http.post("/api/TaskApi/CommitToTest", param).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.TestFails = function (param, successCallBack) {
        $http.post("/api/TaskApi/TestFails", param).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.Tested = function (param, successCallBack) {
        $http.post("/api/TaskApi/Tested", param).success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.GetResourceList = function (environmentId, successCallBack) {
        $http.get("/api/ServerApi/Environment/" + environmentId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.GetDomain = function (projectId, serverId, successCallBack) {
        $http.get("/api/DomainResourceApi/" + projectId + "/" + serverId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    this.GetDomainByTaskId = function (projectId, serverId, taskId, successCallBack) {
        $http.get("/api/DomainResourceApi/" + projectId + "/" + serverId + "/" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    

    this.GetLogTotal = function (taskId, successCallBack) {
        $http.get("/api/TaskLogsApi/LogTotal/" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };

    this.ReleaseDomain = function (taskId, successCallBack) {
        $http.post("/api/DomainResourceApi/ReleaseDomain/?id=" + taskId + "").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
    /////////////////////////////////
    this.GetTaskLogsByTaskId = function (taskId, successCallBack) {
        $http.get("/api/TaskLogsApi/" + taskId + "/ByTaskId").success(function (data) {
            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });
    };
});