var deps = ['treeGrid'];
fireproj = angular.module("FireProj", ['ngMessages', '' + deps + '']);
fireproj.service("DictionaryService", function ($http) {
    //列表分页
    this.getByPage = function () {
        return $http.get("/api/DictionaryApi/");
    };

    //根据Id获取表单信息
    this.getById = function (id) {
        return $http.get("/api/DictionaryApi/" + id + "/ById");
    };

    //新增
    this.post = function (model) {
        return $http.post("/api/DictionaryApi", model);
    };

    //修改
    this.put = function (id, model) {
        return $http.put("/api/DictionaryApi/" + id + "", model);
    };

    //删除
    this.delete = function (id) {
        return $http.delete("/api/DictionaryApi/" + id + "");
    };

   
});



fireproj.controller("DictionaryController", function ($scope, $http, DictionaryService) {
    var tree;
    var rawTreeData;;
  
    DictionaryService.getByPage().success(function (data) {
        rawTreeData = data.rows;
        $scope.tree_data = getTree(rawTreeData, 'Id', 'ParentId');;
    }).error(function (data) {
        formSubmitFailClick(data);
    });

   

    var myTreeData = getTree(rawTreeData, 'Id', 'ParentId');

    $scope.tree_data = myTreeData;
    $scope.my_tree = tree = {};
    $scope.expanding_property = {
        field: "Name",
        displayName: "字典名称",
        sortable : true,
        filterable: true
    };
    $scope.col_defs = [
        {
            field: "Value",
            displayName: "字典值",
            sortable : true,										
            sortingType : "string"
        },
        {
            field: "Description",
            displayName: "描述",
            sortable : true,										
            sortingType: "string",
            filterable: true
        },
        {
            field: "Status",
            displayName: "状态",
            sortable : true,										
            sortingType : "number"
        },
        {
            field: "",
            displayName: "操作",
            sortable: true,
            sortingType: "number"
        }

    ];
    $scope.my_tree_handler = function (branch) {
        console.log('you clicked on', branch)
    }

    function getTree(data, primaryIdName, parentIdName) {
        if (!data || data.length == 0 || !primaryIdName || !parentIdName)
            return [];

        var tree = [],
            rootIds = [],
            item = data[0],
            primaryKey = item[primaryIdName],
            treeObjs = {},
            parentId,
            parent,
            len = data.length,
            i = 0;

        while (i < len) {
            item = data[i++];
            primaryKey = item[primaryIdName];
            treeObjs[primaryKey] = item;
            parentId = item[parentIdName];

            if (parentId) {
                parent = treeObjs[parentId];

                if (parent.children) {
                    parent.children.push(item);
                } else {
                    parent.children = [item];
                }
            } else {
                rootIds.push(primaryKey);
            }
        }

        for (var i = 0; i < rootIds.length; i++) {
            tree.push(treeObjs[rootIds[i]]);
        };

        return tree;
    }

});



