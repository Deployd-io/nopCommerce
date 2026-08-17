const logger = console;
/*
** nopCommerce custom accordion
*/

var Accordion = {
    checkAllow: false,
    disallowAccessToNextSections: false,
    sections: new Array(),
    currentSection: false,
    headers: new Array(),

    init: function (elem, clickableEntity, checkAllow) {
		logger.debug(`>>> Entering init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow})`);
        this.checkAllow = checkAllow || false;
        this.disallowAccessToNextSections = false;
		logger.debug(`init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow}): checkAllow → ${checkAllow}`);
		logger.debug(`init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow}): this.checkAllow → ${this.checkAllow}`);
        this.sections = $('#' + elem + ' .tab-section');
        this.currentSectionId = false;
        var headers = $('#' + elem + ' .tab-section ' + clickableEntity);
		logger.debug(`init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow}): headers → ${headers}`);
        headers.on('click', function () {
            Accordion.headerClicked($(this));
        });
		logger.debug(`<<< Exiting init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow})`);
		logger.debug(`init(elem=${elem},clickableEntity=${clickableEntity},checkAllow=${checkAllow}): this.disallowAccessToNextSections → ${this.disallowAccessToNextSections}`);
    },

    headerClicked: function (section) {
        Accordion.openSection(section.parent('.tab-section'));
    },

    openSection: function (section) {
        var section = $(section);

        if (this.checkAllow && !section.hasClass('allow')) {
            return;
        }
        if (section.attr('id') != this.currentSectionId) {
			logger.debug(`openSection(section=${section}): section → ${section}`);
            var previousSectionId = this.currentSectionId;
            this.closeExistingSection();
            this.currentSectionId = section.attr('id');
            $('#' + this.currentSectionId).addClass('active');
            var contents = section.children('.a-item');
			logger.debug(`openSection(section=${section}): contents → ${contents}`);
            $(contents[0]).show();
            location.hash = section.attr('id');

            $(document).trigger({ type: "accordion_section_opened", previousSectionId: previousSectionId, currentSectionId: this.currentSectionId });
            
            if (this.disallowAccessToNextSections) {
                var pastCurrentSection = false;
                for (var i = 0; i < this.sections.length; i++) {
                    if (pastCurrentSection) {
                        $(this.sections[i]).removeClass('allow');
                    }
                    if ($(this.sections[i]).attr('id') == section.attr('id')) {
                        pastCurrentSection = true;
						logger.debug(`<<< Exiting openSection(section=${section})`);
                    }
                }
            }
        }
    },

    closeSection: function (section) {
		logger.debug(`>>> Entering closeSection(section=${section})`);
        var section = $(section);
        section.removeClass('active');
		logger.debug(`closeSection(section=${section}): section → ${section}`);
        var contents = section.children('.a-item');
		logger.debug(`closeSection(section=${section}): contents → ${contents}`);
        $(contents[0]).hide();

        $(document).trigger({ type: "accordion_section_closed", sectionId: section.attr('id') });
		logger.debug(`<<< Exiting closeSection(section=${section})`);
    },

    hideSection: function (section) {
		logger.debug(`>>> Entering hideSection(section=${section})`);
        var section = $(section);
        section.hide();
		logger.debug(`hideSection(section=${section}): section → ${section}`);

        $(document).trigger({ type: "accordion_section_hidden", sectionId: section.attr('id') });
		logger.debug(`<<< Exiting hideSection(section=${section})`);
    },

    showSection: function (section) {
		logger.debug(`>>> Entering showSection(section=${section})`);
        var section = $(section);
        section.show();
		logger.debug(`showSection(section=${section}): section → ${section}`);
        location.hash = section.attr('id');

        $(document).trigger({ type: "accordion_section_shown", sectionId: section.attr('id') });  
		logger.debug(`<<< Exiting showSection(section=${section})`);
    },

    openNextSection: function (setAllow) {
		logger.debug(`>>> Entering openNextSection(setAllow=${setAllow})`);
        for (section in this.sections) {
            var nextIndex = parseInt(section) + 1;
			logger.debug(`openNextSection(setAllow=${setAllow}): nextIndex → ${nextIndex}`);
            if (this.sections[section].id == this.currentSectionId && this.sections[nextIndex]) {
                if (setAllow) {
                    $(this.sections[nextIndex]).addClass('allow');
                }
                this.openSection(this.sections[nextIndex]);
				logger.debug(`<<< Exiting openNextSection(setAllow=${setAllow})`);
                return;
            }
        }
    },

    openPrevSection: function (setAllow, onlyAllowed) {
		logger.debug(`>>> Entering openPrevSection(setAllow=${setAllow},onlyAllowed=${onlyAllowed})`);
        var prevIndex = 0;
        for (section in this.sections) {
			logger.debug(`openPrevSection(setAllow=${setAllow},onlyAllowed=${onlyAllowed}): prevIndex → ${prevIndex}`);
            if (onlyAllowed) {
                //ensure that the section is allowed
                var tmp = parseInt(section) - 1;
                if (!isNaN(tmp) && $(this.sections[tmp]).hasClass('allow')) {
                    prevIndex = tmp;
                }
            } else {
                prevIndex = parseInt(section) - 1;
            }
            if (this.sections[section].id == this.currentSectionId && this.sections[prevIndex]) {
                if (setAllow) {
                    $(this.sections[prevIndex]).addClass('allow');
                }
                this.openSection(this.sections[prevIndex]);
				logger.debug(`<<< Exiting openPrevSection(setAllow=${setAllow},onlyAllowed=${onlyAllowed})`);
                return;
            }
        }
    },

    closeExistingSection: function () {
		logger.debug('>>> Entering closeExistingSection()');
        if (this.currentSectionId) {
            this.closeSection($('#' + this.currentSectionId));
			logger.debug('<<< Exiting closeExistingSection()');
        }
    }
};
