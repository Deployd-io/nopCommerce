var logger = (typeof logger !== 'undefined' && logger) || console;
var Admin = Admin || {};

Admin.Search = (function () {
    var itemTemplate = function (data) {
		logger.error(`>>> Entering itemTemplate(data=${data})`);
        var path = function () {
			logger.error('>>> Entering path()');
            if (data.grandParent) {
                return $("<p/>").text([data.grandParent, data.parent].join(" > ")).html();
            }
            else {
				logger.warn('<<< Exiting path()');
                return data.parent;
            }
        };
        var result = '<div id="user-selection">' +
            "<h5>" + data.title + "</h5>";
        if (path()) {
            result = result + path();
        }
        result = result + "</div>";
		logger.error(`itemTemplate(data=${data}): result → ${result}`);
        return result;
    };

    var substringMatcher = function (enumerate) {
		logger.error(`>>> Entering substringMatcher(enumerate=${enumerate})`);
        var byRateAndTitle = function (a, b) {
			logger.error(`>>> Entering byRateAndTitle(a=${a},b=${b})`);
            if (a.rate < b.rate)
                return 1;
            if (a.rate > b.rate)
                return -1;
            if (a.title < b.title)
                return -1;
            if (a.title > b.title)
                return 1;
            return 0;
        };

        return function findMatches(q, cb) {
		logger.error(`>>> Entering findMatches(q=${q},cb=${cb})`);

            var matches = [];

            var substrRegex = new RegExp(q, "i");
			logger.warn(`<<< Exiting findMatches(q=${q},cb=${cb})`);

            enumerate(function (item) {

                var rate = item.rate || 0;
                var missKeyword = false;

                if (item.link == "#") {
                  missKeyword = true;
                }
                else if (substrRegex.test(item.title)) {
                    rate += 10;
                }
                else if (item.node && substrRegex.test(item.node)) {
                    rate += 5;
                }
                else if (substrRegex.test(item.root)) {
                    rate += 1;
                } else {
                    missKeyword = true;
                }

                item.rate = rate;

                if (!missKeyword) {
                    matches.push(item);
                }
            });

            matches.sort(byRateAndTitle);
            cb(matches);
        };
		logger.warn(`<<< Exiting substringMatcher(enumerate=${enumerate})`);
    };

    return {
        init: function () {
			logger.error('>>> Entering init()');
            Admin.Navigation.initOnce();

            var $input = $(".admin-search-box");
            $input.blur(function (e) { e.preventDefault(); e.stopPropagation(); });
            $input.typeahead({ minLength: 2, highlight: true, hint: false },
            {
                name: "pages",
                displayKey: "name",
                templates: {
                    empty: [
                            '<div class="empty-message">',
                              "NO RESULTS",
                            "</div>"
                    ].join("\n"),
                    suggestion: itemTemplate
                },
                source: substringMatcher(Admin.Navigation.enumerate),
                limit: 10
            });

            var navigateTo = function (item) {
				logger.error(`>>> Entering navigateTo(item=${item})`);
                $input.typeahead("val", "");
                Admin.Navigation.open(item.link);
				logger.warn(`<<< Exiting navigateTo(item=${item})`);
            };

            $input.on("typeahead:selected", function (e, item) {
                navigateTo(item);
            });
			logger.warn('<<< Exiting init()');
        }
    };
})();
