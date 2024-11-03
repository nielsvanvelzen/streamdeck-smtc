import streamdeck from '@elgato/streamdeck';
import { spawn, type ChildProcessWithoutNullStreams } from 'child_process';
import path from 'path';
import { type Interface } from 'readline';
import readline from 'readline/promises';

interface MediaProperties {
	artist?: string;
	title?: string;

	thumbnail_content_type?: string;
	thumbnail_content?: string;
}

export class ImageProvider {
	private _currentProcess: ChildProcessWithoutNullStreams | null = null;
	private _currentReadLine: Interface | null = null;
	private _actions = new Set<string>();

	constructor() {
		setInterval(() => this._update(), 1000);
	}

	remove(action: string) {
		this._actions.delete(action);
		this._update();
	}

	add(action: string) {
		this._actions.add(action);
		this._update();
	}

	private _update() {
		if (!this._actions.size) {
			this._currentReadLine?.close();
			this._currentReadLine = null;

			this._currentProcess?.kill();
			this._currentProcess = null;
		} else if (!this._currentProcess) {
			this._currentProcess = spawn(path.resolve('./bridge/SmtcBridge.exe'));
			this._currentReadLine = readline.createInterface(this._currentProcess.stdout, this._currentProcess.stdin);
			this._currentReadLine.addListener('line', line => {
				const json = JSON.parse(line);
				// streamdeck.logger.trace(json);
				this._setMediaProperties(json);
			});
		}
	}

	private async _setMediaProperties(mediaProperties: MediaProperties) {
		const promises: Promise<unknown>[] = [];

		for (const id of this._actions) {
			const action = streamdeck.actions.getActionById(id);
			if (!action) continue;

			if (mediaProperties.thumbnail_content_type && mediaProperties.thumbnail_content) {
				// Some application will include multiple comma delimited content types, just use the first
				const contentType = mediaProperties.thumbnail_content_type.split(',')[0];
				const dataUrl = `data:${contentType};base64,${mediaProperties.thumbnail_content}`;
				promises.push(action.setImage(dataUrl, { state: 1 }));
				if ('setState' in action) promises.push(action.setState(1));

				promises.push(action.setTitle(''));
			} else {
				promises.push(action.setImage(undefined, { state: 1 }));
				promises.push(action.setTitle(mediaProperties.title ?? ''));
				if ('setState' in action) promises.push(action.setState(0));
			}
		}

		await Promise.allSettled(promises);
	}
}
