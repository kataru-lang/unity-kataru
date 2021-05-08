pub use crate::{
    ffi::{FFIArray, FFIStr},
    LINE,
};
use kataru::*;

#[repr(C)]
pub enum ParamType {
    String,
    Number,
    Bool,
}

static mut PARAMS: Vec<FFIStr> = Vec::new();
static mut VALUES: Vec<Value> = Vec::new();

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
pub extern "C" fn get_params() -> usize {
    unsafe {
        if let Line::Command(command) = &LINE {
            PARAMS = Vec::new();
            PARAMS.reserve(command.params.len());
            VALUES = Vec::new();
            VALUES.reserve(command.params.len());
            for (param, value) in &command.params {
                PARAMS.push(FFIStr::from(param));
                VALUES.push(value.clone());
            }
            return command.params.len();
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
