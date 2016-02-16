$(function () {
    //contextualMenuSample();
   // handleSample();
});

var contextualMenuSample = function () {
    $("#jstree").jstree({
        "core": {
            "themes": {
                "responsive": false
            },
            // so that create works
            "check_callback": true,
            'data': [{
                "text": "Parent Node",
                "children": [{
                    "text": "Initially selected",
                    "state": {
                        "selected": true
                    }
                }, {
                    "text": "Custom Icon",
                    "icon": "fa fa-warning icon-state-danger"
                }, {
                    "text": "Initially open",
                    "icon": "fa fa-folder icon-state-success",
                    "state": {
                        "opened": true
                    },
                    "children": [
                        { "text": "Another node", "icon": "fa fa-file icon-state-warning" }
                    ]
                }, {
                    "text": "Another Custom Icon",
                    "icon": "fa fa-warning icon-state-warning"
                }, {
                    "text": "Disabled Node",
                    "icon": "fa fa-check icon-state-success",
                    "state": {
                        "disabled": true
                    }
                }, {
                    "text": "Sub Nodes",
                    "icon": "fa fa-folder icon-state-danger",
                    "children": [
                        { "text": "Item 1", "icon": "fa fa-file icon-state-warning" },
                        { "text": "Item 2", "icon": "fa fa-file icon-state-success" },
                        { "text": "Item 3", "icon": "fa fa-file icon-state-default" },
                        { "text": "Item 4", "icon": "fa fa-file icon-state-danger" },
                        { "text": "Item 5", "icon": "fa fa-file icon-state-info" }
                    ]
                }]
            },
                "Another Node"
            ]
        },
        "types": {
            "default": {
                "icon": "fa fa-folder icon-state-warning icon-lg"
            },
            "file": {
                "icon": "fa fa-file icon-state-warning icon-lg"
            }
        },
        "state": { "key": "demo2" },
        "plugins": ["contextmenu", "dnd", "state", "types"]
    });
}
var handleSample = function () {

    $('#projectTree').jstree({
        "core": {
            "themes": {
                "responsive": false
            }
        },
        "types": {
            "default": {
                "icon": "fa fa-folder icon-state-warning icon-lg"
            },
            "file": {
                "icon": "fa fa-file icon-state-warning icon-lg"
            }
        },
        "plugins": ["types"]
    });

    // handle link clicks in tree nodes(support target="_blank" as well)
    $('#projectTree').on('select_node.jstree', function (e, data) {
        var link = $('#' + data.selected).find('a');
        if (link.attr("href") != "#" && link.attr("href") != "javascript:;" && link.attr("href") != "") {
            if (link.attr("target") == "_blank") {
                link.attr("href").target = "_blank";
            }
            document.location.href = link.attr("href");
            return false;
        }
    });
}
function getSelectNode() {
    $('#projectTree').on("changed.jstree", function (e, data) {
        if (data.selected.length) {
            alert(data.instance.get_node(data.selected[0]).text);
        }
    })
}

function getFileMenu(id){
    $('#jstree').jstree({
        'core': {
            'data': [
                'Simple root node',
                {
                    'id': 'node_2',
                    'text': 'Root node with options',
                    'state': { 'opened': true, 'selected': true },
                    'children': [{ 'text': 'Child 1' }, 'Child 2']
                }
            ]
        }
    });
}