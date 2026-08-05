const logger = console;
/**
 * Globalize Runtime v1.7.0
 *
 * https://github.com/globalizejs/globalize
 *
 * Copyright OpenJS Foundation and other contributors
 * Released under the MIT license
 * https://jquery.org/license
 *
 * Date: 2021-08-02T11:53Z
 */
/*!
 * Globalize Runtime v1.7.0 2021-08-02T11:53Z Released under the MIT license
 * http://git.io/TrdQbw
 */
(function( root, factory ) {
logger.debug('>>> Entering function()');

	"use strict";

	// UMD returnExports
	if ( typeof define === "function" && define.amd ) {

		// AMD
		define([
			"../globalize-runtime",
			"./number",
			"./plural"
		], factory );
	} else if ( typeof exports === "object" ) {

		// Node, CommonJS
		module.exports = factory(
			require( "../globalize-runtime" ),
			require( "./number" ),
			require( "./plural" )
		);
	} else {

		// Extend global
		factory( root.Globalize );
	}
}(this, function( Globalize ) {



var formatMessage = Globalize._formatMessage,
	runtimeKey = Globalize._runtimeKey,
	validateParameterPresence = Globalize._validateParameterPresence,
	validateParameterTypeNumber = Globalize._validateParameterTypeNumber;


/**
 * format( value, numberFormatter, pluralGenerator, properties )
 *
 * @value [Number] The number to format
 *
 * @numberFormatter [String] A numberFormatter from Globalize.numberFormatter
 *
 * @pluralGenerator [String] A pluralGenerator from Globalize.pluralGenerator
 *
 * @properties [Object] containing relative time plural message.
 *
 * Format relative time.
 */
var relativeTimeFormat = function( value, numberFormatter, pluralGenerator, properties ) {
logger.debug(`>>> Entering relativeTimeFormat(value=${value},numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);

	var relativeTime,
		message = properties[ "relative-type-" + value ];

	if ( message ) {
		return message;
	}

	relativeTime = value <= 0 ? properties[ "relativeTime-type-past" ] :
		properties[ "relativeTime-type-future" ];

	value = Math.abs( value );

	logger.debug(`relativeTimeFormat(value=${value},numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties}): value → ${value}`);
	message = relativeTime[ "relativeTimePattern-count-" + pluralGenerator( value ) ];
	logger.debug(`relativeTimeFormat(value=${value},numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties}): message → ${message}`);
	logger.debug(`<<< Exiting relativeTimeFormat(value=${value},numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);
	return formatMessage( message, [ numberFormatter( value ) ] );
};




var relativeTimeFormatterFn = function( numberFormatter, pluralGenerator, properties ) {
	logger.debug(`>>> Entering relativeTimeFormatterFn(numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);
	return function relativeTimeFormatter( value ) {
	logger.debug(`>>> Entering relativeTimeFormatter(value=${value})`);
		validateParameterPresence( value, "value" );
		validateParameterTypeNumber( value, "value" );
		logger.debug(`<<< Exiting relativeTimeFormatterFn(numberFormatter=${numberFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);

		logger.debug(`<<< Exiting relativeTimeFormatter(value=${value})`);
		return relativeTimeFormat( value, numberFormatter, pluralGenerator, properties );
	};

};




Globalize._relativeTimeFormatterFn = relativeTimeFormatterFn;
logger.debug(`function(): Globalize._relativeTimeFormatterFn → ${Globalize._relativeTimeFormatterFn}`);

Globalize.formatRelativeTime =
Globalize.prototype.formatRelativeTime = function( value, unit, options ) {
	validateParameterPresence( value, "value" );
	validateParameterTypeNumber( value, "value" );

	return this.relativeTimeFormatter( unit, options )( value );
};

Globalize.relativeTimeFormatter =
Globalize.prototype.relativeTimeFormatter = function( unit, options ) {
	options = options || {};
	return Globalize[ runtimeKey( "relativeTimeFormatter", this._locale, [ unit, options ] ) ];
};

return Globalize;




}));
