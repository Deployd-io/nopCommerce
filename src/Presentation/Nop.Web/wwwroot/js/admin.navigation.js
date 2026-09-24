var logger = (typeof logger !== 'undefined' && logger) || console;
var Admin = Admin || {};

Admin.Navigation = (function () {
    var buildMap = function () {
		logger.error('>>> Entering buildMap()');
        var map = {};

        var linkElements = $("a.nav-link");
		logger.error(`buildMap(): linkElements → ${linkElements}`);

        linkElements.each(function () {
            var parents = $(this).parentsUntil(".nav-sidebar");
            var href;
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
						logger.debug(`buildMap(): href → ${href}`);
						logger.debug(`buildMap(): title → ${title}`);
                        parent = $(parents).eq(2).find("a").find("p").html();
                        parent = parent.substring(0, parent.indexOf("<i class"));
                        grandParent = $(parents).eq(4).find("a").find("p").html();
						logger.debug(`buildMap(): parent → ${parent}`);
                        grandParent = grandParent.substring(0, grandParent.indexOf("<i class"));
                        map[href] = { title: title, link: href, parent: parent, grandParent: grandParent };
						logger.debug(`buildMap(): grandParent → ${grandParent}`);
                        break;
                    }
                default: break;
            }
        });

		logger.warn('<<< Exiting buildMap()');
        return map;
    };

    var map;

    var init = function () {
		logger.error('>>> Entering init()');
        map = buildMap();
        var result = [];
        $.ajax({
            cache: false,
            url: rootAppPath + 'Admin/Plugin/AdminNavigationPlugins',
            type: "GET",
            async: false,
            success: function (data, textStatus, jqXHR) {
			logger.error(`>>> Entering success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
              result = data;
			logger.warn(`<<< Exiting success(data=${data},textStatus=${textStatus},jqXHR=${jqXHR})`);
            }
        });

        for (var i = 0; i < result.length; i++) {
            map[result[i].link] = result[i];
			logger.warn('<<< Exiting init()');
        }
    };
    var events = {};
    return {
        enumerate: function (callback) {
			logger.error(`>>> Entering enumerate(callback=${callback})`);
            for (var url in map) {
                var node = map[url];
                callback.call(node, node);
				logger.warn(`<<< Exiting enumerate(callback=${callback})`);
            }
        },
        open: function (url) {
			logger.error(`>>> Entering open(url=${url})`);
            if (events["open"]) {
                var event = $.Event("open", { url: url });
				logger.error(`open(url=${url}): event → ${event}`);
                events["open"].fire(event);
                if (event.isDefaultPrevented())
                    return;
            }
            window.location.href = url;
			logger.warn(`<<< Exiting open(url=${url})`);
        },

        initOnce: function () {
			logger.error('>>> Entering initOnce()');
            if (!map)
                init();
				logger.warn('<<< Exiting initOnce()');
        },
        init: init
    };
})();
