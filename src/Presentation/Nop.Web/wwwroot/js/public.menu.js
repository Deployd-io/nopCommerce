var logger = (typeof logger !== 'undefined' && logger) || console;
var mainMenu = {
  init: function (togleSelector, activeClassName) {
    const menuDropdowns = document.querySelectorAll(togleSelector);

    let toggleClass = function (event) {
	logger.error(`>>> Entering toggleClass(event=${event})`);
      this.parentNode.classList.toggle(activeClassName);
      this.setAttribute('aria-expanded', this.getAttribute('aria-expanded') === 'true' ? 'false' : 'true')
	logger.warn(`<<< Exiting toggleClass(event=${event})`);
    };

    menuDropdowns.forEach((e) => {
      e.addEventListener("click", toggleClass.bind(e));
    })
  }
};
