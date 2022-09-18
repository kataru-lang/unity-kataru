use kataru::{Bookmark, Value};
use kataru_ffi::{load_bookmark, load_story, set_state_bool, BOOKMARK};

#[test]
fn test_build_codegen_consts() {
    // let story = Story::load("tests/data/story").unwrap();
    let story_path = "tests/data/story";
    load_story(story_path.as_ptr() as *const i8, story_path.len());
    let bookmark_path = "tests/data/bookmark.yml";
    load_bookmark(bookmark_path.as_ptr() as *const i8, bookmark_path.len());
    let varname = "var";
    set_state_bool(varname.as_ptr() as *const i8, varname.len(), true);
    unsafe {
        assert!(BOOKMARK.is_some());
        let bookmark: &Bookmark = BOOKMARK.as_ref().unwrap();
        assert_eq!(bookmark.value(varname).unwrap(), &Value::Bool(true));
    }
}
