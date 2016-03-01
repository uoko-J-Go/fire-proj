//创建一个ProjectController
fireproj.controller("ServerController", function ($scope, $http, ServerService, TaskService, ProjectService) {
    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        $('#dg').datagrid('acceptChanges');
        model.IISData  = $('#dg').datagrid('getRows');
        ServerService.Create(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };

    TaskService.GetEnvironment(function (data) {
        $scope.environmentList = data;
    });


    ProjectService.getAllProject(function (data) {
        $scope.projectList = data;
    });

    

    $scope.CancelForm = function () {
        location.href = "/Project/Index";
    }

    //////////////////////////////////////////////////////////////////
    
    var i = 0;
    var IISData = new Array();
    $scope.Append = function (model) {
        var project = JSON.parse(model.project);
        var info = {
            "Index":i++,
            "Name": model.Domain,
            "SiteName": model.SiteName,
            "ProjectId": project.ProjectId,
            "ProjectName": project.ProjectName
        }
        
        if (model.Index != null) {
            IISData[model.Index] = info;
        }
        else {
            IISData.push(info);
        }
        $scope.items = IISData;
    }
    $scope.edit = function (item) {
        $scope.model.Index = item.Index;
        $scope.model.Domain = item.Name;
        $scope.model.SiteName = item.SiteName;
        $scope.model.ProjectId = item.ProjectId;
    };
    $scope.delete = function (item) {
        IISData.remove(item.Index);
        $scope.items = IISData;

    };

    Array.prototype.remove = function (dx) {
        if (isNaN(dx) || dx > this.length) { return false; }
        for (var i = 0, n = 0; i < this.length; i++) {
            if (this[i] != this[dx]) {
                this[n++] = this[i]
            }
        }
        this.length -= 1
    }

    ///////////////////////////////////
    
        
   
});

var editIndex = undefined;
$(function () {
    var projectData = new Array();
    $.get("/api/ProjectApi/GetAll", function (data) {
        for (var i = 0; i < data.length; i++) {
            projectData.push(
                {
                    ProjectId: data[i].Id,
                    ProjectName: data[i].ProjectName
                }
            );
        }
        $('#dg').datagrid({
            toolbar: '#tb',
            rownumbers: true,
            singleSelect: true,
            onClickCell: onClickCell,
            onEndEdit: onEndEdit,
            onCheck: onCheck,
            columns: [[
                { field: 'Name', title: '域名', width: 200, align: 'center', editor: 'textbox' },
                { field: 'SiteName', title: 'IIS站点', width: 200, align: 'center', editor: 'textbox' },
                {
                    field: 'ProjectId', title: '所属项目', width: 200, align: 'center', editor: {
                        type: 'combobox',
                        options: {
                            valueField: 'ProjectId',
                            textField: 'ProjectName',
                            // method: 'get',
                            // url: '/api/ProjectApi/GetAll',
                            data: projectData,
                            required: true
                        }
                    },
                    formatter: function (value, row) {
                        return row.ProjectName;
                    },
                }
            ]]
        });
    });


});
function onCheck(index, row) {
    editIndex = index;
}
function endEditing() {
    if (editIndex == undefined) { return true }
    if ($('#dg').datagrid('validateRow', editIndex)) {
        $('#dg').datagrid('endEdit', editIndex);
        editIndex = undefined;
        return true;
    } else {
        return false;
    }
}
function onClickCell(index, field) {
    if (editIndex != index) {
        if (endEditing()) {
            $('#dg').datagrid('selectRow', index)
                    .datagrid('beginEdit', index);
            var ed = $('#dg').datagrid('getEditor', { index: index, field: field });
            if (ed) {
                ($(ed.target).data('textbox') ? $(ed.target).textbox('textbox') : $(ed.target)).focus();
            }
            editIndex = index;
        } else {
            setTimeout(function () {
                $('#dg').datagrid('selectRow', editIndex);
            }, 0);
        }
    }
}
function onEndEdit(index, row) {
    var ed = $('#dg').datagrid('getEditor', {
        index: index,
        field: 'ProjectId'
    });
    row.ProjectName = $(ed.target).combobox('getText');
}
function append() {
    if (endEditing()) {
        $('#dg').datagrid('appendRow', {});
        editIndex = $('#dg').datagrid('getRows').length - 1;
        $('#dg').datagrid('selectRow', editIndex)
                .datagrid('beginEdit', editIndex);
    }
}
function removeit() {
    if (editIndex == undefined) { return }
    $('#dg').datagrid('cancelEdit', editIndex)
            .datagrid('deleteRow', editIndex);
    editIndex = undefined;
}
function accept() {
    if (endEditing()) {
        $('#dg').datagrid('acceptChanges');
    }
}

