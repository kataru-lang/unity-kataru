use std::{fs, os::raw::c_char};

pub use crate::{
    ffi::{FFIArray, FFIStr},
    LINE, STORY,
};

use kataru::*;

/// Generate the constants C# file.
/// Assumes story is already loaded.
#[no_mangle]
pub extern "C" fn codegen_consts(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_codegen_consts(path))
}

/// Using the already loaded story, generate constants;
fn try_codegen_consts(path: &str) -> Result<()> {
    unsafe {
        match &STORY {
            Some(story) => {
                let source = build_codegen_consts(story)?;
                if let Err(err) = fs::write(path, &source) {
                    return Err(error!(
                        "Error writing generated file to '{}': {}",
                        path, err
                    ));
                }
                Ok(())
            }
            None => Err(error!("Story was none.")),
        }
    }
}

/// Convert a kataru identifier to a C# varname.
fn get_varname(name: &str) -> String {
    if name == "global" {
        "Global".to_string()
    } else {
        name.replace(":", "_")
    }
}

/// Simultaneously construct vectors of varnames and vardefs.
fn get_vars_defs<S: AsRef<str>>(names: &[S]) -> (Vec<String>, Vec<String>) {
    let mut varnames = Vec::with_capacity(names.len());
    let mut vardefs = Vec::with_capacity(names.len());

    for name in names {
        let varname = get_varname(name.as_ref());
        vardefs.push(format!("{} = \"{}\"", varname, name.as_ref()));
        varnames.push(varname);
    }

    (varnames, vardefs)
}

/// Generates the code for
/// Make public for test access.
pub fn build_codegen_consts(story: &Story) -> Result<String> {
    // Collect all entities that need constants.
    let mut namespaces = Vec::<&str>::with_capacity(story.len());
    let mut passages = Vec::<String>::with_capacity(story.len());
    let mut characters = Vec::<String>::with_capacity(story.len());

    // Keep track of namespace boundaries for passages and characters.
    // The last passage index used for this namespace`passage_bounds[namespace_enum]`.

    for (namespace, section) in story {
        // For global namespace, don't add global to the sorted list.
        // And don't prepend the namespace.
        if namespace == kataru::GLOBAL {
            for (character, _character_data) in &section.config.characters {
                characters.push(character.to_string());
            }
            for (passage_name, _passage) in &section.passages {
                passages.push(passage_name.to_string());
            }
        } else {
            namespaces.push(namespace);
            for (character, _character_data) in &section.config.characters {
                characters.push(format!("{}:{}", namespace, character));
            }
            for (passage_name, _passage) in &section.passages {
                passages.push(format!("{}:{}", namespace, passage_name));
            }
        }
    }

    namespaces.sort();
    passages.sort();
    characters.sort();

    let (namespace_vars, namespace_defs) = get_vars_defs(&namespaces);
    let (passage_vars, passage_defs) = get_vars_defs(&passages);
    let (character_vars, character_defs) = get_vars_defs(&characters);

    let separator = ",\n            ";
    let source = format!(
        r###"/// DO NOT EDIT.
/// This file was autogenerated by Kataru based on your scripts.

namespace Kataru
{{
    public static partial class Namespaces
    {{
        public const string Global = "{global}",
            {namespace_defs};

        private static readonly string[] all = {{
            Global,
            {namespace_vars}
        }};
    }}

    public static partial class Passages
    {{
        public const string None = "",
            {passage_defs};

        private static readonly string[] all = {{
            None,
            {passage_vars}
        }};
    }}

    public static partial class Characters
    {{
        public const string None = "",
            {character_defs};

        private static readonly string[] all = {{
            None,
            {character_vars}
        }};
    }}
}}
"###,
        global = kataru::GLOBAL,
        namespace_defs = namespace_defs.join(separator),
        namespace_vars = namespace_vars.join(separator),
        passage_defs = passage_defs.join(separator),
        passage_vars = passage_vars.join(separator),
        character_defs = character_defs.join(separator),
        character_vars = character_vars.join(separator),
    );

    Ok(source)
}
