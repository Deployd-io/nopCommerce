const logger = console;
/**
 * Globalize v1.7.0
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
 * Globalize v1.7.0 2021-08-02T11:53Z Released under the MIT license
 * http://git.io/TrdQbw
 */
(function( root, factory ) {
logger.debug('>>> Entering function()');

	// UMD returnExports
	if ( typeof define === "function" && define.amd ) {

		// AMD
		define([
			"cldr",
			"cldr/event"
		], factory );
	} else if ( typeof exports === "object" ) {

		// Node, CommonJS
		module.exports = factory( require( "cldrjs" ) );
	} else {

		// Global
		root.Globalize = factory( root.Cldr );
	}
}( this, function( Cldr ) {


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
		logger.debug('>>> Entering replace()');
		name = name.replace( /^{([^}]*)}$/, "$1" );
		return toString( data[ name ] );
	});

	logger.debug(`replace(): error.code → ${error.code}`);
	return message;
};

logger.debug(`<<< Exiting formatMessage(message=${message},data=${data})`);



var objectExtend = function() {
	logger.debug('>>> Entering objectExtend()');
	var destination = arguments[ 0 ],
		sources = [].slice.call( arguments, 1 );

	sources.forEach(function( source ) {
		logger.debug('>>> Entering forEach()');
		var prop;
		for ( prop in source ) {
			destination[ prop ] = source[ prop ];
		}
	});

	logger.debug('<<< Exiting forEach()');
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

	objectExtend( error, attributes );

	logger.debug(`createError(code=${code},message=${message},attributes=${attributes}): error.code → ${error.code}`);
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
	logger.debug('>>> Entering map()');
		return part.value;
	}).join( "" );
	logger.debug(`<<< Exiting partsJoin(parts=${parts})`);
};




var runtimeStringify = function( args ) {
	logger.debug(`>>> Entering runtimeStringify(args=${args})`);
	return JSON.stringify( args, function( _key, value ) {
	logger.debug(`>>> Entering stringify(args=${args})`);
		if ( value && value.runtimeKey ) {
			return value.runtimeKey;
		}
		return value;
	} );
	logger.debug(`<<< Exiting runtimeStringify(args=${args})`);
};




// Based on http://stackoverflow.com/questions/7616461/generate-a-hash-from-string-in-javascript-jquery
var stringHash = function( str ) {
	logger.debug(`>>> Entering stringHash(str=${str})`);
	return [].reduce.call( str, function( hash, i ) {
	logger.debug(`>>> Entering call(str=${str})`);
		var chr = i.charCodeAt( 0 );
		hash = ( ( hash << 5 ) - hash ) + chr;
		return hash | 0;
	}, 0 );
	logger.debug(`<<< Exiting stringHash(str=${str})`);
};




var runtimeKey = function( fnName, locale, args, argsStr ) {
	var hash;
	argsStr = argsStr || runtimeStringify( args );
	hash = stringHash( fnName + locale + argsStr );
	logger.debug(`runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr}): argsStr → ${argsStr}`);
	logger.debug(`runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr}): hash → ${hash}`);
	logger.debug(`<<< Exiting runtimeKey(fnName=${fnName},locale=${locale},args=${args},argsStr=${argsStr})`);
	return hash > 0 ? "a" + hash : "b" + Math.abs( hash );
};




