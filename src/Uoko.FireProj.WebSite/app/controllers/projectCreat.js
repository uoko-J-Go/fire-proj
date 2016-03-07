
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
           onClick: zTreeOnClick
       }
   };
   var projectId;
   function getAsyncUrl(treeId, treeNode) {
       if (typeof treeNode == "undefined") {
           return "http://gitlab.uoko.ioc:12015/api/v3/projects/" + projectId + "/repository/tree?private_token=JX4Gb7W_gfp7PdzpBjpG";
       }
       else {
           var path = "";
           var node = treeNode.getPath();
           if (typeof node != "undefined") {

               for (var i = 0; i < node.length; i++) {
                   path += node[i].name + "/";
               }
           }
           return "http://gitlab.uoko.ioc:12015/api/v3/projects/" + projectId + "/repository/tree?private_token=JX4Gb7W_gfp7PdzpBjpG&path=" + path + "";
       }
   };

   function ajaxDataFilter(treeId, parentNode, responseData) {
       if (responseData) {
           for (var i = 0; i < responseData.length; i++) {
               if (responseData[i].type == "tree") {
                   responseData[i].isParent = "true";
               }
           }
       }
       return responseData;
   };

   function zTreeOnClick(event, treeId, treeNode) {
      $("#ProjectCsprojName").val(getFullPath());
   };

   $scope.showMenu = function () {
       var cityObj = $("#ProjectCsprojName");
       var cityOffset = $("#ProjectCsprojName").offset();
       $("#menuContent").css({ left: cityOffset.left + "px", top: cityOffset.top + cityObj.outerHeight() + "px" }).slideDown("fast");

       $("body").bind("mousedown", onBodyDown);
   }
   function hideMenu() {
       $("#menuContent").fadeOut("fast");
       $("body").unbind("mousedown", onBodyDown);
   }
   function onBodyDown(event) {
       if (!(event.target.id == "menuBtn" || event.target.id == "menuContent" || $(event.target).parents("#menuContent").length > 0)) {
           hideMenu();
       }
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
        if (typeof gitlabInfo == 'string') {
            gitlabInfo = JSON.parse(gitlabInfo);
        }
        model.ProjectRepo = gitlabInfo.http_url_to_repo;
        model.RepoId = gitlabInfo.id;
        model.ProjectGitlabName = gitlabInfo.name;
        var projectCsprojName = $("#ProjectCsprojName").val();

        //model.ProjectCsprojName = projectCsprojName.substring(0, projectCsprojName.length - 1);

        model.ProjectCsprojName = projectCsprojName;

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
        if (typeof project == 'string') {
            project = JSON.parse(project);
        }
        projectId = project.id;
        ProjectService.getGitLabSln(project.id, function (data) {
            
            var fileList = new Array();
            for (var i = 0; i < data.length; i++) {
                if ((/\.[^\.]+$/.exec(data[i].name)) == ".sln") {
                    $scope.model.ProjectSlnName = data[i].name;//项目sln名称
                }
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


