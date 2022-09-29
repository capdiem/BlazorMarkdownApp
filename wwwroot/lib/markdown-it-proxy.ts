import MarkdownIt from "markdown-it";
import Prism from "prismjs";

export function render(src: string) {
  const md = new MarkdownIt({
    highlight,
  });
  var code = md.render(src);
  // console.log(code)
  return code;
}

function highlight(str: string, lang: string) {
  console.log("lang", lang);

  if (!lang) {
    return str;
  }

  lang = lang.toLowerCase();
  const rawLang = lang;

  lang = getLangCodeFromExtension(lang);

  console.log("lang2", lang);

  console.log("Prism.languages", Prism.languages);

  if (!Prism.languages[lang]) {
    try {
      loadLanguages([lang]);
    } catch (e) {
      console.warn(
        `[markdown-it-proxy] Syntax highlight for language "${lang}" is not supported.`
      );
    }
  }

  if (Prism.languages[lang]) {
    const code = Prism.highlight(str, Prism.languages[lang], lang);
    console.log("code", code);
    return code;
  }

  return str;
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
