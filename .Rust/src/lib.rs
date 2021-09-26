mod ffi;

mod dialogue;
pub use dialogue::{
    get_attribute, get_attribute_positions, get_attributes, get_speaker, get_speech,
};

mod bookmark;
pub use bookmark::{
    load_bookmark, set_line, set_state_bool, set_state_number, set_state_string, BOOKMARK,
};

mod story;
pub use story::{init_runner, load_story, next, save_story, validate, LINE, STORY};

mod choices;
pub use choices::{get_choice, get_choices, get_timeout};

mod commands;
pub use commands::{get_command, get_params};

mod codegen;
pub use codegen::{build_codegen_consts, codegen_consts};
