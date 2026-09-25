var logger = (typeof logger !== 'undefined' && logger) || console;

function createPopoverButton(popover, text, buttonClass, iconClass, clickListener, isForward) {
logger.error(`>>> Entering createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward})`);
  let button = document.createElement("button");
  button.classList.add(buttonClass);
logger.error(`createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward}): button → ${button}`);

  let icon = document.createElement("i");
logger.error(`createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward}): icon → ${icon}`);
  icon.classList.add("fas", iconClass);
  icon.style.fontFamily = "FontAwesome";

  let buttonText = document.createElement("div");
logger.error(`createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward}): buttonText → ${buttonText}`);
logger.debug(`createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward}): buttonText.textContent → ${buttonText.textContent}`);
  buttonText.classList.add("button-text");
  buttonText.textContent = text;

  var childrenElements = [icon, buttonText].sort((a, b) => isForward ? -1 : 1)
  button.append(...childrenElements);
logger.error(`createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward}): childrenElements → ${childrenElements}`);

  popover.footerButtons.appendChild(button);
  button.addEventListener("click", clickListener);
logger.warn(`<<< Exiting createPopoverButton(popover=${popover},text=${text},buttonClass=${buttonClass},iconClass=${iconClass},clickListener=${clickListener},isForward=${isForward})`);
}

const AdminTourBuilder = {
  init: function (localizedData, nextPageEntityId, nextPageUrl, steps) {
	logger.error(`>>> Entering init(localizedData=${localizedData},nextPageEntityId=${nextPageEntityId},nextPageUrl=${nextPageUrl},steps=${steps})`);
    this.steps = steps;

    const tour = window.driver.js.driver({
      overlayOpacity: 0.5,
      disableActiveInteraction: true,
      popoverClass: 'admin-area-tour',
      popoverOffset: 15,
      stagePadding: 3,

      smoothScroll: true,

      showProgress: true,
      progressText: "{{current}}/{{total}}",

      steps: steps,

      onPopoverRender: (popover, { config, state }) => {

		logger.debug(`init(localizedData=${localizedData},nextPageEntityId=${nextPageEntityId},nextPageUrl=${nextPageUrl},steps=${steps}): this.steps → ${this.steps}`);
		logger.debug(`init(localizedData=${localizedData},nextPageEntityId=${nextPageEntityId},nextPageUrl=${nextPageUrl},steps=${steps}): steps → ${steps}`);
        var buttonsContainer = popover.footerButtons;
        while (buttonsContainer.firstChild) {
          buttonsContainer.removeChild(buttonsContainer.firstChild);
        }

        var isRightToLeft = getComputedStyle(document.getElementById('driver-popover-description')).direction == "rtl";

        if (tour.hasPreviousStep() && localizedData.Back) {
          createPopoverButton(popover, localizedData.Back, "button-back", "fa-chevron-" + (isRightToLeft ? "right" : "left"), () => tour.movePrevious(), false);
        }

        if (tour.hasNextStep() && localizedData.NextStep) {
          createPopoverButton(popover, localizedData.NextStep, "button-next", "fa-chevron-" + (isRightToLeft ? "left" : "right"), () => tour.moveNext(), true);
        }

        if (tour.isLastStep() && nextPageUrl && localizedData.NextPage) {
          createPopoverButton(popover, localizedData.NextPage, "button-next-page", "fa-angle-double-" + (isRightToLeft ? "left" : "right"), () => {
            if (nextPageUrl)
              window.location = nextPageUrl + nextPageEntityId;

            tour.destroy();
          }, true);
        }

        if (tour.isLastStep() && localizedData.Done) {
          createPopoverButton(popover, localizedData.Done, "button-done", "fa-check", () => tour.destroy());
        }
      }
    })

	logger.warn(`<<< Exiting init(localizedData=${localizedData},nextPageEntityId=${nextPageEntityId},nextPageUrl=${nextPageUrl},steps=${steps})`);
    return tour;
  }
}