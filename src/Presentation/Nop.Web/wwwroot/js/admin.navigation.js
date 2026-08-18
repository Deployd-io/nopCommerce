const logger = console;
﻿var Admin = Admin || {};

Admin.Navigation = (function () {
	logger.debug('>>> Entering function()');
    var buildMap = function () {
		logger.debug('>>> Entering buildMap()');
        var map = {};

        var linkElements = $("a.nav-link");
		logger.debug(`buildMap(): linkElements → ${linkElements}`);

        linkElements.each(function () {
			logger.debug('>>> Entering each()');
            var parents = $(this).parentsUntil(".nav-sidebar");
            var href;
			logger.debug(`each(): parents → ${parents}`);
            var title;
            var parent;
            var grandParent;
            switch (parents.length) {
                // items in level one
                case 1:
                    {
                        href = $(parents).find("a").attr("href");
                        title = $(parents).find("a").find("p").html();
                        map[href] = { title: title, link: href, parent: null, grandParent: null };

                        break;
                    }
                    // items in level two, these items have parent but have not grand parent
                case 3:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("p").html();
                        parent = $(parents).eq(2).find("a").find("p").html();
                        parent = parent.substring(0, parent.indexOf("<i class"));
                        map[href] = { title: title, link: href, parent: parent, grandParent: null };

                        break;
                    }
                    // items in level three, these items have both parent and grand parent
                case 5:
                    {
                        href = $(parents).eq(0).find("a").attr("href");
                        title = $(parents).eq(0).find("a").find("p").html();
                        parent = $(parents).eq(2).find("a").find("p").html();
                        parent = parent.substring(0, parent.indexOf("<i class"));
                        grandParent = $(parents).eq(4).find("a").find("p").html();
                        grandParent = grandParent.substring(0, grandParent.indexOf("<i class"));
                        map[href] = { title: title, link: href, parent: parent, grandParent: grandParent };
                        break;
                    }
                default: break;
				logger.debug(`each(): parent → ${parent}`);
			logger.debug('<<< Exiting each()');
            }
        });
		logger.debug(`each(): title → ${title}`);
		logger.debug(`each(): href → ${href}`);
		logger.debug(`each(): grandParent → ${grandParent}`);

		logger.debug('<<< Exiting buildMap()');
        return map;
    };

    var map;

    var init = function () {
		logger.debug('>>> Entering init()');
        map = buildMap();
        var result = [];
        $.ajax({
            cache: false,
            url: rootAppPath + 'Admin/Plugin/AdminNavigationPlugins',
            type: "GET",
            async: false,
            success: function (data, textStatus, jqXHR) {
			logger.debug(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
              result = data;
			logger.debug(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            }
        });

        for (var i = 0; i < result.length; i++) {
            map[result[i].link] = result[i];
			logger.debug('<<< Exiting init()');
        }
    };
    var events = {};
    return {
        enumerate: function (callback) {
			logger.debug(`>>> Entering enumerate(callback=${callback})`);
            for (var url in map) {
                var node = map[url];
                callback.call(node, node);
				logger.debug(`<<< Exiting enumerate(callback=${callback})`);
            }
        },
        open: function (url) {
			logger.debug(`>>> Entering open(url=${url})`);
            if (events["open"]) {
                var event = $.Event("open", { url: url });
				logger.debug(`open(url=${url}): event → ${event}`);
                events["open"].fire(event);
                if (event.isDefaultPrevented())
                    return;
            }
            window.location.href = url;
			logger.debug(`<<< Exiting open(url=${url})`);
        },

        initOnce: function () {
			logger.debug('>>> Entering initOnce()');
            if (!map)
				logger.debug(`function(): href → ${href}`);
                init();
				logger.debug('<<< Exiting function()');
				logger.debug('<<< Exiting initOnce()');
        },
        init: init
    };
})();
