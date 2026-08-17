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
		define( factory );
	} else if ( typeof exports === "object" ) {

		// Node, CommonJS
		module.exports = factory();
	} else {

		// Globalize
		root.Globalize = factory();
	}
}( this, function() {




/**
 * A toString method that outputs meaningful values for objects or arrays and
 * still performs as fast as a plain string in case variable is string, or as
 * fast as `"" + number` in case variable is a number.
 * Ref: http://jsperf.com/my-stringify
 */
var toString = function( variable ) {
	logger.debug(`>>> Entering toString(variable=${variable})`);
	logger.debug(`<<< Exiting toString(variable=${variable})`);
	return typeof variable === "string" ? variable : ( typeof variable === "number" ? "" +
		variable : JSON.stringify( variable ) );
};




/**
 * formatMessage( message, data )
 *
 * @message [String] A message with optional {vars} to be replaced.
 *
 * @data [Array or JSON] Object with replacing-variables content.
 *
 * Return the formatted message. For example:
 *
 * - formatMessage( "{0} second", [ 1 ] ); // 1 second
 *
 * - formatMessage( "{0}/{1}", ["m", "s"] ); // m/s
 *
 * - formatMessage( "{name} <{email}>", {
 *     name: "Foo",
 *     email: "bar@baz.qux"
 *   }); // Foo <bar@baz.qux>
 */
var formatMessage = function( message, data ) {
logger.debug(`>>> Entering formatMessage(message=${message},data=${data})`);

	// Replace {attribute}'s
	message = message.replace( /{[0-9a-zA-Z-_. ]+}/g, function( name ) {
		name = name.replace( /^{([^}]*)}$/, "$1" );
		return toString( data[ name ] );
	});

	logger.debug(`<<< Exiting formatMessage(message=${message},data=${data})`);
	return message;
};




var objectExtend = function() {
	logger.debug('>>> Entering objectExtend()');
	var destination = arguments[ 0 ],
		sources = [].slice.call( arguments, 1 );

	sources.forEach(function( source ) {
		logger.debug(`>>> Entering forEach(prop=${prop})`);
		var prop;
		for ( prop in source ) {
			destination[ prop ] = source[ prop ];
		}
	});

	logger.debug(`<<< Exiting forEach(prop=${prop})`);
	return destination;
};




var createError = function( code, message, attributes ) {
	logger.debug(`>>> Entering createError(code=${code},message=${message},attributes=${attributes})`);
	var error;

	message = code + ( message ? ": " + formatMessage( message, attributes ) : "" );
	error = new Error( message );
	logger.debug(`createError(code=${code},message=${message},attributes=${attributes}): message → ${message}`);
	logger.debug(`createError(code=${code},message=${message},attributes=${attributes}): error → ${error}`);
	error.code = code;
	logger.debug(`createError(code=${code},message=${message},attributes=${attributes}): error.code → ${error.code}`);

	objectExtend( error, attributes );

	logger.debug(`<<< Exiting createError(code=${code},message=${message},attributes=${attributes})`);
	return error;
};




/**
 * Pushes part to parts array, concat two consecutive parts of the same type.
 */
var partsPush = function( parts, type, value ) {
logger.debug(`>>> Entering partsPush(parts=${parts},type=${type},value=${value})`);

		// Concat two consecutive parts of same type
		if ( parts.length && parts[ parts.length - 1 ].type === type ) {
			parts[ parts.length - 1 ].value += value;
			return;
		}

		parts.push( { type: type, value: value } );
		logger.debug(`<<< Exiting partsPush(parts=${parts},type=${type},value=${value})`);
};




/**
 * formatMessage( message, data )
 *
 * @message [String] A message with optional {vars} to be replaced.
 *
 * @data [Array or JSON] Object with replacing-variables content.
 *
 * Return the formatted message. For example:
 *
 * - formatMessage( "{0} second", [ 1 ] );
 * > [{type: "variable", value: "1", name: "0"}, {type: "literal", value: " second"}]
 *
 * - formatMessage( "{0}/{1}", ["m", "s"] );
 * > [
 *     { type: "variable", value: "m", name: "0" },
 *     { type: "literal", value: " /" },
 *     { type: "variable", value: "s", name: "1" }
 *   ]
 */
var formatMessageToParts = function( message, data ) {
logger.debug(`>>> Entering formatMessageToParts(message=${message},data=${data})`);

	var lastOffset = 0,
		parts = [];

	// Create parts.
	message.replace( /{[0-9a-zA-Z-_. ]+}/g, function( nameIncludingBrackets, offset ) {
		var name = nameIncludingBrackets.slice( 1, -1 );
		partsPush( parts, "literal", message.slice( lastOffset, offset ));
		partsPush( parts, "variable", data[ name ] );
		parts[ parts.length - 1 ].name = name;
		lastOffset += offset + nameIncludingBrackets.length;
	});

	// Skip empty ones such as `{ type: 'literal', value: '' }`.
	return parts.filter(function( part ) {
	logger.debug('>>> Entering filter()');
		return part.value !== "";
	});
	logger.debug(`<<< Exiting formatMessageToParts(message=${message},data=${data})`);
};




/**
 * Returns joined parts values.
 */
var partsJoin = function( parts ) {
	logger.debug(`>>> Entering partsJoin(parts=${parts})`);
	return parts.map( function( part ) {
		logger.debug(`<<< Exiting partsJoin(parts=${parts})`);
		return part.value;
	}).join( "" );
};




var runtimeStringify = function( args ) {
	logger.debug(`>>> Entering runtimeStringify(args=${args})`);
	return JSON.stringify( args, function( _key, value ) {
		if ( value && value.runtimeKey ) {
			logger.debug(`<<< Exiting runtimeStringify(args=${args})`);
			return value.runtimeKey;
		}
		return value;
	} );
};




// Based on http://stackoverflow.com/questions/7616461/generate-a-hash-from-string-in-javascript-jquery
var stringHash = function( str ) {
	logger.debug(`>>> Entering stringHash(str=${str})`);
	return [].reduce.call( str, function( hash, i ) {
		var chr = i.charCodeAt( 0 );
		hash = ( ( hash << 5 ) - hash ) + chr;
		return hash | 0;
	}, 0 );
	logger.debug(`<<< Exiting stringHash(str=${str})`);
};




var runtimeKey = function( fnName, locale, args, argsStr ) {
	logger.debug(`>>> Entering runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr})`);
	var hash;
	argsStr = argsStr || runtimeStringify( args );
	hash = stringHash( fnName + locale + argsStr );
	logger.debug(`runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr}): argsStr → ${argsStr}`);
	logger.debug(`<<< Exiting runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr})`);
	return hash > 0 ? "a" + hash : "b" + Math.abs( hash );
};




var validate = function( code, message, check, attributes ) {
	if ( !check ) {
		throw createError( code, message, attributes );
	}
};




var validateParameterPresence = function( value, name ) {
	logger.debug(`>>> Entering validateParameterPresence(value=${value},name=${name})`);
	validate( "E_MISSING_PARAMETER", "Missing required parameter `{name}`.",
		value !== undefined, { name: name });
		logger.debug(`<<< Exiting validateParameterPresence(value=${value},name=${name})`);
};




var validateParameterType = function( value, name, check, expected ) {
	logger.debug(`>>> Entering validateParameterType(value=${value},name=${name},check=${check},expected=${expected})`);
	validate(
		"E_INVALID_PAR_TYPE",
		"Invalid `{name}` parameter ({value}). {expected} expected.",
		check,
		{
			expected: expected,
			name: name,
			value: value
		}
	);
	logger.debug(`<<< Exiting validateParameterType(value=${value},name=${name},check=${check},expected=${expected})`);
};




var validateParameterTypeString = function( value, name ) {
	logger.debug(`>>> Entering validateParameterTypeString(value=${value},name=${name})`);
	validateParameterType(
		value,
		name,
		value === undefined || typeof value === "string",
		"a string"
	);
	logger.debug(`<<< Exiting validateParameterTypeString(value=${value},name=${name})`);
};




// ref: https://developer.mozilla.org/en-US/docs/Web/JavaScript/Guide/Regular_Expressions?redirectlocale=en-US&redirectslug=JavaScript%2FGuide%2FRegular_Expressions
var regexpEscape = function( string ) {
	logger.debug(`>>> Entering regexpEscape(string=${string})`);
	logger.debug(`<<< Exiting regexpEscape(string=${string})`);
	return string.replace( /([.*+?^=!:${}()|\[\]\/\\])/g, "\\$1" );
};




var stringPad = function( str, count, right ) {
	logger.debug(`>>> Entering stringPad(str=${str},count=${count},right=${right})`);
	var length;
	if ( typeof str !== "string" ) {
		str = String( str );
	}
	for ( length = str.length; length < count; length += 1 ) {
		str = ( right ? ( str + "0" ) : ( "0" + str ) );
	}
	logger.debug(`stringPad(str=${str},count=${count},right=${right}): str → ${str}`);
	logger.debug(`<<< Exiting stringPad(str=${str},count=${count},right=${right})`);
	return str;
};




function Globalize( locale ) {
	logger.debug(`>>> Entering Globalize(locale=${locale})`);
	if ( !( this instanceof Globalize ) ) {
		logger.debug(`<<< Exiting Globalize(locale=${locale})`);
		return new Globalize( locale );
	}

	validateParameterPresence( locale, "locale" );
	validateParameterTypeString( locale, "locale" );

	this._locale = locale;
	logger.debug(`Globalize(locale=${locale}): this._locale → ${this._locale}`);
}

Globalize.locale = function( locale ) {
	validateParameterTypeString( locale, "locale" );

	if ( arguments.length ) {
		this._locale = locale;
	}
	logger.debug(`function(locale=${locale}): this._locale → ${this._locale}`);
	return this._locale;
};

Globalize._createError = createError;
Globalize._formatMessage = formatMessage;
Globalize._formatMessageToParts = formatMessageToParts;
Globalize._partsJoin = partsJoin;
Globalize._partsPush = partsPush;
Globalize._regexpEscape = regexpEscape;
Globalize._runtimeKey = runtimeKey;
Globalize._stringPad = stringPad;
Globalize._validateParameterPresence = validateParameterPresence;
Globalize._validateParameterTypeString = validateParameterTypeString;
Globalize._validateParameterType = validateParameterType;

return Globalize;




}));
