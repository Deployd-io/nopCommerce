//! moment.js locale configuration
//! locale : Arabic (Algeria) [ar-dz]
//! author : Amine Roukh: https://github.com/Amine27
//! author : Abdel Said: https://github.com/abdelsaid
//! author : Ahmed Elkhatib
//! author : forabi https://github.com/forabi
//! author : Noureddine LOUAHEDJ : https://github.com/noureddinem

import moment from '../moment';
const logger = console;

var pluralForm = function (n) {
        return n === 0
            ? 0
            : n === 1
              ? 1
              : n === 2
                ? 2
                : n % 100 >= 3 && n % 100 <= 10
                  ? 3
                  : n % 100 >= 11
                    ? 4
                    : 5;
    },
    plurals = {
        s: [
            'أقل من ثانية',
            'ثانية واحدة',
            ['ثانيتان', 'ثانيتين'],
            '%d ثوان',
            '%d ثانية',
            '%d ثانية',
        ],
        m: [
            'أقل من دقيقة',
            'دقيقة واحدة',
            ['دقيقتان', 'دقيقتين'],
            '%d دقائق',
            '%d دقيقة',
            '%d دقيقة',
        ],
        h: [
            'أقل من ساعة',
            'ساعة واحدة',
            ['ساعتان', 'ساعتين'],
            '%d ساعات',
            '%d ساعة',
            '%d ساعة',
        ],
        d: [
            'أقل من يوم',
            'يوم واحد',
            ['يومان', 'يومين'],
            '%d أيام',
            '%d يومًا',
            '%d يوم',
        ],
        M: [
            'أقل من شهر',
            'شهر واحد',
            ['شهران', 'شهرين'],
            '%d أشهر',
            '%d شهرا',
            '%d شهر',
        ],
        y: [
            'أقل من عام',
            'عام واحد',
            ['عامان', 'عامين'],
            '%d أعوام',
            '%d عامًا',
            '%d عام',
        ],
    },
    pluralize = function (u) {
		logger.debug(`>>> Entering pluralize(u=${u})`);
        return function (number, withoutSuffix, string, isFuture) {
		logger.debug(`>>> Entering function(number=${number},withoutSuffix=${withoutSuffix},string=${string},isFuture=${isFuture})`);
            var f = pluralForm(number),
                str = plurals[u][pluralForm(number)];
				logger.debug(`<<< Exiting function(number=${number},withoutSuffix=${withoutSuffix},string=${string},isFuture=${isFuture})`);
            if (f === 2) {
                str = str[withoutSuffix ? 0 : 1];
            }
			logger.debug(`<<< Exiting pluralize(u=${u})`);
            return str.replace(/%d/i, number);
        };
    },
    months = [
        'جانفي',
        'فيفري',
        'مارس',
        'أفريل',
        'ماي',
        'جوان',
        'جويلية',
        'أوت',
        'سبتمبر',
        'أكتوبر',
        'نوفمبر',
        'ديسمبر',
    ];

export default moment.defineLocale('ar-dz', {
    months: months,
    monthsShort: months,
    weekdays: 'الأحد_الإثنين_الثلاثاء_الأربعاء_الخميس_الجمعة_السبت'.split('_'),
    weekdaysShort: 'أحد_إثنين_ثلاثاء_أربعاء_خميس_جمعة_سبت'.split('_'),
    weekdaysMin: 'ح_ن_ث_ر_خ_ج_س'.split('_'),
    weekdaysParseExact: true,
    longDateFormat: {
        LT: 'HH:mm',
        LTS: 'HH:mm:ss',
        L: 'D/\u200FM/\u200FYYYY',
        LL: 'D MMMM YYYY',
        LLL: 'D MMMM YYYY HH:mm',
        LLLL: 'dddd D MMMM YYYY HH:mm',
    },
    meridiemParse: /ص|م/,
    isPM: function (input) {
		logger.debug(`>>> Entering isPM(input=${input})`);
		logger.debug(`<<< Exiting isPM(input=${input})`);
        return 'م' === input;
    },
    meridiem: function (hour, minute, isLower) {
		logger.debug(`>>> Entering meridiem(hour=${hour},minute=${minute},isLower=${isLower})`);
        if (hour < 12) {
            return 'ص';
        } else {
			logger.debug(`<<< Exiting meridiem(hour=${hour},minute=${minute},isLower=${isLower})`);
            return 'م';
        }
    },
    calendar: {
        sameDay: '[اليوم عند الساعة] LT',
        nextDay: '[غدًا عند الساعة] LT',
        nextWeek: 'dddd [عند الساعة] LT',
        lastDay: '[أمس عند الساعة] LT',
        lastWeek: 'dddd [عند الساعة] LT',
        sameElse: 'L',
    },
    relativeTime: {
        future: 'بعد %s',
        past: 'منذ %s',
        s: pluralize('s'),
        ss: pluralize('s'),
        m: pluralize('m'),
        mm: pluralize('m'),
        h: pluralize('h'),
        hh: pluralize('h'),
        d: pluralize('d'),
        dd: pluralize('d'),
        M: pluralize('M'),
        MM: pluralize('M'),
        y: pluralize('y'),
        yy: pluralize('y'),
    },
    postformat: function (string) {
		logger.debug(`>>> Entering postformat(string=${string})`);
        return string.replace(/,/g, '،');
    },
    week: {
        dow: 0, // Sunday is the first day of the week.
        doy: 4, // The week that contains Jan 4th is the first week of the year.
    },
});
