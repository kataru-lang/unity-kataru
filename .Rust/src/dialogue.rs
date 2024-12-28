pub use crate::{ffi::FFIStr, LINE};
use kataru::*;

static mut ATTRIBUTES_JSON: String = String::new();

#[no_mangle]
pub extern "C" fn get_speaker() -> FFIStr {
    unsafe {
        if let Line::Dialogue(dialogue) = &LINE {
            FFIStr::from(&dialogue.name)
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_speech() -> FFIStr {
    unsafe {
        if let Line::Dialogue(dialogue) = &LINE {
            FFIStr::from(&dialogue.text)
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_attributes() -> FFIStr {
    unsafe {
        if let Line::Dialogue(dialogue) = &LINE {
            ATTRIBUTES_JSON = match serde_json::to_string(&dialogue.attributes) {
                Ok(json) => json,
                Err(err) => format!("{{\"error\": \"{}\"}}", err),
            };
            FFIStr::from(&ATTRIBUTES_JSON)
        } else {
            FFIStr::from("{\"error\": \"Called get_params on a non-dialogue line.\"}")
        }
    }
}