var functionName = function( fn ) {
	logger.debug(`>>> Entering functionName(fn=${fn})`);
	if ( fn.name !== undefined ) {
		return fn.name;
	}

	// fn.name is not supported by IE.
	var matches = /^function\s+([\w\$]+)\s*\(/.exec( fn.toString() );

	if ( matches && matches.length > 0 ) {
		logger.debug(`<<< Exiting functionName(fn=${fn})`);
		return matches[ 1 ];
	}
};




var runtimeBind = function( args, cldr, fn, runtimeArgs ) {
logger.debug(`>>> Entering runtimeBind(args=${args},cldr=${cldr},fn=${fn},runtimeArgs=${runtimeArgs})`);

	var argsStr = runtimeStringify( args ),
		fnName = functionName( fn ),
		locale = cldr.locale;
		logger.debug(`runtimeBind(args=${args},cldr=${cldr},fn=${fn},runtimeArgs=${runtimeArgs}): argsStr → ${argsStr}`);

	// If name of the function is not available, this is most likely due to uglification,
	// which most likely means we are in production, and runtimeBind here is not necessary.
	if ( !fnName ) {
		return fn;
	}

	fn.runtimeKey = runtimeKey( fnName, locale, null, argsStr );

	fn.generatorString = function() {
		return "Globalize(\"" + locale + "\")." + fnName + "(" + argsStr.slice( 1, -1 ) + ")";
	};

	fn.runtimeArgs = runtimeArgs;
	logger.debug(`runtimeBind(args=${args},cldr=${cldr},fn=${fn},runtimeArgs=${runtimeArgs}): fn.runtimeArgs → ${fn.runtimeArgs}`);

	logger.debug(`<<< Exiting runtimeBind(args=${args},cldr=${cldr},fn=${fn},runtimeArgs=${runtimeArgs})`);
	return fn;
};




var validate = function( code, message, check, attributes ) {
	if ( !check ) {
		throw createError( code, message, attributes );
	}
};




var alwaysArray = function( stringOrArray ) {
	logger.debug(`>>> Entering alwaysArray(stringOrArray=${stringOrArray})`);
	return Array.isArray( stringOrArray ) ? stringOrArray : stringOrArray ? [ stringOrArray ] : [];
};




var validateCldr = function( path, value, options ) {
	logger.debug(`>>> Entering validateCldr(path=${path},value=${value},options=${options})`);
	var skipBoolean;
	options = options || {};

	skipBoolean = alwaysArray( options.skip ).some(function( pathRe ) {
		logger.debug('>>> Entering some()');
		return pathRe.test( path );
	});

	validate( "E_MISSING_CLDR", "Missing required CLDR content `{path}`.", value || skipBoolean, {
		path: path
	});
	logger.debug(`<<< Exiting validateCldr(path=${path},value=${value},options=${options})`);
};




var validateDefaultLocale = function( value ) {
	logger.debug(`>>> Entering validateDefaultLocale(value=${value})`);
	validate( "E_DEFAULT_LOCALE_NOT_DEFINED", "Default locale has not been defined.",
		value !== undefined, {} );
		logger.debug(`<<< Exiting validateDefaultLocale(value=${value})`);
};




var validateParameterPresence = function( value, name ) {
	logger.debug(`>>> Entering validateParameterPresence(value=${value},name=${name})`);
	validate( "E_MISSING_PARAMETER", "Missing required parameter `{name}`.",
		value !== undefined, { name: name });
		logger.debug(`<<< Exiting validateParameterPresence(value=${value},name=${name})`);
};




/**
 * range( value, name, minimum, maximum )
 *
 * @value [Number].
 *
 * @name [String] name of variable.
 *
 * @minimum [Number]. The lowest valid value, inclusive.
 *
 * @maximum [Number]. The greatest valid value, inclusive.
 */
var validateParameterRange = function( value, name, minimum, maximum ) {
	logger.debug(`>>> Entering validateParameterRange(value=${value},name=${name},minimum=${minimum},maximum=${maximum})`);
	validate(
		"E_PAR_OUT_OF_RANGE",
		"Parameter `{name}` has value `{value}` out of range [{minimum}, {maximum}].",
		value === undefined || value >= minimum && value <= maximum,
		{
			maximum: maximum,
			minimum: minimum,
			name: name,
			value: value
		}
	);
	logger.debug(`<<< Exiting validateParameterRange(value=${value},name=${name},minimum=${minimum},maximum=${maximum})`);
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




var validateParameterTypeLocale = function( value, name ) {
	logger.debug(`>>> Entering validateParameterTypeLocale(value=${value},name=${name})`);
	validateParameterType(
		value,
		name,
		value === undefined || typeof value === "string" || value instanceof Cldr,
		"String or Cldr instance"
	);
	logger.debug(`<<< Exiting validateParameterTypeLocale(value=${value},name=${name})`);
};




/**
 * Function inspired by jQuery Core, but reduced to our use case.
 */
var isPlainObject = function( obj ) {
	logger.debug(`>>> Entering isPlainObject(obj=${obj})`);
	logger.debug(`<<< Exiting isPlainObject(obj=${obj})`);
	return obj !== null && "" + obj === "[object Object]";
};




var validateParameterTypePlainObject = function( value, name ) {
	logger.debug(`>>> Entering validateParameterTypePlainObject(value=${value},name=${name})`);
	validateParameterType(
		value,
		name,
		value === undefined || isPlainObject( value ),
		"Plain Object"
	);
	logger.debug(`<<< Exiting validateParameterTypePlainObject(value=${value},name=${name})`);
};




var alwaysCldr = function( localeOrCldr ) {
	logger.debug(`>>> Entering alwaysCldr(localeOrCldr=${localeOrCldr})`);
	logger.debug(`<<< Exiting alwaysCldr(localeOrCldr=${localeOrCldr})`);
	return localeOrCldr instanceof Cldr ? localeOrCldr : new Cldr( localeOrCldr );
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




function validateLikelySubtags( cldr ) {
	logger.debug(`>>> Entering validateLikelySubtags(cldr=${cldr})`);
	cldr.once( "get", validateCldr );
	cldr.get( "supplemental/likelySubtags" );
	logger.debug(`<<< Exiting validateLikelySubtags(cldr=${cldr})`);
}

/**
 * [new] Globalize( locale|cldr )
 *
 * @locale [String]
 *
 * @cldr [Cldr instance]
 *
 * Create a Globalize instance.
 */
function Globalize( locale ) {
	logger.debug(`>>> Entering Globalize(locale=${locale})`);
	if ( !( this instanceof Globalize ) ) {
		return new Globalize( locale );
	}

	validateParameterPresence( locale, "locale" );
	validateParameterTypeLocale( locale, "locale" );

	this.cldr = alwaysCldr( locale );

	validateLikelySubtags( this.cldr );
	logger.debug(`<<< Exiting Globalize(locale=${locale})`);
}

/**
 * Globalize.load( json, ... )
 *
 * @json [JSON]
 *
 * Load resolved or unresolved cldr data.
 * Somewhat equivalent to previous Globalize.addCultureInfo(...).
 */
Globalize.load = function() {

	// validations are delegated to Cldr.load().
	Cldr.load.apply( Cldr, arguments );
};

/**
 * Globalize.locale( [locale|cldr] )
 *
 * @locale [String]
 *
 * @cldr [Cldr instance]
 *
 * Set default Cldr instance if locale or cldr argument is passed.
 *
 * Return the default Cldr instance.
 */
Globalize.locale = function( locale ) {
	validateParameterTypeLocale( locale, "locale" );

	if ( arguments.length ) {
		this.cldr = alwaysCldr( locale );
		validateLikelySubtags( this.cldr );
	}
	return this.cldr;
};

/**
 * Optimization to avoid duplicating some internal functions across modules.
 */
Globalize._alwaysArray = alwaysArray;
Globalize._createError = createError;
Globalize._formatMessage = formatMessage;
Globalize._formatMessageToParts = formatMessageToParts;
Globalize._isPlainObject = isPlainObject;
Globalize._objectExtend = objectExtend;
Globalize._partsJoin = partsJoin;
Globalize._partsPush = partsPush;
Globalize._regexpEscape = regexpEscape;
Globalize._runtimeBind = runtimeBind;
Globalize._stringPad = stringPad;
Globalize._validate = validate;
Globalize._validateCldr = validateCldr;
Globalize._validateDefaultLocale = validateDefaultLocale;
Globalize._validateParameterPresence = validateParameterPresence;
Globalize._validateParameterRange = validateParameterRange;
Globalize._validateParameterTypePlainObject = validateParameterTypePlainObject;
Globalize._validateParameterType = validateParameterType;

return Globalize;




}));
