pub use crate::{
    ffi::{FFIArray, FFIStr},
    LINE,
};
use kataru::*;

static mut CHOICES: Vec<FFIStr> = Vec::new();

#[no_mangle]
pub extern "C" fn get_choices() -> usize {
    unsafe {
        if let Some(Line::Choices(choices)) = &LINE {
            CHOICES = Vec::new();
            CHOICES.reserve(choices.choices.len());
            for (choice, _target) in &choices.choices {
                CHOICES.push(FFIStr::from(choice));
            }
            choices.choices.len()
        } else {
            0
        }
    }
}

#[no_mangle]
pub extern "C" fn get_choice(i: usize) -> FFIStr {
    unsafe { CHOICES[i] }
}

#[no_mangle]
pub extern "C" fn get_timeout() -> f64 {
    unsafe {
        if let Some(Line::Choices(choices)) = &LINE {
            choices.timeout
        } else {
            0.0
        }
    }
}
