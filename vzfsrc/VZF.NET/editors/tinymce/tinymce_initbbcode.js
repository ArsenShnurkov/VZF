﻿tinyMCE.init({
    selector: "textarea",
    language: editorLanguage,
    plugins: "bbcode,spellchecker,charmap, image, code",
    toolbar: "bold,italic,underline,undo,redo,link,unlink,image,bbcode,code, charmap,forecolor,removeformat,cleanup,code,scayt,spellchecker",
    menubar: false,
    //theme_advanced_styles : "Code=codeStyle;Quote=quoteStyle",
    entity_encoding: "raw",
    convert_fonts_to_spans: false,
    convert_urls: false
});