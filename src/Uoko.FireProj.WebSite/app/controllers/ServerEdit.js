fireproj.controller("ServerController", function ($scope, $http, ServerService,TaskService) {
    
    $scope.LoadForm = function () {
        var serverId = $("#id").val();
        ServerService.getById(serverId, function (data) {
            $scope.model = data;
        });
    }

    $scope.GetEnvironment = function () {
        TaskService.GetEnvironment(function (data) {
            $scope.environmentList = data;
            $scope.LoadForm();
        });
    }
    //声明表单提交事件
    $scope.SubmitFrom = function (model) {
        $('#dg').datagrid('acceptChanges');
        model.IISData = $('#dg').datagrid('getRows');

        ServerService.Update(model, function (data) {
            location.href = "/SystemSet/ServerMgmt";
        });
    };
    $scope.Init = function () {
        $scope.GetEnvironment();
    }


    $scope.Init();
});

var editIndex = undefined;
$(function () {
    var serverId = $("#id").val();

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
            onCheck: onCheck,
            onEndEdit: onEndEdit,
            url: '/api/DomainResourceApi/' + serverId + '',
            method: 'get',
            columns: [[
                { field: 'Name', title: '域名', width: 200, align: 'center', editor: 'textbox' },
                { field: 'SiteName', title: 'IIS站点', width: 200, align: 'center', editor: 'textbox' },
                {
                    field: 'ProjectId', title: '所属项目', width: 200, align: 'center', editor: 'textbox',
                    formatter: function (value, row) {
                            return row.ProjectName;
                        },
                        editor: {
                        type: 'combobox',
                        options: {
                            valueField: 'ProjectId',
                            textField: 'ProjectName',
                            //method: 'get',
                            //url: '/api/ProjectApi/GetAll',
                            data: projectData,
                            required: true
                        }
                    }
                }
            ]]
        });
       
    });

   
});
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
function onCheck(index, row) {
    editIndex = index;
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

    var rows = $('#dg').datagrid('getSelected');
    if (rows.Id != undefined) { //已保存数据库的数据执行从数据库中删除
        bootbox.confirm("确认从数据库删除?", function (result) {
            if (result == true) {
                $.post("/api/DomainResourceApi/DeleteDomain/" + rows.Id + "", function (data) {
                    bootbox.alert("删除成功!", function () {
                        $('#dg').datagrid('cancelEdit', editIndex).datagrid('deleteRow', editIndex);
                        editIndex = undefined;
                    });
                });
            }
        });
    }
    else {
        $('#dg').datagrid('cancelEdit', editIndex).datagrid('deleteRow', editIndex);
        editIndex = undefined;
    }
    
}
function accept() {
    if (endEditing()) {
        $('#dg').datagrid('acceptChanges');
    }
}