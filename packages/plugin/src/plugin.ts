import streamDeck, { LogLevel } from '@elgato/streamdeck';

import { ImageProvider } from './ImageProvider';
import { ThumbnailAction } from './actions/ThumbnailAction';

streamDeck.logger.setLevel(LogLevel.TRACE);

const imageProvider = new ImageProvider();
streamDeck.actions.registerAction(new ThumbnailAction(imageProvider));
streamDeck.connect();
