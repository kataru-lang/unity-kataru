pub use crate::ffi::FFIStr;
use crate::RUNNER;
use kataru::*;
use std::os::raw::c_char;

/// Loads a bookmark if it exists.
/// If `default` is `true`, on failure to load it will create a new default bookmark.
fn try_load_bookmark(path: &str) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.load_bookmark(Bookmark::load(path)?)
        } else {
            Err(error!("Runner was not initialized."))
        }
    }
}
#[no_mangle]
pub extern "C" fn load_bookmark(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_load_bookmark(path))
}
fn try_save_bookmark(path: &str) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.save_bookmark(path)
        } else {
            Err(error!("Runner was not initialized."))
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
        if let Some(runner) = RUNNER.as_mut() {
            runner.set_state(
                StateMod {
                    var: key,
                    op: AssignOperator::None,
                },
                value,
            )
        } else {
            Err(error!("Runner was not initialized."))
        }
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
pub extern "C" fn get_namespace() -> FFIStr {
    unsafe {
        if let Some(runner) = RUNNER.as_ref() {
            FFIStr::from(runner.namespace())
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_passage() -> FFIStr {
    unsafe {
        if let Some(runner) = RUNNER.as_ref() {
            FFIStr::from(runner.passage())
        } else {
            FFIStr::from("")
        }
    }
}

fn try_get_state(key: &str) -> String {
    let runner = unsafe {
        let Some(runner) = RUNNER.as_mut() else {
            return "\'error\": \"Runner was not initialized.\"".to_string();
        };
        runner
    };
    let Ok(value) = runner.get_state(key) else {
        return format!("{{\"error\": \"Invalid variable name {key}\"}}");
    };
    let mut params = Params::new();
    params.insert("value".to_string(), value.clone());
    match serde_json::to_string(&params) {
        Ok(json) => json,
        Err(err) => format!("{{\"error\": \"{}\"}}", err),
    }
}
#[no_mangle]
pub extern "C" fn get_state(key: *const c_char, length: usize) -> FFIStr {
    FFIStr::from(&try_get_state(FFIStr::to_str(key, length)))
}

fn try_set_line(line: usize) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.set_line(line);
            Ok(())
        } else {
            Err(error!("Runner was not initialize."))
        }
    }
}

#[no_mangle]
pub extern "C" fn set_line(line: usize) -> FFIStr {
    FFIStr::result(try_set_line(line))
}

#[no_mangle]
pub extern "C" fn get_line() -> usize {
    unsafe {
        if let Some(runner) = RUNNER.as_ref() {
            runner.line()
        } else {
            0
        }
    }
}
