import { action, SingletonAction, type WillAppearEvent, type WillDisappearEvent } from '@elgato/streamdeck';
import type { ImageProvider } from '../ImageProvider';

@action({ UUID: 'nl.ndat.win-smtc.thumbnail' })
export class ThumbnailAction extends SingletonAction {
	constructor(private readonly imageProvider: ImageProvider) {
		super();
	}

	override async onWillAppear(ev: WillAppearEvent): Promise<void> {
		this.imageProvider.add(ev.action.id);
	}

	override async onWillDisappear(ev: WillDisappearEvent): Promise<void> {
		this.imageProvider.remove(ev.action.id);
	}
}
