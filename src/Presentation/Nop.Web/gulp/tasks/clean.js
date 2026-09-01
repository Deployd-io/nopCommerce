const logger = console;
import { deleteAsync } from 'del';
const targetPath = './wwwroot/lib_npm/';

export default function clean(cb) {
logger.debug(`>>> Entering clean(cb=${cb})`);
logger.debug(`<<< Exiting clean(cb=${cb})`);
  return deleteAsync(targetPath + '**/*').then(() => {
    cb()
  })
};