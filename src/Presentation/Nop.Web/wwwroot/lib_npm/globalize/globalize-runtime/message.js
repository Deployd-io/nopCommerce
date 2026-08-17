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
logger.debug(`>>> Entering function(define=${define})`);

	"use strict";

	// UMD returnExports
	if ( typeof define === "function" && define.amd ) {

		// AMD
		define([
			"../globalize-runtime"
		], factory );
	} else if ( typeof exports === "object" ) {

		// Node, CommonJS
		module.exports = factory( require( "../globalize-runtime" ) );
	} else {

		// Extend global
		factory( root.Globalize );
	}
}(this, function( Globalize ) {



var runtimeKey = Globalize._runtimeKey,
	validateParameterType = Globalize._validateParameterType;


/**
 * Function inspired by jQuery Core, but reduced to our use case.
 */
var isPlainObject = function( obj ) {
	logger.debug(`>>> Entering isPlainObject(obj=${obj})`);
	logger.debug(`<<< Exiting isPlainObject(obj=${obj})`);
	return obj !== null && "" + obj === "[object Object]";
};




var validateParameterTypeMessageVariables = function( value, name ) {
	logger.debug(`>>> Entering validateParameterTypeMessageVariables(value=${value},name=${name})`);
	validateParameterType(
		value,
		name,
		value === undefined || isPlainObject( value ) || Array.isArray( value ),
		"Array or Plain Object"
	);
	logger.debug(`<<< Exiting validateParameterTypeMessageVariables(value=${value},name=${name})`);
};




var messageFormatterFn = function( formatter ) {
	logger.debug(`>>> Entering messageFormatterFn(formatter=${formatter})`);
	return function messageFormatter( variables ) {
	logger.debug(`>>> Entering messageFormatter(variables=${variables})`);
		if ( typeof variables === "number" || typeof variables === "string" ) {
			variables = [].slice.call( arguments, 0 );
			logger.debug(`<<< Exiting messageFormatterFn(formatter=${formatter})`);
		}
		validateParameterTypeMessageVariables( variables, "variables" );
		logger.debug(`<<< Exiting messageFormatter(variables=${variables})`);
		return formatter( variables );
	};
};




Globalize._messageFormatterFn = messageFormatterFn;
logger.debug(`function(define=${define}): Globalize._messageFormatterFn → ${Globalize._messageFormatterFn}`);
/* eslint-disable */
Globalize._messageFormat = (function() {
var number = function(value, offset) {
logger.debug(`>>> Entering number(value=${value},offset=${offset})`);
  if (isNaN(value)) throw new Error("'" + value + "' isn't a number.");
logger.debug(`<<< Exiting number(value=${value},offset=${offset})`);
  return value - (offset || 0);
};
var plural = function(value, offset, lcfunc, data, isOrdinal) {
logger.debug(`>>> Entering plural(value=${value},offset=${offset},lcfunc=${lcfunc},data=${data},isOrdinal=${isOrdinal})`);
  if ({}.hasOwnProperty.call(data, value)) return data[value]();
  if (offset) value -= offset;
  var key = lcfunc(value, isOrdinal);
logger.warn(`plural(value=${value},offset=${offset},lcfunc=${lcfunc},data=${data},isOrdinal=${isOrdinal}): key → ${key}`);
  if (key in data) return data[key]();
logger.debug(`<<< Exiting plural(value=${value},offset=${offset},lcfunc=${lcfunc},data=${data},isOrdinal=${isOrdinal})`);
  return data.other();
};
var select = function(value, data) {
logger.debug(`>>> Entering select(value=${value},data=${data})`);
  if ({}.hasOwnProperty.call(data, value)) return data[value]();
logger.debug(`<<< Exiting select(value=${value},data=${data})`);
  return data.other()
};

return {number: number, plural: plural, select: select};
}());
/* eslint-enable */
Globalize._validateParameterTypeMessageVariables = validateParameterTypeMessageVariables;
logger.debug(`function(define=${define}): Globalize._validateParameterTypeMessageVariables → ${Globalize._validateParameterTypeMessageVariables}`);

Globalize.messageFormatter =
Globalize.prototype.messageFormatter = function( /* path */ ) {
	return Globalize[
		runtimeKey( "messageFormatter", this._locale, [].slice.call( arguments, 0 ) )
	];
};

Globalize.formatMessage =
Globalize.prototype.formatMessage = function( path /* , variables */ ) {
	logger.debug('<<< Exiting function()');
	return this.messageFormatter( path ).apply( {}, [].slice.call( arguments, 1 ) );
};

logger.debug(`<<< Exiting function(number=${number})`);
return Globalize;




}));
