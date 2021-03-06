﻿function fancyTreeForumJumpSingleNodeLazyJs(selector, treeId, userId, boardId, echoActive, activeNode, argums, jsonData, forumUrl) {
    $('#' + treeId).fancytree(
    {
        title: 'Fancy Tree',
        toggleEffect: { height: 'toggle', duration: 100 },
        autoFocus: false,
        source: {
            url: jsonData + 's=' + boardId + argums + forumUrl
        },
        activate: function (event, data) {
            $('#' + echoActive).value = data.node.key;
            if (data.href) {
                window.open(data.href, data.node.target);
            }
        },
        lazyLoad: function (event, dtnode) {
            dtnode.result = $.ajax({
                url: jsonData + '=' + dtnode.node.key + argums + forumUrl,
                dataType: 'json'
            });
        }
    });
}

