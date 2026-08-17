//! moment.js locale configuration
//! locale : Afrikaans [af]
//! author : Werner Mollentze : https://github.com/wernerm

import moment from '../moment';
const logger = console;

export default moment.defineLocale('af', {
    months: 'Januarie_Februarie_Maart_April_Mei_Junie_Julie_Augustus_September_Oktober_November_Desember'.split(
        '_'
    ),
    monthsShort: 'Jan_Feb_Mrt_Apr_Mei_Jun_Jul_Aug_Sep_Okt_Nov_Des'.split('_'),
    weekdays: 'Sondag_Maandag_Dinsdag_Woensdag_Donderdag_Vrydag_Saterdag'.split(
        '_'
    ),
    weekdaysShort: 'Son_Maa_Din_Woe_Don_Vry_Sat'.split('_'),
    weekdaysMin: 'So_Ma_Di_Wo_Do_Vr_Sa'.split('_'),
    meridiemParse: /vm|nm/i,
    isPM: function (input) {
		logger.debug(`>>> Entering isPM(input=${input})`);
		logger.debug(`<<< Exiting isPM(input=${input})`);
        return /^nm$/i.test(input);
    },
    meridiem: function (hours, minutes, isLower) {
		logger.debug(`>>> Entering meridiem(hours=${hours},minutes=${minutes},isLower=${isLower})`);
        if (hours < 12) {
            return isLower ? 'vm' : 'VM';
        } else {
			logger.debug(`<<< Exiting meridiem(hours=${hours},minutes=${minutes},isLower=${isLower})`);
            return isLower ? 'nm' : 'NM';
        }
    },
    longDateFormat: {
        LT: 'HH:mm',
        LTS: 'HH:mm:ss',
        L: 'DD/MM/YYYY',
        LL: 'D MMMM YYYY',
        LLL: 'D MMMM YYYY HH:mm',
        LLLL: 'dddd, D MMMM YYYY HH:mm',
    },
    calendar: {
        sameDay: '[Vandag om] LT',
        nextDay: '[Môre om] LT',
        nextWeek: 'dddd [om] LT',
        lastDay: '[Gister om] LT',
        lastWeek: '[Laas] dddd [om] LT',
        sameElse: 'L',
    },
    relativeTime: {
        future: 'oor %s',
        past: '%s gelede',
        s: "'n paar sekondes",
        ss: '%d sekondes',
        m: "'n minuut",
        mm: '%d minute',
        h: "'n uur",
        hh: '%d ure',
        d: "'n dag",
        dd: '%d dae',
        M: "'n maand",
        MM: '%d maande',
        y: "'n jaar",
        yy: '%d jaar',
    },
    dayOfMonthOrdinalParse: /\d{1,2}(ste|de)/,
    ordinal: function (number) {
		logger.debug(`>>> Entering ordinal(number=${number})`);
        return (
            number +
            (number === 1 || number === 8 || number >= 20 ? 'ste' : 'de')
        ); // Thanks to Joris Röling : https://github.com/jjupiter
    },
    week: {
        dow: 1, // Maandag is die eerste dag van die week.
        doy: 4, // Die week wat die 4de Januarie bevat is die eerste week van die jaar.
    },
});
