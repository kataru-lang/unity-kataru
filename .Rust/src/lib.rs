mod ffi;
pub use ffi::FFIStr;

mod dialogue;
pub use dialogue::{get_attributes, get_speaker, get_speech};

mod bookmark;
pub use bookmark::{
    get_state, load_bookmark, save_bookmark, set_line, set_state_bool, set_state_number,
    set_state_string,
};

mod story;
use kataru::{Line, Runner};
pub use story::{init_runner, next, save_story, validate};

mod choices;
pub use choices::{get_choice, get_choices, get_timeout};

mod commands;
pub use commands::{get_command, get_params};

mod codegen;
pub use codegen::{build_codegen_consts, codegen_consts};

/// Global static mutable variables.
pub static mut RUNNER: Option<Runner> = None;
pub static mut LINE: Line = Line::End;
