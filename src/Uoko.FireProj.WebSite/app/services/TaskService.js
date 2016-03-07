fireproj.service("TaskService", function ($http) {


    function transfTaskData(taskList) {
        return _.each(taskList, function (task, key) {
            if (task.TaskInfo.DeployInfoIocJson) {
                task.DeployInfoIoc = JSON.parse(task.TaskInfo.DeployInfoIocJson);
            }
            if (task.TaskInfo.DeployInfoPreJson) {
                task.DeployInfoPre = JSON.parse(task.TaskInfo.DeployInfoPreJson);
            }
            if (task.TaskInfo.DeployInfoOnlineJson) {
                task.DeployInfoOnline = JSON.parse(task.TaskInfo.DeployInfoOnlineJson);
            }

            task.IocTestAllPassed = task.TaskInfo.IocCheckUserId && !(/-[01]/m.test(task.TaskInfo.IocCheckUserId));

            task.PreTestAllPassed = task.TaskInfo.PreCheckUserId && !(/-[01]/m.test(task.TaskInfo.PreCheckUserId));

            task.OnlineTestAllPassed = task.TaskInfo.OnlineCheckUserId && !(/-[01]/m.test(task.TaskInfo.OnlineCheckUserId));
        });
    }

    this.GetTaskByPage = function (params, successCallBack) {
        $http.get("/api/TaskApi", { params: params }).success(function (data) {

            data.rows = transfTaskData(data.rows);

            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    };


    this.GetTasksNeedToBeOnline = function(params,cb) {
          $http.get("/api/TaskApi/tasksNeedOnline/"+params.projectId, { params: params }).success(function (data) {

              data.rows = transfTaskData(data.rows);

            if (successCallBack != undefined) {
                successCallBack(data);
            }
        }).error(function (data) {
            //错误处理
        });;
    }


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
    

    this.BeginDeploy = function (taskId,deployStage,successCallBack) {
        $http.post("/api/TaskApi/BeginDeploy?taskId=" + taskId + "&deployStage=" + deployStage).success(function (data) {
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


    this.UpdateTestStatus = function (param, successCallBack) {
        $http.post("/api/TaskApi/UpdateTestStatus", param).success(function (data) {
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

    this.GetDomain = function (projectId, serverId, taskId, successCallBack) {
        $http.get("/api/DomainResourceApi/" + projectId + "/" + serverId + "/" + taskId).success(function (data) {
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