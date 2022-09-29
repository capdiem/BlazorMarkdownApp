import { defineConfig } from "rollup";
import { terser } from "rollup-plugin-terser";

import commonjs from "@rollup/plugin-commonjs";
import json from "@rollup/plugin-json";
import resolve from "@rollup/plugin-node-resolve";
import typescript from "@rollup/plugin-typescript";

export default defineConfig({
  input: "./wwwroot/lib/markdown-it-proxy.ts",
  output: [
    {
      file: "./wwwroot/js/markdown-it-proxy.min.js",
      format: "es",
      sourcemap: true,
    },
  ],
  // plugins: [typescript(), terser()],
  plugins: [resolve(), commonjs(), json(), typescript()],
  watch: {
    exclude: "node_modules/**",
  },
});
