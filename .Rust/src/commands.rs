pub use crate::{ffi::FFIStr, LINE};
use kataru::*;
use serde_json;

static mut PARAMS_JSON: String = String::new();

#[no_mangle]
pub extern "C" fn get_command() -> FFIStr {
    unsafe {
        if let Line::Command(command) = &LINE {
            return FFIStr::from(&command.name);
        }
        FFIStr::from("")
    }
}

#[no_mangle]
pub extern "C" fn get_params() -> FFIStr {
    unsafe {
        if let Line::Command(command) = &LINE {
            match serde_json::to_string(&command.params) {
                Ok(json) => PARAMS_JSON = json,
                Err(err) => PARAMS_JSON = format!("{{\"error\": \"{}\"}}", err),
            }
            FFIStr::from(&PARAMS_JSON)
        } else {
            FFIStr::from("{\"error\": \"Called get_params on a non-command line.\"}")
        }
    }
}
