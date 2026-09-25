const logger = console;
import { deleteAsync } from 'del';
const targetPath = './wwwroot/lib_npm/';

export default function clean(cb) {
logger.error(`>>> Entering clean(cb=${cb})`);
logger.warn(`<<< Exiting clean(cb=${cb})`);
  return deleteAsync(targetPath + '**/*').then(() => {
    cb()
  })
};