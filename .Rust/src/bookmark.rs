pub use crate::ffi::{FFIArray, FFIStr};
use kataru::*;
use std::os::raw::c_char;

pub static mut BOOKMARK: Option<Bookmark> = None;

/// Loads a bookmark if it exists.
/// If `default` is `true`, on failure to load it will create a new default bookmark.
fn try_load_bookmark(path: &str) -> Result<()> {
    unsafe {
        BOOKMARK = Some(Bookmark::load(path)?);
        Ok(())
    }
}
#[no_mangle]
pub extern "C" fn load_bookmark(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_load_bookmark(path))
}
fn try_save_bookmark(path: &str) -> Result<()> {
    unsafe {
        match &BOOKMARK {
            Some(bookmark) => bookmark.save(path),
            None => Err(error!("Bookmark was None.")),
        }
    }
}
#[no_mangle]
pub extern "C" fn save_bookmark(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_save_bookmark(path))
}

fn try_set_state(key: &str, value: Value) -> Result<()> {
    unsafe {
        if let Some(bookmark) = BOOKMARK.as_mut() {
            let local_state = bookmark.state()?;
            if let Some(var) = local_state.get_mut(key) {
                *var = value;
                return Ok(());
            }

            let root_state = bookmark.root_state()?;
            if let Some(var) = root_state.get_mut(key) {
                *var = value;
                return Ok(());
            }
            return Err(error!("Invalid variable name."));
        }
        Err(error!("Bookmark was None."))
    }
}
#[no_mangle]
pub extern "C" fn set_state_string(
    key: *const c_char,
    length: usize,
    value: *const c_char,
    value_length: usize,
) -> FFIStr {
    let key = FFIStr::to_str(key, length);
    let value = FFIStr::to_str(value, value_length);
    FFIStr::result(try_set_state(key, Value::String(value.to_string())))
}
#[no_mangle]
pub extern "C" fn set_state_number(key: *const c_char, length: usize, value: f64) -> FFIStr {
    let key = FFIStr::to_str(key, length);
    FFIStr::result(try_set_state(key, Value::Number(value)))
}
#[no_mangle]
pub extern "C" fn set_state_bool(key: *const c_char, length: usize, value: bool) -> FFIStr {
    let key = FFIStr::to_str(key, length);
    FFIStr::result(try_set_state(key, Value::Bool(value)))
}

#[no_mangle]
pub extern "C" fn get_passage() -> FFIStr {
    unsafe {
        if let Some(bookmark) = &BOOKMARK {
            FFIStr::from(&bookmark.position.passage)
        } else {
            FFIStr::from("")
        }
    }
}

fn try_set_line(line: usize) -> Result<()> {
    unsafe {
        if let Some(bookmark) = BOOKMARK.as_mut() {
            bookmark.position.line = line;
            Ok(())
        } else {
            Err(error!("Bookmark was None."))
        }
    }
}
#[no_mangle]
pub extern "C" fn set_line(line: usize) -> FFIStr {
    FFIStr::result(try_set_line(line))
}
