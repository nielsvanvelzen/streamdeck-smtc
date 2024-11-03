import commonjs from '@rollup/plugin-commonjs';
import nodeResolve from '@rollup/plugin-node-resolve';
import typescript from '@rollup/plugin-typescript';
import path from 'node:path';
import url from 'node:url';

/**
 * @type {import('rollup').RollupOptions}
 */
const config = {
	input: 'src/plugin.ts',
	output: {
		file: `../../nl.ndat.win-smtc.sdPlugin/bin/plugin.js`,
		sourcemap: !!process.env.ROLLUP_WATCH,
		sourcemapPathTransform: (relativeSourcePath, sourcemapPath) => {
			return url.pathToFileURL(path.resolve(path.dirname(sourcemapPath), relativeSourcePath)).href;
		},
	},
	plugins: [
		{
			name: 'watch-externals',
			buildStart: function () {
				this.addWatchFile(`../../nl.ndat.win-smtc.sdPlugin/manifest.json`);
			},
		},
		typescript({
			mapRoot: !!process.env.ROLLUP_WATCH ? './' : undefined,
		}),
		nodeResolve({
			browser: false,
			exportConditions: ['node'],
			preferBuiltins: true,
		}),
		commonjs(),
		{
			name: 'emit-module-package-file',
			generateBundle() {
				this.emitFile({
					fileName: 'package.json',
					source: `{ "type": "module" }`,
					type: 'asset',
				});
			},
		},
	],
};

export default config;
