use kataru::Value;
use kataru_ffi::{get_params, get_state, init_runner, next, set_state_bool, RUNNER};

#[test]
fn test_bookmark() {
    let story_path = "tests/data/story";
    let bookmark_path = "tests/data/bookmark.yml";
    let validate = true;
    init_runner(
        story_path.as_ptr() as *const i8,
        story_path.len(),
        bookmark_path.as_ptr() as *const i8,
        bookmark_path.len(),
        validate,
    );

    // Test get state
    {
        let varname = "var";
        let result = get_state(varname.as_ptr() as *const i8, varname.len());
        assert_eq!(result.as_str(), "{\"value\":false}");
    }
    // Test set state
    {
        let varname = "var";
        set_state_bool(varname.as_ptr() as *const i8, varname.len(), true);
        unsafe {
            assert!(RUNNER.is_some());
            let runner = RUNNER.as_mut().unwrap();
            assert_eq!(runner.get_state(varname).unwrap(), &Value::Bool(true));
        }
    }

    // Test commands
    {
        let input = "";
        next(input.as_ptr() as *const i8, input.len());
        let res = next(input.as_ptr() as *const i8, input.len());
        assert_eq!(res.as_str(), "");
        let params = get_params();
        assert_eq!(params.as_str(), "{\"param\":1.0}");
    }
}
