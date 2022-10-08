import MarkdownIt from "markdown-it";

declare const hljs;

export function render(src: string) {
  const md = new MarkdownIt({
    highlight,
  });
  return md.render(src);
}

function highlight(str: string, lang: string) {
  if (!hljs || !lang) {
    return str;
  }

  lang = lang.toLowerCase();

  lang = getLangCodeFromExtension(lang);

  console.log("lang", lang);

  if (hljs.getLanguage(lang)) {
    try {
      return hljs.highlight(str, { language: lang }).value;
    } catch (error) {
      console.error(
        `[markdown-it-proxy] Syntax highlight for language ${lang} failed.`
      );
      return str;
    }
  } else {
    console.warn(
      `[markdown-it-proxy] Syntax highlight for language "${lang}" is not supported.`
    );
  }
}

function getLangCodeFromExtension(extension) {
  const extensionMap = {
    cs: "csharp",
    html: "markup",
    md: "markdown",
    ts: "typescript",
    py: "python",
    sh: "bash",
    yml: "yaml",
  };

  return extensionMap[extension] || extension;
}
