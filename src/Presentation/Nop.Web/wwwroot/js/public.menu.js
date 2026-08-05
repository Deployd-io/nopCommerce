const logger = console;
var mainMenu = {
  init: function (togleSelector, activeClassName) {
	logger.debug(`>>> Entering init(togleSelector=${togleSelector},activeClassName=${activeClassName})`);
    const menuDropdowns = document.querySelectorAll(togleSelector);

	logger.debug(`init(togleSelector=${togleSelector},activeClassName=${activeClassName}): menuDropdowns → ${menuDropdowns}`);
    let toggleClass = function (event) {
	logger.debug(`>>> Entering toggleClass(event=${event})`);
      this.parentNode.classList.toggle(activeClassName);
      this.setAttribute('aria-expanded', this.getAttribute('aria-expanded') === 'true' ? 'false' : 'true')
    };
	logger.debug(`<<< Exiting toggleClass(event=${event})`);

    menuDropdowns.forEach((e) => {
      e.addEventListener("click", toggleClass.bind(e));
    })
  }
};
