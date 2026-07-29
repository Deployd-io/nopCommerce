//! moment.js locale configuration
//! locale : Latvian [lv]
//! author : Kristaps Karlsons : https://github.com/skakri
//! author : Jānis Elmeris : https://github.com/JanisE

import moment from '../moment';

var units = {
    ss: 'sekundes_sekundēm_sekunde_sekundes'.split('_'),
    m: 'minūtes_minūtēm_minūte_minūtes'.split('_'),
    mm: 'minūtes_minūtēm_minūte_minūtes'.split('_'),
    h: 'stundas_stundām_stunda_stundas'.split('_'),
    hh: 'stundas_stundām_stunda_stundas'.split('_'),
    d: 'dienas_dienām_diena_dienas'.split('_'),
    dd: 'dienas_dienām_diena_dienas'.split('_'),
    M: 'mēneša_mēnešiem_mēnesis_mēneši'.split('_'),
    MM: 'mēneša_mēnešiem_mēnesis_mēneši'.split('_'),
    y: 'gada_gadiem_gads_gadi'.split('_'),
    yy: 'gada_gadiem_gads_gadi'.split('_'),
};
/**
 * @param withoutSuffix boolean true = a length of time; false = before/after a period of time.
 */
function format(forms, number, withoutSuffix) {
	logger.debug(`>>> Entering format(forms=${forms},number=${number},withoutSuffix=${withoutSuffix})`);
    if (withoutSuffix) {
        // E.g. "21 minūte", "3 minūtes".
        return number % 10 === 1 && number % 100 !== 11 ? forms[2] : forms[3];
    } else {
        // E.g. "21 minūtes" as in "pēc 21 minūtes".
        // E.g. "3 minūtēm" as in "pēc 3 minūtēm".
		logger.debug(`<<< Exiting format(forms=${forms},number=${number},withoutSuffix=${withoutSuffix})`);
        return number % 10 === 1 && number % 100 !== 11 ? forms[0] : forms[1];
    }
}
function relativeTimeWithPlural(number, withoutSuffix, key) {
	logger.debug(`>>> Entering relativeTimeWithPlural(number=${number},withoutSuffix=${withoutSuffix},key=${key})`);
	logger.debug(`<<< Exiting relativeTimeWithPlural(number=${number},withoutSuffix=${withoutSuffix},key=${key})`);
    return number + ' ' + format(units[key], number, withoutSuffix);
}
function relativeTimeWithSingular(number, withoutSuffix, key) {
	logger.debug(`>>> Entering relativeTimeWithSingular(number=${number},withoutSuffix=${withoutSuffix},key=${key})`);
	logger.debug(`<<< Exiting relativeTimeWithSingular(number=${number},withoutSuffix=${withoutSuffix},key=${key})`);
    return format(units[key], number, withoutSuffix);
}
function relativeSeconds(number, withoutSuffix) {
	logger.debug(`>>> Entering relativeSeconds(number=${number},withoutSuffix=${withoutSuffix})`);
	logger.debug(`<<< Exiting relativeSeconds(number=${number},withoutSuffix=${withoutSuffix})`);
    return withoutSuffix ? 'dažas sekundes' : 'dažām sekundēm';
}

export default moment.defineLocale('lv', {
    months: 'janvāris_februāris_marts_aprīlis_maijs_jūnijs_jūlijs_augusts_septembris_oktobris_novembris_decembris'.split(
        '_'
    ),
    monthsShort: 'jan_feb_mar_apr_mai_jūn_jūl_aug_sep_okt_nov_dec'.split('_'),
    weekdays:
        'svētdiena_pirmdiena_otrdiena_trešdiena_ceturtdiena_piektdiena_sestdiena'.split(
            '_'
        ),
    weekdaysShort: 'Sv_P_O_T_C_Pk_S'.split('_'),
    weekdaysMin: 'Sv_P_O_T_C_Pk_S'.split('_'),
    weekdaysParseExact: true,
    longDateFormat: {
        LT: 'HH:mm',
        LTS: 'HH:mm:ss',
        L: 'DD.MM.YYYY.',
        LL: 'YYYY. [gada] D. MMMM',
        LLL: 'YYYY. [gada] D. MMMM, HH:mm',
        LLLL: 'YYYY. [gada] D. MMMM, dddd, HH:mm',
    },
    calendar: {
        sameDay: '[Šodien pulksten] LT',
        nextDay: '[Rīt pulksten] LT',
        nextWeek: 'dddd [pulksten] LT',
        lastDay: '[Vakar pulksten] LT',
        lastWeek: '[Pagājušā] dddd [pulksten] LT',
        sameElse: 'L',
    },
    relativeTime: {
        future: 'pēc %s',
        past: 'pirms %s',
        s: relativeSeconds,
        ss: relativeTimeWithPlural,
        m: relativeTimeWithSingular,
        mm: relativeTimeWithPlural,
        h: relativeTimeWithSingular,
        hh: relativeTimeWithPlural,
        d: relativeTimeWithSingular,
        dd: relativeTimeWithPlural,
        M: relativeTimeWithSingular,
        MM: relativeTimeWithPlural,
        y: relativeTimeWithSingular,
        yy: relativeTimeWithPlural,
    },
    dayOfMonthOrdinalParse: /\d{1,2}\./,
    ordinal: '%d.',
    week: {
        dow: 1, // Monday is the first day of the week.
        doy: 4, // The week that contains Jan 4th is the first week of the year.
    },
});
