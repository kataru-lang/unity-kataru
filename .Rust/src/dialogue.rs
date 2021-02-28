pub use crate::{
    ffi::{FFIArray, FFIStr},
    LINE,
};
use kataru::*;

static mut ATTRIBUTES: Vec<FFIStr> = Vec::new();
static mut ATTRIBUTE_POS_I32: Vec<Vec<i32>> = Vec::new();
static mut ATTRIBUTE_POS: Vec<FFIArray> = Vec::new();

#[no_mangle]
pub extern "C" fn get_speaker() -> FFIStr {
    unsafe {
        if let Some(Line::Dialogue(dialogue)) = &LINE {
            FFIStr::from(&dialogue.name)
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_speech() -> FFIStr {
    unsafe {
        if let Some(Line::Dialogue(dialogue)) = &LINE {
            FFIStr::from(&dialogue.text)
        } else {
            FFIStr::from("")
        }
    }
}

#[no_mangle]
pub extern "C" fn get_attributes() -> usize {
    unsafe {
        if let Some(Line::Dialogue(dialogue)) = &LINE {
            ATTRIBUTES.clear();
            ATTRIBUTES.reserve(dialogue.attributes.len());
            ATTRIBUTE_POS_I32.clear();
            ATTRIBUTE_POS_I32.reserve(dialogue.attributes.len());
            ATTRIBUTE_POS.clear();
            ATTRIBUTE_POS.reserve(dialogue.attributes.len());

            for (attribute, positions) in &dialogue.attributes {
                // Create the converted i32 array
                let mut pos_i32: Vec<i32> = Vec::new();
                pos_i32.reserve(positions.len());
                for pos in positions {
                    pos_i32.push(*pos as i32);
                }

                ATTRIBUTE_POS_I32.push(pos_i32);
                ATTRIBUTE_POS.push(FFIArray::from(ATTRIBUTE_POS_I32.last().unwrap()));
                ATTRIBUTES.push(FFIStr::from(attribute));
            }
            dialogue.attributes.len()
        } else {
            0
        }
    }
}

#[no_mangle]
pub extern "C" fn get_attribute(i: usize) -> FFIStr {
    unsafe { ATTRIBUTES[i] }
}

#[no_mangle]
pub extern "C" fn get_attribute_positions(i: usize) -> FFIArray {
    unsafe { ATTRIBUTE_POS[i] }
}
