pub use crate::{
    ffi::{FFIArray, FFIStr},
    LINE,
};
use kataru::*;

#[repr(C)]
pub enum ParamType {
    None,
    String,
    Number,
    Bool,
}

static mut COMMANDS: Vec<FFIStr> = Vec::new();
static mut PARAMS: Vec<FFIStr> = Vec::new();
static mut VALUES: Vec<Value> = Vec::new();

#[no_mangle]
pub extern "C" fn get_commands() -> usize {
    unsafe {
        if let Some(Line::Commands(cmds)) = &LINE {
            COMMANDS = Vec::new();
            COMMANDS.reserve(cmds.len());
            for cmd in cmds {
                for (cmd_name, _params) in cmd {
                    COMMANDS.push(FFIStr::from(cmd_name));
                }
            }
            cmds.len()
        } else {
            0
        }
    }
}

#[no_mangle]
pub extern "C" fn get_command(i: usize) -> FFIStr {
    unsafe { COMMANDS[i] }
}

#[no_mangle]
pub extern "C" fn get_params(i: usize) -> usize {
    unsafe {
        if let Some(Line::Commands(cmds)) = &LINE {
            let cmd = &cmds[i];
            for (_cmd_name, params) in cmd {
                PARAMS = Vec::new();
                PARAMS.reserve(params.len());
                VALUES = Vec::new();
                VALUES.reserve(params.len());
                for (param, value) in params {
                    PARAMS.push(FFIStr::from(param));
                    VALUES.push(value.clone());
                }
                return params.len();
            }
            0
        } else {
            0
        }
    }
}
#[no_mangle]
pub extern "C" fn get_param(i: usize) -> FFIStr {
    unsafe { PARAMS[i] }
}

#[no_mangle]
pub extern "C" fn get_param_type(i: usize) -> ParamType {
    unsafe {
        match VALUES[i] {
            Value::None => ParamType::None,
            Value::String(_) => ParamType::String,
            Value::Number(_) => ParamType::Number,
            Value::Bool(_) => ParamType::Bool,
        }
    }
}

#[no_mangle]
pub extern "C" fn get_param_string(i: usize) -> FFIStr {
    unsafe {
        if let Value::String(string) = &VALUES[i] {
            FFIStr::from(string)
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_param_number(i: usize) -> f64 {
    unsafe {
        if let Value::Number(float) = &VALUES[i] {
            *float
        } else {
            0.0
        }
    }
}

#[no_mangle]
pub extern "C" fn get_param_bool(i: usize) -> bool {
    unsafe {
        if let Value::Bool(bool) = &VALUES[i] {
            *bool
        } else {
            false
        }
    }
}
