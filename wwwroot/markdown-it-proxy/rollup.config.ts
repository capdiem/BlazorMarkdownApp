import { defineConfig } from "rollup";
import { terser } from "rollup-plugin-terser";

import babel from "@rollup/plugin-babel";
import commonjs from "@rollup/plugin-commonjs";
import json from "@rollup/plugin-json";
import resolve from "@rollup/plugin-node-resolve";
import typescript from "@rollup/plugin-typescript";

export default defineConfig({
  input: "./index.ts",
  output: [
    {
      file: "../js/markdown-it-proxy.min.js",
      format: "es",
      sourcemap: true,
    },
  ],
  // plugins: [typescript(), terser()],
  plugins: [
    typescript(),
    json(),
    resolve(),
    commonjs(),
    babel({ babelHelpers: "bundled" }),
  ],
  watch: {
    exclude: "node_modules/**",
  },
});
