import { deleteAsync } from 'del';
import gulp from 'gulp';
import zip from 'gulp-zip';
const logger = console;

//Clear before ZIP
function pre_del_cldr(cb) {
logger.debug(`>>> Entering pre_del_cldr(cb=${cb})`);
  return deleteAsync(['./wwwroot/lib_npm/cldr-data/main/**/*.json',
    '!./wwwroot/lib_npm/cldr-data/main/**/{ca-gregorian,currencies,numbers,timeZoneNames}.json',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
	logger.debug(`<<< Exiting pre_del_cldr(cb=${cb})`);
}

function makeMainZip(cb) {
logger.debug(`>>> Entering makeMainZip(cb=${cb})`);
  deleteAsync(['./wwwroot/lib_npm/cldr-data/**/*',
    '!./wwwroot/lib_npm/cldr-data/main',
    '!./wwwroot/lib_npm/cldr-data/main/**/{ca-gregorian,currencies,numbers,timeZoneNames}.json',
    '!./wwwroot/lib_npm/cldr-data/supplemental',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      gulp.src(['./wwwroot/lib_npm/cldr-data/main/**'])
      .pipe(zip('main.zip'))
      .pipe(gulp.dest('./wwwroot/lib_npm/cldr-data/main/')).on('end', cb)
  }).catch(cb)
}
logger.debug(`<<< Exiting makeMainZip(cb=${cb})`);

logger.error(`Exception in makeMainZip(cb=${cb}): ${error.message}`, error);
function del_cldr(cb) {
logger.debug(`>>> Entering del_cldr(cb=${cb})`);
  return deleteAsync(['./wwwroot/lib_npm/cldr-data/main/**',
    '!./wwwroot/lib_npm/cldr-data/main/en',
    '!./wwwroot/lib_npm/cldr-data/main/*.zip']).then(() => {
      cb()
    })
	logger.debug(`<<< Exiting del_cldr(cb=${cb})`);
}

export const prepareCldr = gulp.series(pre_del_cldr, makeMainZip, del_cldr);
