use std::fmt;
use std::os::raw::c_char;
use std::{slice, str};

static mut RESULT: String = String::new();

#[no_mangle]
pub extern "C" fn test_mod() -> FFIStr {
    FFIStr::from("Test works!")
}

#[repr(C)]
#[derive(Clone, Copy)]
pub struct FFIStr {
    strptr: *const u8,
    length: usize,
}

impl FFIStr {
    pub fn from(string: &str) -> Self {
        Self {
            strptr: string.as_ptr(),
            length: string.len(),
        }
    }

    pub fn to_str(c_chars: *const c_char, length: usize) -> &'static str {
        unsafe { &str::from_utf8_unchecked(slice::from_raw_parts(c_chars as *const u8, length)) }
    }

    pub fn result<T, E: fmt::Debug>(result: Result<T, E>) -> Self {
        match result {
            Ok(_) => Self::from(""),
            Err(e) => unsafe {
                RESULT = format!("{:?}", e);
                Self::from(&RESULT)
            },
        }
    }
}
