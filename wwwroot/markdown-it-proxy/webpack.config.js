const path = require("path");

module.exports = {
  entry: "./index.ts",
  mode: "none",
  module: {
    rules: [
      {
        test: /\.tsx?$/,
        exclude: /(node_modules|bower_components)/,
        use: {
          loader: "babel-loader",
          options: {
            presets: ["@babel/preset-env", "@babel/preset-typescript"],
            plugins: [
              [
                "prismjs",
                {
                  languages: ["javascript", "css", "markup", "csharp", "sql"],
                  // "plugins": ["line-numbers"],
                  // "theme": "twilight",
                  // "css": true
                },
              ],
            ],
          },
        },
      },
      {
        test: /\.tsx?$/,
        use: "ts-loader",
        exclude: /node_modules/,
      },
    ],
  },
  resolve: {
    extensions: [".ts", ".js"],
  },
  experiments: {
    outputModule: true,
  },
  output: {
    filename: "markdown-it-proxy.min.js",
    module: true,
    path: path.resolve(__dirname, "../js"),
  },
};
