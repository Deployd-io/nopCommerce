//! moment.js locale configuration
//! locale : Urdu [ur]
//! author : Sawood Alam : https://github.com/ibnesayeed
//! author : Zack : https://github.com/ZackVision

import moment from '../moment';
const logger = console;

var months = [
        'جنوری',
        'فروری',
        'مارچ',
        'اپریل',
        'مئی',
        'جون',
        'جولائی',
        'اگست',
        'ستمبر',
        'اکتوبر',
        'نومبر',
        'دسمبر',
    ],
    days = ['اتوار', 'پیر', 'منگل', 'بدھ', 'جمعرات', 'جمعہ', 'ہفتہ'];

export default moment.defineLocale('ur', {
    months: months,
    monthsShort: months,
    weekdays: days,
    weekdaysShort: days,
    weekdaysMin: days,
    longDateFormat: {
        LT: 'HH:mm',
        LTS: 'HH:mm:ss',
        L: 'DD/MM/YYYY',
        LL: 'D MMMM YYYY',
        LLL: 'D MMMM YYYY HH:mm',
        LLLL: 'dddd، D MMMM YYYY HH:mm',
    },
    meridiemParse: /صبح|شام/,
    isPM: function (input) {
		logger.debug(`>>> Entering isPM(input=${input})`);
		logger.debug(`<<< Exiting isPM(input=${input})`);
        return 'شام' === input;
    },
    meridiem: function (hour, minute, isLower) {
		logger.debug(`>>> Entering meridiem(hour=${hour},minute=${minute},isLower=${isLower})`);
        if (hour < 12) {
            return 'صبح';
        }
		logger.debug(`<<< Exiting meridiem(hour=${hour},minute=${minute},isLower=${isLower})`);
        return 'شام';
    },
    calendar: {
        sameDay: '[آج بوقت] LT',
        nextDay: '[کل بوقت] LT',
        nextWeek: 'dddd [بوقت] LT',
        lastDay: '[گذشتہ روز بوقت] LT',
        lastWeek: '[گذشتہ] dddd [بوقت] LT',
        sameElse: 'L',
    },
    relativeTime: {
        future: '%s بعد',
        past: '%s قبل',
        s: 'چند سیکنڈ',
        ss: '%d سیکنڈ',
        m: 'ایک منٹ',
        mm: '%d منٹ',
        h: 'ایک گھنٹہ',
        hh: '%d گھنٹے',
        d: 'ایک دن',
        dd: '%d دن',
        M: 'ایک ماہ',
        MM: '%d ماہ',
        y: 'ایک سال',
        yy: '%d سال',
    },
    preparse: function (string) {
		logger.debug(`>>> Entering preparse(string=${string})`);
		logger.debug(`<<< Exiting preparse(string=${string})`);
        return string.replace(/،/g, ',');
    },
    postformat: function (string) {
		logger.debug(`>>> Entering postformat(string=${string})`);
		logger.debug(`<<< Exiting postformat(string=${string})`);
        return string.replace(/,/g, '،');
    },
    week: {
        dow: 1, // Monday is the first day of the week.
        doy: 4, // The week that contains Jan 4th is the first week of the year.
    },
});
