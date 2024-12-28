pub use crate::ffi::FFIStr;
use crate::{LINE, RUNNER};
use kataru::*;
use std::os::raw::c_char;

fn try_save_story(path: &str) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.save_story(path)
        } else {
            Err(error!("Runner was not initialized."))
        }
    }
}
#[no_mangle]
pub extern "C" fn save_story(path: *const c_char, length: usize) -> FFIStr {
    let path = FFIStr::to_str(path, length);
    FFIStr::result(try_save_story(path))
}

fn try_init_runner(story_path: &str, bookmark_path: &str, validate: bool) -> Result<()> {
    unsafe {
        RUNNER = Some(Runner::init(
            Bookmark::load(bookmark_path)?,
            Story::load(story_path)?,
            validate,
        )?);
        Ok(())
    }
}
#[no_mangle]
pub extern "C" fn init_runner(
    story_path: *const c_char,
    story_length: usize,
    bookmark_path: *const c_char,
    bookmark_length: usize,
    validate: bool,
) -> FFIStr {
    FFIStr::result(try_init_runner(
        FFIStr::to_str(story_path, story_length),
        FFIStr::to_str(bookmark_path, bookmark_length),
        validate,
    ))
}

fn try_validate() -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.validate()
        } else {
            Err(error!("Runner was not initialized."))
        }
    }
}
#[no_mangle]
pub extern "C" fn validate() -> FFIStr {
    FFIStr::result(try_validate())
}

fn try_next(input: &str) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            LINE = runner.next(input)?;
            Ok(())
        } else {
            Err(error!("Runner was not initialized."))
        }
    }
}
#[no_mangle]
pub extern "C" fn next(input: *const c_char, length: usize) -> FFIStr {
    let input = FFIStr::to_str(input, length);
    FFIStr::result(try_next(input))
}

#[no_mangle]
pub extern "C" fn tag() -> LineTag {
    unsafe { LineTag::tag(&LINE) }
}

fn try_goto_passage(passage: &str) -> Result<()> {
    unsafe {
        if let Some(runner) = RUNNER.as_mut() {
            runner.clear_stack();
            runner.goto(passage.to_string())?;
            Ok(())
        } else {
            Err(error!("Runner was not initialized."))
        }
    }
}
#[no_mangle]
pub extern "C" fn goto_passage(passage: *const c_char, length: usize) -> FFIStr {
    let passage = FFIStr::to_str(passage, length);
    FFIStr::result(try_goto_passage(passage))
}

fn try_save_snapshot(name: &str) -> Result<()> {
    unsafe {
        match RUNNER.as_mut() {
            Some(runner) => {
                runner.save_snapshot(name);
                Ok(())
            }
            None => Err(error!("Bookmark was None.")),
        }
    }
}
#[no_mangle]
pub extern "C" fn save_snapshot(name: *const c_char, length: usize) -> FFIStr {
    let name = FFIStr::to_str(name, length);
    FFIStr::result(try_save_snapshot(name))
}

fn try_load_snapshot(name: &str) -> Result<()> {
    unsafe {
        match RUNNER.as_mut() {
            Some(runner) => runner.load_snapshot(name),
            None => Err(error!("Bookmark was None.")),
        }
    }
}
#[no_mangle]
pub extern "C" fn load_snapshot(name: *const c_char, length: usize) -> FFIStr {
    let name = FFIStr::to_str(name, length);
    FFIStr::result(try_load_snapshot(name))
}
