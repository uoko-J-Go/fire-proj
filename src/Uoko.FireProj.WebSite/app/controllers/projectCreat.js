﻿
//创建一个ProjectController
fireproj.controller("ProjectController", function ($scope, $http, ProjectService) {
   var zTreeObj;
    // zTree 的参数配置，深入使用请参考 API 文档（setting 配置详解）
   var setting = {
       async: {
           enable: true,
           type: "get",
           url: getAsyncUrl,
           dataFilter: ajaxDataFilter,
       },
       callback: {
           onDblClick: zTreeOnDblClick
       }
   };
   var projectId;
   function getAsyncUrl(treeId, treeNode) {
       if (typeof treeNode == "undefined") {
           return "{0}/api/v3/projects/{1}/repository/tree?private_token={2}".Format(Global.GitLabUrl, projectId, Global.GitLabToken);
       }
       else {
           var path = "";
           var node = treeNode.getPath();
           if (typeof node != "undefined") {

               for (var i = 0; i < node.length; i++) {
                   path += node[i].name + "/";
               }
           }
           return "{0}/api/v3/projects/{1}/repository/tree?private_token={2}&path={3}".Format(Global.GitLabUrl, projectId, Global.GitLabToken, path);
       }
   };

   function ajaxDataFilter(treeId, parentNode, responseData) {
       if (responseData) {
           for (var i = 0; i < responseData.length; i++) {
               if (responseData[i].type == "tree") {
                   responseData[i].isParent = true;
               }
           }
       }
       return responseData;
   };

   function zTreeOnDblClick(event, treeId, treeNode) {
       if (treeNode.isParent) {
           return;
       }
       var fullpath = getFullPath();
       if (checkButton == "sln") {
           if (!/\.(sln)/i.test(fullpath)) {
               alert("请选择正确的解决方案文件");
               return;
           }
           $("#ProjectSlnName").val(fullpath);
          
       }
       else {
           if (!/\.(csproj|xproj)/i.test(fullpath)) {
               alert("请选择正确的项目文件");
               return;
           }
           $("#ProjectCsprojName").val(fullpath);
       }
       $('#myModal').modal('hide');
   };

   var checkButton = "";

   $scope.showMenu = function (type) {
       checkButton = type;
       $('#myModal').modal('show');
   }
   function getFullPath() {
       var path = "";
       var treeObj = $.fn.zTree.getZTreeObj("treeDemo");
       var sNodes = treeObj.getSelectedNodes();
       if (sNodes.length > 0) {
           var node = sNodes[0].getPath();

           if (typeof node != "undefined") {
               //遍历获取完整的相对路径
               for (var i = 0; i < node.length; i++) {     
                   path += node[i].name;
                   if (node[i].isParent) {
                       path += "/";
                   }
               }
           }
       }
       return path;
   }


    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        var gitlabInfo = model.Project;
        model.ProjectRepo = gitlabInfo.http_url_to_repo;
        model.RepoId = gitlabInfo.id;
        model.ProjectGitlabName = gitlabInfo.name;

        model.ProjectCsprojName = $("#ProjectCsprojName").val();
        model.ProjectSlnName = $("#ProjectSlnName").val();

        ProjectService.post(model).success(function (data) {
            location.href = "/Project/Index";
        }).error(function (data) {
            formSubmitFailClick(data);
        });
    };

    //获取gitlab所有项目信息
    ProjectService.getGitlbProject().success(function (data) {
        $scope.ProjectList = data;
    }).error(function (data) {
        formSubmitFailClick(data);
    });

    //选择项目change事件
    $scope.GetProjectInfo = function (project) {
        $scope.showSln = project;
        projectId = project.id;
        ProjectService.getGitLabSln(project.id, function (data) {
            
            var fileList = new Array();
            for (var i = 0; i < data.length; i++) {
                //if ((/\.[^\.]+$/.exec(data[i].name)) == ".sln") {
                //    $scope.model.ProjectSlnName = data[i].name;//项目sln名称
                //}
                if (data[i].type == "tree" && data[i].name != ".nuget" && data[i].name != ".vs") {
                    fileList.push(data[i]);
                }
            }
            $scope.menuList = fileList;
            zTreeObj = $.fn.zTree.init($("#treeDemo"), setting);
        });
    }


   

   




    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }
});


