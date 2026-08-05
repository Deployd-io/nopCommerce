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
			"./number"
		], factory );
	} else if ( typeof exports === "object" ) {

		// Node, CommonJS
		module.exports = factory(
			require( "../globalize-runtime" ),
			require( "./number" )
		);
	} else {

		// Extend global
		factory( root.Globalize );
	}
}(this, function( Globalize ) {



var formatMessageToParts = Globalize._formatMessageToParts,
	partsJoin = Globalize._partsJoin,
	partsPush = Globalize._partsPush,
	runtimeKey = Globalize._runtimeKey,
	validateParameterPresence = Globalize._validateParameterPresence,
	validateParameterTypeNumber = Globalize._validateParameterTypeNumber;


var currencyFormatterFn = function( currencyToPartsFormatter ) {
	logger.debug(`>>> Entering currencyFormatterFn(currencyToPartsFormatter=${currencyToPartsFormatter})`);
	return function currencyFormatter( value ) {
	logger.debug(`>>> Entering currencyFormatter(value=${value})`);
		logger.debug(`<<< Exiting currencyFormatter(value=${value})`);
		return partsJoin( currencyToPartsFormatter( value ));
	};
	logger.debug(`<<< Exiting currencyFormatterFn(currencyToPartsFormatter=${currencyToPartsFormatter})`);
};




/**
 * nameFormat( formattedNumber, pluralForm, properties )
 *
 * Return the appropriate name form currency format.
 */
var currencyNameFormat = function( formattedNumber, pluralForm, properties ) {
	logger.debug(`>>> Entering currencyNameFormat(formattedNumber=${formattedNumber},pluralForm=${pluralForm},properties=${properties})`);
	var displayName, unitPattern,
		parts = [],
		displayNames = properties.displayNames || {},
		unitPatterns = properties.unitPatterns;

	displayName = displayNames[ "displayName-count-" + pluralForm ] ||
		displayNames[ "displayName-count-other" ] ||
		displayNames.displayName ||
		properties.currency;
	unitPattern = unitPatterns[ "unitPattern-count-" + pluralForm ] ||
		unitPatterns[ "unitPattern-count-other" ];

	formatMessageToParts( unitPattern, [ formattedNumber, displayName ]).forEach(function( part ) {
		logger.debug(`>>> Entering forEach(unitPattern=${unitPattern},displayName=${displayName})`);
		if ( part.type === "variable" && part.name === "0" ) {
			part.value.forEach(function( part ) {
				partsPush( parts, part.type, part.value );
			});
		} else if ( part.type === "variable" && part.name === "1" ) {
			partsPush( parts, "currency", part.value );
		} else {
			partsPush( parts, "literal", part.value );
		}
	});

	logger.debug(`forEach(part.type=${part.type}): part.value → ${part.value}`);
	logger.debug(`<<< Exiting forEach(unitPattern=${unitPattern},displayName=${displayName})`);
	return parts;
};




/**
 * symbolFormat( parts, symbol )
 *
 * Return the appropriate symbol/account form format.
 */
var currencySymbolFormat = function( parts, symbol ) {
	logger.debug(`>>> Entering currencySymbolFormat(parts=${parts},symbol=${symbol})`);
	parts.forEach(function( part ) {
		if ( part.type === "currency" ) {
			part.value = symbol;
		}
	});
	logger.debug(`currencySymbolFormat(parts=${parts},symbol=${symbol}): part.value → ${part.value}`);
	logger.debug(`<<< Exiting currencySymbolFormat(parts=${parts},symbol=${symbol})`);
	return parts;
};

logger.debug(`function(): part.value → ${part.value}`);



var currencyToPartsFormatterFn = function( numberToPartsFormatter, pluralGenerator, properties ) {
	logger.debug(`>>> Entering currencyToPartsFormatterFn(numberToPartsFormatter=${numberToPartsFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);
	var fn;

	// Return formatter when style is "name".
	if ( pluralGenerator && properties ) {
		fn = function currencyToPartsFormatter( value ) {
			logger.debug(`>>> Entering currencyToPartsFormatter(value=${value})`);
			validateParameterPresence( value, "value" );
			validateParameterTypeNumber( value, "value" );
			logger.debug(`<<< Exiting currencyToPartsFormatter(value=${value})`);
			return currencyNameFormat(
				numberToPartsFormatter( value ),
				pluralGenerator( value ),
				properties
			);
		};

	// Return formatter when style is "symbol", "accounting", or "code".
	} else {
		fn = function currencyToPartsFormatter( value ) {

			// 1: Reusing pluralGenerator argument, but in this case it is actually `symbol`
			return currencySymbolFormat( numberToPartsFormatter( value ), pluralGenerator /* 1 */ );
		};
	}

	logger.debug(`<<< Exiting currencyToPartsFormatterFn(numberToPartsFormatter=${numberToPartsFormatter},pluralGenerator=${pluralGenerator},properties=${properties})`);
	return fn;
};




Globalize._currencyFormatterFn = currencyFormatterFn;
logger.debug(`function(): Globalize._currencyFormatterFn → ${Globalize._currencyFormatterFn}`);
logger.debug(`function(): Globalize._currencyNameFormat → ${Globalize._currencyNameFormat}`);
logger.debug(`function(): Globalize._currencyToPartsFormatterFn → ${Globalize._currencyToPartsFormatterFn}`);
Globalize._currencyNameFormat = currencyNameFormat;
Globalize._currencyToPartsFormatterFn = currencyToPartsFormatterFn;

Globalize.currencyFormatter =
Globalize.prototype.currencyFormatter = function( currency, options ) {
	options = options || {};
	return Globalize[ runtimeKey( "currencyFormatter", this._locale, [ currency, options ] ) ];
};

Globalize.currencyToPartsFormatter =
Globalize.prototype.currencyToPartsFormatter = function( currency, options ) {
	options = options || {};
	return Globalize[
		runtimeKey( "currencyToPartsFormatter", this._locale, [ currency, options ] )
	];
};

Globalize.formatCurrency =
Globalize.prototype.formatCurrency = function( value, currency, options ) {
	validateParameterPresence( value, "value" );
	validateParameterTypeNumber( value, "value" );

	return this.currencyFormatter( currency, options )( value );
};

Globalize.formatCurrencyToParts =
Globalize.prototype.formatCurrencyToParts = function( value, currency, options ) {
	validateParameterPresence( value, "value" );
	validateParameterTypeNumber( value, "value" );

	return this.currencyToPartsFormatter( currency, options )( value );
};

return Globalize;




}));
